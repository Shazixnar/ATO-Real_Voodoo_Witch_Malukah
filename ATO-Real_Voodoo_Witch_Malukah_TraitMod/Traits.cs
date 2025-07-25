using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Obeliskial_Content;
using Obeliskial_Essentials;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Collections;
using TMPro.Examples;
using System.Text;
using System.Text.RegularExpressions;

namespace TraitMod
{
    [HarmonyPatch]
    internal class Traits
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Character), "DamageBonus")]
        public static void DamageBonusPostfix(ref Character __instance, ref float[] __result)
        {
            if (AtOManager.Instance.TeamHaveTrait("shazixnarhealingbrew") && __instance.GetMaxHP() > 120 && __instance.IsHero)
            {
                int hpDifference = __instance.GetMaxHP() - 120;
                if (hpDifference >= 9)
                {
                    __result[0] += (float)(hpDifference / 9f * 0.2);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Character), "GetTraitDamagePercentModifiers")]
        public static void GetTraitDamagePercentModifiersPostfix(ref Character __instance, ref float __result)
        {
            if (AtOManager.Instance.TeamHaveTrait("shazixnarhealingbrew") && __instance.GetMaxHP() > 120 && __instance.IsHero)
            {
                int hpDifference = __instance.GetMaxHP() - 120;
                if (hpDifference >= 9)
                {
                    float num1 = hpDifference / 9f;
                    float num2 = 2f;
                    __result += num1 * num2;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Character), "HealReceivedBonus")]
        public static void HealReceivedPostfix(ref Character __instance, ref float[] __result)
        {
            if (AtOManager.Instance.TeamHaveTrait("shazixnarhealingbrew") && __instance.GetMaxHP() > 120 && __instance.IsHero)
            {
                int hpDifference = __instance.GetMaxHP() - 120;
                if (hpDifference >= 9)
                {
                    __result[0] += (float)(hpDifference / 9f * 0.2);
                    __result[1] += (float)(hpDifference / 9f * 2);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardData), "SetDescriptionNew")]
        public static void SetDescriptionNewPostfix(ref CardData __instance)
        {
            if (__instance.Item != null && __instance.Item.AuracurseCustomString == "itemCustomTextExplodeChargesMonsters")
            {
                if (__instance.Item.AuracurseCustomModValue1 - 25 >= 0)
                __instance.DescriptionNormalized = Regex.Replace(__instance.DescriptionNormalized, "monsters explode at [0-9]*", "monsters explosion charges <color=#263ABC><size=+.1>+" + (__instance.Item.AuracurseCustomModValue1 - 25) + "</color></size> stacks");
                else if (__instance.Item.AuracurseCustomModValue1 - 25 < 0)
                __instance.DescriptionNormalized = Regex.Replace(__instance.DescriptionNormalized, "monsters explode at [0-9]*", "monsters explosion charges <color=#263ABC><size=+.1>-" + (25 - __instance.Item.AuracurseCustomModValue1) + "</color></size> stacks");
            }
        }

        // list of your trait IDs
        public static string[] myTraitList = { "shazixnarjinx", "shazixnarhealingbrew" };

        public static void myDoTrait(string _trait, ref Trait __instance)
        {
            // get info you may need
            Enums.EventActivation _theEvent = Traverse.Create(__instance).Field("theEvent").GetValue<Enums.EventActivation>();
            Character _character = Traverse.Create(__instance).Field("character").GetValue<Character>();
            Character _target = Traverse.Create(__instance).Field("target").GetValue<Character>();
            int _auxInt = Traverse.Create(__instance).Field("auxInt").GetValue<int>();
            string _auxString = Traverse.Create(__instance).Field("auxString").GetValue<string>();
            CardData _castedCard = Traverse.Create(__instance).Field("castedCard").GetValue<CardData>();
            Traverse.Create(__instance).Field("character").SetValue(_character);
            Traverse.Create(__instance).Field("target").SetValue(_target);
            Traverse.Create(__instance).Field("theEvent").SetValue(_theEvent);
            Traverse.Create(__instance).Field("auxInt").SetValue(_auxInt);
            Traverse.Create(__instance).Field("auxString").SetValue(_auxString);
            Traverse.Create(__instance).Field("castedCard").SetValue(_castedCard);
            TraitData traitData = Globals.Instance.GetTraitData(_trait);
            List<CardData> cardDataList = new List<CardData>();
            List<string> heroHand = MatchManager.Instance.GetHeroHand(_character.HeroIndex);
            Hero[] teamHero = MatchManager.Instance.GetTeamHero();
            NPC[] teamNpc = MatchManager.Instance.GetTeamNPC();

            // activate traits
            if (_trait == "shazixnarjinx")
            {
                if (_target != null && _target.Alive)
                {
                    _target.SetAuraTrait(_character, "dark", 2);
                    _target.SetAuraTrait(_character, "poison", 2);
                    _character.HeroItem.ScrollCombatText(Texts.Instance.GetText("traits_Jinx", ""), Enums.CombatScrollEffectType.Trait);
                }
                return;
            }
            else if (_trait == "shazixnarhealingbrew")
            {
                if (MatchManager.Instance != null && _castedCard != null)
                {
                    traitData = Globals.Instance.GetTraitData("shazixnarhealingbrew");
                    if (MatchManager.Instance.activatedTraits != null && MatchManager.Instance.activatedTraits.ContainsKey("shazixnarhealingbrew") && MatchManager.Instance.activatedTraits["shazixnarhealingbrew"] > traitData.TimesPerTurn - 1)
                    {
                        return;
                    }
                    if (MatchManager.Instance.energyJustWastedByHero > 0 && (_castedCard.GetCardTypes().Contains(Enums.CardType.Healing_Spell) || _castedCard.GetCardTypes().Contains(Enums.CardType.Shadow_Spell)) && _character.HeroData != null)
                    {
                        if (!MatchManager.Instance.activatedTraits.ContainsKey("shazixnarhealingbrew"))
                        {
                            MatchManager.Instance.activatedTraits.Add("shazixnarhealingbrew", 1);
                        }
                        else
                        {
                            Dictionary<string, int> activatedTraits = MatchManager.Instance.activatedTraits;
                            activatedTraits["shazixnarhealingbrew"] = activatedTraits["shazixnarhealingbrew"] + 1;
                        }
                        MatchManager.Instance.SetTraitInfoText();
                        _character.ModifyEnergy(1, true);
                        if (_character.HeroItem != null)
                        {
                            _character.HeroItem.ScrollCombatText(Texts.Instance.GetText("traits_Healing Brew", "") + TextChargesLeft(MatchManager.Instance.activatedTraits["shazixnarhealingbrew"], traitData.TimesPerTurn), Enums.CombatScrollEffectType.Trait);
                            EffectsManager.Instance.PlayEffectAC("energy", true, _character.HeroItem.CharImageT, false, 0f);
                        }
                        int lowHP = teamHero[0].HpCurrent;
                        Hero hero = teamHero[0];
                        for (int i = 0; i < teamHero.Length; i++)
                        {
                            if (teamHero[i] != null && teamHero[i].HeroData != null && teamHero[i].Alive)
                            {
                                int lowHPCurrent = teamHero[i].HpCurrent;
                                if (lowHP > lowHPCurrent)
                                {
                                    lowHP = lowHPCurrent;
                                    hero = teamHero[i];
                                }
                            }
                        }
                        hero.SetAuraTrait(_character, "regeneration", 2);
                        hero.SetAuraTrait(_character, "vitality", 1);
                        if (hero.HeroItem != null)
                        {
                            EffectsManager.Instance.PlayEffectAC("regeneration", true, hero.HeroItem.CharImageT, false, 0f);
                            EffectsManager.Instance.PlayEffectAC("vitality", true, hero.HeroItem.CharImageT, false, 0f);
                        }
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Trait), "DoTrait")]
        public static bool DoTrait(Enums.EventActivation _theEvent, string _trait, Character _character, Character _target, int _auxInt, string _auxString, CardData _castedCard, ref Trait __instance)
        {
            if ((UnityEngine.Object)MatchManager.Instance == (UnityEngine.Object)null)
                return false;
            Traverse.Create(__instance).Field("character").SetValue(_character);
            Traverse.Create(__instance).Field("target").SetValue(_target);
            Traverse.Create(__instance).Field("theEvent").SetValue(_theEvent);
            Traverse.Create(__instance).Field("auxInt").SetValue(_auxInt);
            Traverse.Create(__instance).Field("auxString").SetValue(_auxString);
            Traverse.Create(__instance).Field("castedCard").SetValue(_castedCard);
            if (Content.medsCustomTraitsSource.Contains(_trait) && myTraitList.Contains(_trait))
            {
                myDoTrait(_trait, ref __instance);
                return false;
            }
            return true;
        }

        public static string TextChargesLeft(int currentCharges, int chargesTotal)
        {
            int cCharges = currentCharges;
            int cTotal = chargesTotal;
            return "<br><color=#FFF>" + cCharges.ToString() + "/" + cTotal.ToString() + "</color>";
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AtOManager), "GlobalAuraCurseModificationByTraitsAndItems")]
        public static void GlobalAuraCurseModificationByTraitsAndItemsPostfix(ref AtOManager __instance, ref AuraCurseData __result, string _type, string _acId, Character _characterCaster, Character _characterTarget)
        {
            bool flag = false;
            bool flag2 = false;
            if (_characterCaster != null && _characterCaster.IsHero)
            {
                flag = _characterCaster.IsHero;
            }
            if (_characterTarget != null && _characterTarget.IsHero)
            {
                flag2 = true;
            }
            if (_acId == "dark")
            {
                int ExplodeAtStacksModify = 25;
                if (_type == "set")
                {
                    float DamageWhenConsumedPerChargeModify = 2;
                    if (!flag2)
                    {
                        if (__instance.TeamHaveItem("thedarkone", 0, true))
                        {
                            ExplodeAtStacksModify += Globals.Instance.GetItemData("thedarkone").AuracurseCustomModValue1 - 25;
                        }
                        if (__instance.TeamHaveItem("blackdeck", 0, true))
                        {
                            ExplodeAtStacksModify += Globals.Instance.GetItemData("blackdeck").AuracurseCustomModValue1 - 25;
                        }
                        if (__instance.TeamHaveItem("cupofdeath", 0, true))
                        {
                            ExplodeAtStacksModify += Globals.Instance.GetItemData("cupofdeath").AuracurseCustomModValue1 - 25;
                        }
                        if (__instance.TeamHaveTrait("shazixnarshadowform"))
                        {
                            ExplodeAtStacksModify += 5;
                            DamageWhenConsumedPerChargeModify += 0.5f;
                        }
                        if (__instance.TeamHavePerk("mainperkdark2c"))
                        {
                            DamageWhenConsumedPerChargeModify += 0.7f;
                        }
                        __result.ExplodeAtStacks = ExplodeAtStacksModify;
                        __result.DamageWhenConsumedPerCharge = DamageWhenConsumedPerChargeModify;
                    }
                }
                else if (_type == "consume")
                {
                    float DamageWhenConsumedPerChargeModify = 2;
                    if (!flag)
                    {
                        if (__instance.TeamHaveTrait("shazixnarshadowform"))
                        {
                            DamageWhenConsumedPerChargeModify += 0.5f;
                        }
                        if (__instance.TeamHavePerk("mainperkdark2c"))
                        {
                            DamageWhenConsumedPerChargeModify += 0.7f;
                        }
                        __result.DamageWhenConsumedPerCharge = DamageWhenConsumedPerChargeModify;
                    }
                }
            }
            else if (_acId == "sharp" && _type == "set" && flag2)
            {
                if (__instance.TeamHavePerk("mainperkSharp1d"))
                { // Sharp on heros also increases the Shadow damage by 1 per charge.
                    __result.AuraDamageType3 = Enums.DamageType.Shadow;
                    __result.AuraDamageIncreasedPerStack3 = 1;
                }
                if (__instance.TeamHaveTrait("shrilltone"))
                {
                    __result.AuraDamageType4 = Enums.DamageType.Mind;
                    __result.AuraDamageIncreasedPerStack4 = 1;
                }
            }
            else if (_acId == "regeneration" && _type == "set" && flag2)
            {
                if (__instance.TeamHavePerk("mainperkregeneration1c"))
                { // Regeneration on heroes also increases all resistance by 0.5% per charge and increases Max HP by 1 per charge.
                    __result.ResistModified = Enums.DamageType.All;
                    __result.ResistModifiedPercentagePerStack = 0.5f;
                    __result.CharacterStatModified = Enums.CharacterStat.Hp;
                    __result.CharacterStatModifiedValuePerStack = 1;
                }
                if (__instance.TeamHaveTrait("shazixnarjinx"))
                { // Regeneration on heroes increases Shadow damage by 0.5 per charge.
                    __result.AuraDamageType = Enums.DamageType.Shadow;
                    __result.AuraDamageIncreasedPerStack = 0.5f;
                }
                if (__instance.TeamHaveTrait("shazixnarmojo"))
                { // Increase Max charge 25
                    __result.MaxMadnessCharges = 75;
                }
            }
            else if (_acId == "vitality" && _type == "set" && flag2)
            {
                if (__instance.TeamHaveTrait("shazixnarmojo"))
                { // Increase Max charge 25
                    __result.MaxMadnessCharges = 75;
                }
                if (__instance.TeamHavePerk("mainperkvitality1a") && _characterTarget != null)
                { // Vitality on heroes instead increases Max HP by 8 per charge.
                    __result.CharacterStatModifiedValuePerStack = 8;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "SetEvent")]
        public static void SetEventPrefix(ref Character __instance, ref Enums.EventActivation theEvent, Character target = null)
        {
        }
    }
}

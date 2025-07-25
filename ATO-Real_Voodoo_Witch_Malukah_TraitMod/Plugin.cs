﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Obeliskial_Essentials;
using static Obeliskial_Essentials.Essentials;

namespace RealVoodooWitchMalukah
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.stiffmeds.obeliskialessentials")]
    [BepInDependency("com.stiffmeds.obeliskialcontent")]
    [BepInProcess("AcrossTheObelisk.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal const int ModDate = 20250725;
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;
        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} has loaded!");
            // register with Obeliskial Essentials
            RegisterMod(
                _name: PluginInfo.PLUGIN_NAME,
                _author: "Shazixnar",
                _description: "Make Malukah gain more curse or support.",
                _version: PluginInfo.PLUGIN_VERSION,
                _date: ModDate,
                _link: @"https://across-the-obelisk.thunderstore.io/package/Shazixnar/Real_Voodoo_Witch_Malukah/",
                _contentFolder: "Real Voodoo Witch Malukah",
                _type: new string[5] { "content", "hero", "trait", "card", "perk" }
            );
            medsTexts["custommainperksharp1d"] = "Sharp on heroes also increases the Shadow damage by 1 per charge.";
            medsTexts["custommainperkregeneration1c"] = "Regeneration on heroes also increases all resistance by 0.5% per charge and increases Max HP by 1 per charge.";
            medsTexts["custommainperkvitality1a"] = "Vitality on heroes instead increases Max HP by 8 per charge.";
            medsTexts["custommainperkdark2c"] = "Dark explosion on enemies deals 0.7 more damage per charge.";
            // apply patches
            harmony.PatchAll();
        }
    }
}

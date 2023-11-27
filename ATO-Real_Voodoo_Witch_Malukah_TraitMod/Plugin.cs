using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;

namespace TraitMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.stiffmeds.obeliskialessentials")]
    [BepInProcess("AcrossTheObelisk.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal const int ModDate = 20231127;
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;
        private void Awake()
        {
            medsTexts["custommainperksharp1d"] = "Sharp on heroes also increases the Shadow damage by 1 per charge.";
            medsTexts["custommainperkregeneration1c"] = "Regeneration on heroes also increases all resistance by 0.5% per charge and increases Max HP by 1 per charge.";
            medsTexts["custommainperkvitality1a"] = "Vitality on heroes instead increases Max HP by 8 per charge.";
            medsTexts["custommainperkdark2c"] = "Dark explotion on enemies deals 0.7 more damage per charge.";
            Log = Logger;
            Log.LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} has loaded!");
            // register with Obeliskial Essentials
            AddModVersionText(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, ModDate.ToString());
            // apply patches
            harmony.PatchAll();
        }
    }
}

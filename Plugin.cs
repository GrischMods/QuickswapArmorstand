using System.Reflection;
using HarmonyLib;
using BepInEx;

namespace Grisch.QuickswapArmorstand;
    
    [BepInDependency("randyknapp.mods.extendeditemdataframework", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string ModName = "GrischQuickSwapArmorstand";
        internal const string ModVersion = "1.0.0";
        private const string Author = "grisch";
        private const string ModGUID = Author + "." + ModName;
        public static bool EIDF_Installed = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("randyknapp.mods.extendeditemdataframework");

        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Harmony harmony = new(ModGUID);
            harmony.PatchAll(assembly);
        }
        
    }
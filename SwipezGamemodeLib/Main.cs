using System;
using System.Linq;
using HarmonyLib;
using LabFusion;
using MelonLoader;
using SwipezGamemodeLib.Patches;

namespace SwipezGamemodeLib
{
    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Type type = FusionMod.FusionAssembly.GetTypes()
                .First(t => t.Name == "AccessoryInstance");
            
            // Manual harmony patch
            HarmonyInstance.Patch(type.GetMethod("Update", AccessTools.all),
                new HarmonyMethod(typeof(AccessoryInstancePatch).GetMethod("Prefix", AccessTools.all)));
        }
    }
}
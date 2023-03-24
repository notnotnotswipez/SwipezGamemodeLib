using HarmonyLib;
using LabFusion.Syncables;
using SLZ.Rig;
using UnityEngine;

namespace SwipezGamemodeLib.Patches
{
    public class BlacklistPatches
    {
        [HarmonyPatch(typeof(SyncBlacklist), "HasBlacklistedComponents")]
        public static class BlacklistBypassPatch
        {
            public static bool Prefix(GameObject go, ref bool __result)
            {
                RigManager rigManager = go.GetComponentInParent<RigManager>();
                if (rigManager != null)
                {
                    if (rigManager.name.ToLower().Contains("ragdoll"))
                    {
                        __result = false;
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
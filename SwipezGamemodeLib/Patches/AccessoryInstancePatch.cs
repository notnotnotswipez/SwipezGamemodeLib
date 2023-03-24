using System.Reflection;
using SLZ.Rig;
using SwipezGamemodeLib.Spectator;
using UnityEngine;

namespace SwipezGamemodeLib.Patches
{
    public static class AccessoryInstancePatch
    {
        public static bool Prefix(object __instance)
        {
            // Use reflection to get the rigmanager variable
            var rigManager = (RigManager) __instance.GetType().GetField("rigManager", BindingFlags.Public | BindingFlags.Instance).GetValue(__instance);
            if (rigManager)
            {
                if (PlayerIdExtensions.hiddenManagers.Contains(rigManager))
                {
                    // Get accessory gameobject with reflection
                    var accessory = (GameObject) __instance.GetType().GetField("accessory", BindingFlags.Public | BindingFlags.Instance).GetValue(__instance);
                    if (accessory)
                    {
                        accessory.SetActive(false);
                    }
                    return false;
                }
            }
            return true;
        }
    }
}
using HarmonyLib;
using SLZ.Bonelab;
using SLZ.Marrow.SceneStreaming;
using SLZ.Props;
using SLZ.Rig;
using SLZ.Zones;
using SwipezGamemodeLib.Spawning;
using UnityEngine;

namespace SwipezGamemodeLib.Patches
{
    public class RepFixingPatches
    {
        [HarmonyPatch(typeof(OpenControllerRig), "OnEarlyUpdate")]
        private class ControllerRigPatch
        {
            public static bool Prefix(OpenControllerRig __instance)
            {
                if (__instance.manager.gameObject.name.Contains("Ragdoll"))
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PullCordDevice), "StartAvatarUpdateEvent")]
        private class PullCordPreventionPatch
        {
            public static bool Prefix(PullCordDevice __instance)
            {
                if (__instance.rm.name.Contains("Ragdoll"))
                {
                    return false;
                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(PullCordDevice), "AvatarTransformationSequence")]
        private class PullCordSequencePreventPatch
        {
            public static bool Prefix(PullCordDevice __instance)
            {
                if (__instance.rm.name.Contains("Ragdoll"))
                {
                    return false;
                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(PullCordForceChange), "ForceChange")]
        private class ForceChangePullCordPreventPatch
        {
            public static bool Prefix(PullCordForceChange __instance)
            {
                if (__instance.rigManager.name.Contains("Ragdoll"))
                {
                    return false;
                }

                return true;
            }
        }

        
        [HarmonyPatch(typeof(PullCordDevice), "ForceAvatarChange")]
        private class PullCordForceAvatarPreventionPatch
        {
            public static bool Prefix(PullCordDevice __instance)
            {
                if (__instance.rm.name.Contains("Ragdoll"))
                {
                    return false;
                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(PullCordDevice), "SwapAvatar")]
        private class PullCordTransformSequencePatch
        {
            public static bool Prefix(PullCordDevice __instance)
            {
                if (__instance.rm.name.Contains("Ragdoll"))
                {
                    return false;
                }
                
                return true;
            }
        }

        [HarmonyPatch(typeof(SceneZone), "OnTriggerEnter", typeof(Collider))]
        public class SceneZoneEnterPatch
        {
            public static bool Prefix(SceneZone __instance, Collider other)
            {
                RigManager rigManager = SpawnManager.GetComponentOnObject<RigManager>(other.gameObject);
                if (rigManager)
                {
                    if (rigManager.name.Contains("Ragdoll"))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(SceneZone), "OnTriggerExit", typeof(Collider))]
        public class SceneZoneExitPatch
        {
            public static bool Prefix(SceneZone __instance, Collider other)
            {
                RigManager rigManager = SpawnManager.GetComponentOnObject<RigManager>(other.gameObject);
                if (rigManager)
                {
                    if (rigManager.name.Contains("Ragdoll"))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(ChunkTrigger), "OnTriggerExit", typeof(Collider))]
        public class ChunkTriggerExitPatch
        {
            public static bool Prefix(ChunkTrigger __instance, Collider other)
            {
                RigManager rigManager = SpawnManager.GetComponentOnObject<RigManager>(other.gameObject);
                if (rigManager)
                {
                    if (rigManager.name.Contains("Ragdoll"))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(ChunkTrigger), "OnTriggerEnter", typeof(Collider))]
        public class ChunkTriggerEnterPatch
        {
            public static bool Prefix(ChunkTrigger __instance, Collider other)
            {
                RigManager rigManager = SpawnManager.GetComponentOnObject<RigManager>(other.gameObject);
                if (rigManager)
                {
                    if (rigManager.name.Contains("Ragdoll"))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(TriggerLasers), "OnTriggerEnter", typeof(Collider))]
        public class TriggerLaserEnterPatch
        {
            public static bool Prefix(TriggerLasers __instance, Collider other)
            {
                RigManager rigManager = SpawnManager.GetComponentOnObject<RigManager>(other.gameObject);
                if (rigManager)
                {
                    if (rigManager.name.Contains("Ragdoll"))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(TriggerLasers), "OnTriggerExit", typeof(Collider))]
        public class TriggerLaserExitPatch
        {
            public static bool Prefix(TriggerLasers __instance, Collider other)
            {
                RigManager rigManager = SpawnManager.GetComponentOnObject<RigManager>(other.gameObject);
                if (rigManager)
                {
                    if (rigManager.name.Contains("Ragdoll"))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
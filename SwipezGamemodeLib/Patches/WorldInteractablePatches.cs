using System.Collections;
using HarmonyLib;
using LabFusion.Data;
using LabFusion.Grabbables;
using LabFusion.MonoBehaviours;
using LabFusion.Network;
using LabFusion.Representation;
using LabFusion.Senders;
using LabFusion.Syncables;
using LabFusion.Utilities;
using MelonLoader;
using SLZ;
using SLZ.Interaction;
using SLZ.Rig;
using SwipezGamemodeLib.Spectator;
using SwipezGamemodeLib.Utilities;
using UnityEngine;

namespace SwipezGamemodeLib.Patches
{
    public class WorldInteractablePatches
    {
        [HarmonyPatch(typeof(GrabHelper), "SendObjectAttach")]
        public class FusionAttachPatch
        {
            public static bool Prefix()
            {
                if (!FusionPlayerExtended.worldInteractable)
                {
                    return false;
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(PropSender), "SendOwnershipTransfer", typeof(ISyncable))]
        public class FusionOwnershipPatch
        {
            public static bool Prefix()
            {
                if (!FusionPlayerExtended.worldInteractable)
                {
                    return false;
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(Grip), "OnAttachedToHand")]
        public class GripAttachPatch
        {
            public static void Postfix(Grip __instance, Hand hand)
            {
                if (!FusionPlayerExtended.worldInteractable)
                {
                    if (BoneLib.Player.rigManager != null)
                    {
                        if (hand.manager == BoneLib.Player.rigManager)
                        {
                            if (__instance.HasHost)
                            {
                                if (__instance.Host.Rb != null)
                                {
                                    __instance.ForceDetach(hand);
                                }
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerRep), "AttachObject")]
        public class AttachObjectPatch
        {
            public static bool Prefix(PlayerRep __instance)
            {
                if (PlayerIdExtensions.hiddenIds.Contains(__instance.PlayerId))
                {
                    return false;
                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(PlayerRep), "DetachObject")]
        public class DetachObjectPatch
        {
            public static bool Prefix(PlayerRep __instance)
            {
                if (PlayerIdExtensions.hiddenIds.Contains(__instance.PlayerId))
                {
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(CollisionSyncer), "OnCollisionEnter")]
        public class CollisionSyncPatch
        {
            public static bool Prefix()
            {
                if (!FusionPlayerExtended.worldInteractable)
                {
                    return false;
                }

                return true;
            }
        }
        
        [HarmonyPatch(typeof(ForcePullGrip), nameof(ForcePullGrip.OnFarHandHoverUpdate))]
        public static class ForcePullPatches
        {
            public static bool Prefix(ForcePullGrip __instance, ref bool __state, Hand hand)
            {
                if (hand.manager == BoneLib.Player.rigManager)
                {
                    if (!FusionPlayerExtended.worldInteractable)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(GrabHelper), "SendObjectDetach")]
        public class FusionDetachPatch
        {
            public static bool Prefix()
            {
                if (!FusionPlayerExtended.worldInteractable)
                {
                    return false;
                }
                return true;
            }
        }
        
        [HarmonyPatch(typeof(GrabHelper), "SendObjectForcePull")]
        public class FusionForceGrabPatch
        {
            public static bool Prefix()
            {
                if (!FusionPlayerExtended.worldInteractable)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
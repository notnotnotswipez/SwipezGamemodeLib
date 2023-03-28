using System.Collections;
using HarmonyLib;
using LabFusion.Data;
using LabFusion.Grabbables;
using LabFusion.Network;
using LabFusion.Representation;
using LabFusion.Syncables;
using LabFusion.Utilities;
using MelonLoader;
using SLZ.Combat;
using SLZ.Interaction;
using SLZ.Marrow.Pool;
using SLZ.Props.Weapons;
using SLZ.Rig;
using UnityEngine;

namespace SwipezGamemodeLib.Patches
{
    public class GrabHelperOverridePatches
    {
        [HarmonyPatch(typeof(GrabHelper), "SendObjectAttach")]
        public static class GrabHelperSendObjectOverridePatch
        {
            public static bool Prefix(Hand hand, Grip grip)
            {
                // Circumvent this entire method and write our own bypass.
                // Sorry lackoftrazz, but I want my damn ragdolls to sync!
                if (NetworkInfo.HasServer)
                {
                    MelonCoroutines.Start(Internal_ObjectAttachRoutine(hand, grip));
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(SyncBlacklist), "IsSyncWhitelisted")]
        public static class SyncBlacklistOverridePatch
        {
            public static bool Prefix(GameObject go, ref bool __result)
            {
                // Circumvent this entire method and write our own bypass.
                // Sorry lackoftrazz, but I want my damn ragdolls to sync!
                RigManager rigManager = go.GetComponentInParent<RigManager>();
                if (rigManager)
                {
                    if (rigManager.name.ToLower().Contains("ragdoll"))
                    {
                        __result = true;
                        return false;
                    }
                }

                if (SyncBlacklist.HasBlacklistedComponents(go))
                {
                    __result = false;
                    return false;
                }

                // Other hardcoded stuff (probably cleanup later)
                bool hasRigidbody = go.GetComponentInChildren<Rigidbody>(true) != null;

                bool hasGunProperties = go.GetComponentInChildren<FirearmCartridge>(true) == null || go.GetComponentInChildren<Gun>(true) != null;

                bool spawnableProperties = true;

                var assetPoolee = go.GetComponentInChildren<AssetPoolee>();
                if (assetPoolee)
                    spawnableProperties = assetPoolee.spawnableCrate.Barcode != CommonBarcodes.BOARD_BARCODE;

                bool isValid = hasRigidbody && hasGunProperties && spawnableProperties;

                __result = isValid;
                return false;
            }
        }

        internal static IEnumerator Internal_ObjectAttachRoutine(Hand hand, Grip grip)
        {
            // Delay a few frames
            for (var i = 0; i < 4; i++)
                yield return null;

            if (NetworkInfo.HasServer)
            {
                var handedness = hand.handedness;

                // Get base values for the message
                byte smallId = PlayerIdManager.LocalSmallId;
                GrabGroup group = GrabGroup.UNKNOWN;
                SerializedGrab serializedGrab = null;
                bool validGrip = false;

                // If the grip exists, we'll check its stuff
                if (grip != null)
                {
                    // Check for player body grab
                    if (PlayerRepUtilities.FindAttachedPlayer(grip, out var repId, out var repReferences, out var isAvatarGrip))
                    {
                        group = GrabGroup.PLAYER_BODY;
                        serializedGrab = new SerializedPlayerBodyGrab(repId, repReferences.GetIndex(grip, isAvatarGrip).Value, isAvatarGrip);
                        validGrip = true;
                    }
                    // Check for static grips
                    else if (grip.IsStatic)
                    {
                        if (grip.TryCast<WorldGrip>() != null)
                        {
                            group = GrabGroup.WORLD_GRIP;
                            serializedGrab = new SerializedWorldGrab(smallId);
                            validGrip = true;
                        }
                        else
                        {
                            group = GrabGroup.STATIC;
                            serializedGrab = new SerializedStaticGrab(grip.gameObject.GetFullPath());
                            validGrip = true;
                        }
                    }
                    // Check for prop grips
                    else if (grip.HasRigidbody)
                    {
                        bool ignore = false;
                        RigManager rigManager = grip.GetComponentInParent<RigManager>();
                        if (rigManager)
                        {
                            if (!rigManager.name.ToLower().Contains("ragdoll"))
                            {
                                ignore = true;
                            }
                        }

                        if (!ignore)
                        {
                            group = GrabGroup.PROP;
                            GrabHelper.GetGripInfo(grip, out var host);

                            GameObject root = host.GetSyncRoot();

                            // Do we already have a synced object?
                            if (GripExtender.Cache.TryGet(grip, out var syncable) ||
                                PropSyncable.HostCache.TryGet(host.gameObject, out syncable) ||
                                PropSyncable.Cache.TryGet(root, out syncable))
                            {
                                serializedGrab = new SerializedPropGrab("_", syncable.GetIndex(grip).Value,
                                    syncable.GetId());
                                validGrip = true;
                            }
                            else
                            {
                                // Make sure the GameObject is whitelisted before syncing
                                if (!root.IsSyncWhitelisted())
                                    yield break;

                                // Create a new one
                                if (!NetworkInfo.IsServer)
                                {
                                    syncable = new PropSyncable(host);

                                    ushort queuedId = SyncManager.QueueSyncable(syncable);

                                    using (var writer = FusionWriter.Create(SyncableIDRequestData.Size))
                                    {
                                        using (var data = SyncableIDRequestData.Create(smallId, queuedId))
                                        {
                                            writer.Write(data);

                                            using (var message =
                                                   FusionMessage.Create(NativeMessageTag.SyncableIDRequest, writer))
                                            {
                                                MessageSender.BroadcastMessageExceptSelf(NetworkChannel.Reliable,
                                                    message);
                                            }
                                        }
                                    }

                                    while (syncable.IsQueued())
                                        yield return null;

                                    yield return null;

                                    serializedGrab = new SerializedPropGrab(host.gameObject.GetFullPath(),
                                        syncable.GetIndex(grip).Value, syncable.Id);
                                    validGrip = true;
                                }
                                else if (NetworkInfo.IsServer)
                                {
                                    syncable = new PropSyncable(host);
                                    SyncManager.RegisterSyncable(syncable, SyncManager.AllocateSyncID());
                                    serializedGrab = new SerializedPropGrab(host.gameObject.GetFullPath(),
                                        syncable.GetIndex(grip).Value, syncable.Id);

                                    validGrip = true;
                                }
                            }
                        }
                    }
                }

                // Now, send the message
                if (validGrip) {
                    // Write the default grip values
                    serializedGrab.WriteDefaultGrip(hand, grip);

                    using (var writer = FusionWriter.Create(PlayerRepGrabData.Size + serializedGrab.GetSize()))
                    {
                        using (var data = PlayerRepGrabData.Create(smallId, handedness, group, serializedGrab))
                        {
                            writer.Write(data);

                            using (var message = FusionMessage.Create(NativeMessageTag.PlayerRepGrab, writer))
                            {
                                MessageSender.BroadcastMessageExceptSelf(NetworkChannel.Reliable, message);
                            }
                        }
                    }
                }
            }
        }
    }
}
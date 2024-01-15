using System.Collections.Generic;
using System.Reflection;
using BoneLib;
using LabFusion.Network;
using LabFusion.Representation;
using MelonLoader;
using SLZ;
using SLZ.Rig;
using SwipezGamemodeLib.Data;
using UnityEngine;

namespace SwipezGamemodeLib.Spectator
{
    public static class PlayerIdExtensions
    {
        public static List<RigManager> hiddenManagers = new List<RigManager>();
        public static List<PlayerId> hiddenIds = new List<PlayerId>();
        internal static Dictionary<PlayerId, HeadIcon> _headIcons = new Dictionary<PlayerId, HeadIcon>();

        public static void Hide(this PlayerId playerId)
        {
            if (PlayerRepManager.TryGetPlayerRep(playerId, out var playerRep))
            {
                if (hiddenManagers.Contains(playerRep.RigReferences.RigManager))
                {
                    return;
                }
                hiddenManagers.Add(playerRep.RigReferences.RigManager);
                playerRep.DetachObject(Handedness.LEFT);
                playerRep.DetachObject(Handedness.RIGHT);
                playerRep.RigReferences.RigManager.gameObject.active = false;
                playerRep.repCanvas.gameObject.active = false;
                hiddenIds.Add(playerId);
                
                // Get private field
                var audioSourceField = playerRep.GetType().GetField("_voiceSource", BindingFlags.NonPublic | BindingFlags.Instance);
                var audioSource = audioSourceField.GetValue(playerRep) as AudioSource;
                if (audioSource)
                {
               
                    audioSource.mute = true;
                }
            }
        }

        public static void Show(this PlayerId playerId)
        {
            if (PlayerRepManager.TryGetPlayerRep(playerId, out var playerRep))
            {
                if (!hiddenManagers.Contains(playerRep.RigReferences.RigManager))
                {
                    return;
                }
                hiddenManagers.Remove(playerRep.RigReferences.RigManager);
                hiddenIds.Remove(playerId);
                playerRep.RigReferences.RigManager.gameObject.active = true;
                playerRep.repCanvas.gameObject.active = true;
                
                // A reset might be needed, there are some cases where the rep seems to freak out really badly, assuming its because the rep is trying to
                // Use velocity to pickup to where its supposed to be, but since its hidden, it cant, so it just freaks out when it comes back.
                playerRep.RigReferences.RigManager.physicsRig.ResetHands(Handedness.BOTH);
                playerRep.RigReferences.RigManager.physicsRig.UnRagdollRig();
                playerRep.RigReferences.RigManager.TeleportToPose(playerRep.serializedPelvis.position, Vector3.forward, true);
                
                var audioSourceField = playerRep.GetType().GetField("_voiceSource", BindingFlags.NonPublic | BindingFlags.Instance);
                var audioSource = audioSourceField.GetValue(playerRep) as AudioSource;
                if (audioSource)
                {
                    audioSource.mute = false;
                }
            }
        }

        public static void SetHeadIcon(this PlayerId playerId, Texture2D texture2D)
        {
            if (texture2D == null)
            {
                if (_headIcons.ContainsKey(playerId))
                {
                    _headIcons[playerId].Cleanup();
                    _headIcons.Remove(playerId);
                }
                return;
            }

            if (!_headIcons.ContainsKey(playerId))
            {
                _headIcons.Add(playerId, new HeadIcon(playerId));
            }
            HeadIcon headIcon = _headIcons[playerId];
            headIcon.SetIcon(texture2D);
        }
    }
}
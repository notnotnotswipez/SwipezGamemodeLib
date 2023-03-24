using System.Collections.Generic;
using System.Reflection;
using LabFusion.Network;
using LabFusion.Representation;
using MelonLoader;
using SLZ;
using SLZ.Rig;
using UnityEngine;

namespace SwipezGamemodeLib.Spectator
{
    public static class PlayerIdExtensions
    {
        public static List<RigManager> hiddenManagers = new List<RigManager>();
        private static Dictionary<RigManager, PlayerHiddenStorage> hiddenStorage = new Dictionary<RigManager, PlayerHiddenStorage>();

        public static void Hide(this PlayerId playerId)
        {
            if (PlayerRepManager.TryGetPlayerRep(playerId, out var playerRep))
            {
                if (hiddenManagers.Contains(playerRep.RigReferences.RigManager))
                {
                    return;
                }
                /*PlayerHiddenStorage playerHiddenStorage = new PlayerHiddenStorage();
                playerHiddenStorage.Populate(playerRep.RigReferences.RigManager);
                hiddenStorage.Add(playerRep.RigReferences.RigManager, playerHiddenStorage);*/

                hiddenManagers.Add(playerRep.RigReferences.RigManager);
                playerRep.RigReferences.LeftHand.DetachObject();
                playerRep.RigReferences.RightHand.DetachObject();
                playerRep.RigReferences.RigManager.gameObject.active = false;
                playerRep.repCanvas.gameObject.active = false;
                
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
                /*PlayerHiddenStorage playerHiddenStorage = hiddenStorage[playerRep.RigReferences.RigManager];
                playerHiddenStorage.Show();
                hiddenStorage.Remove(playerRep.RigReferences.RigManager);*/
                hiddenManagers.Remove(playerRep.RigReferences.RigManager);
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
    }
}
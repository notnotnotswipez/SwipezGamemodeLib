using System;
using BoneLib;
using LabFusion.Network;
using LabFusion.Representation;
using SwipezGamemodeLib.Module;

namespace SwipezGamemodeLib.Events
{
    public class SwipezGamemodeLibEvents
    {
        public static event Action<PlayerId, string> OnPlayerSendEvent;
        
        public static void PlayerSendEvent(PlayerId playerId, string eventName)
        {
            SafeActions.InvokeActionSafe(OnPlayerSendEvent, playerId, eventName);
        }

        public static void SendPlayerEvent(string eventName)
        {
            if (NetworkInfo.HasServer)
            {
                using (var writer = FusionWriter.Create()) {
                    using (var data = PlayerSendCustomEventMessageData.Create(PlayerIdManager.LocalId, eventName)) {
                        writer.Write(data);
                        using (var message = FusionMessage.ModuleCreate<PlayerSendCustomEventMessageHandler>(writer))
                        {
                            MessageSender.SendToServer(NetworkChannel.Reliable, message);
                        }
                    }
                }
            }
        }
    }
}
using System;
using LabFusion.Data;
using LabFusion.Network;
using LabFusion.Representation;
using SwipezGamemodeLib.Events;

namespace SwipezGamemodeLib.Module
{
    public class PlayerSendCustomEventMessageData : IFusionSerializable, IDisposable
    {
        public PlayerId sender;
        public string eventSent;
        
        public void Deserialize(FusionReader reader)
        {
            sender = PlayerIdManager.GetPlayerId(reader.ReadByte());
            eventSent = reader.ReadString();
        }

        public void Serialize(FusionWriter writer)
        {
            writer.Write(sender.SmallId);
            writer.Write(eventSent);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static PlayerSendCustomEventMessageData Create(PlayerId sender, string eventName)
        {
            return new PlayerSendCustomEventMessageData()
            {
                sender = sender,
                eventSent = eventName
            };
        }
    }
    
    public class PlayerSendCustomEventMessageHandler : ModuleMessageHandler
    {
        public override void HandleMessage(byte[] bytes, bool isServerHandled = false)
        {
            using (var reader = FusionReader.Create(bytes))
            {
                using (var data = reader.ReadFusionSerializable<PlayerSendCustomEventMessageData>())
                {
                    if (NetworkInfo.IsServer && isServerHandled) {
                        using (var message = FusionMessage.ModuleCreate<PlayerSendCustomEventMessageHandler>(bytes))
                        {
                            MessageSender.BroadcastMessageExcept(data.sender, NetworkChannel.Reliable, message);
                        }

                        return;
                    }
                    
                    SwipezGamemodeLibEvents.PlayerSendEvent(data.sender, data.eventSent);
                }
            }
        }
    }
}
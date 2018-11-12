using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon;
using NetCommon.Codes;
using Server.GameLogic.Session;
using Server.Message.Interfaces;
using System;


namespace Server.ServerHandlers
{
    public class JoinSessionHandler : INetMessageHandler
    {
        public NetOperationCode Code => NetOperationCode.JoinSession;

        public bool HandleMessage(INetMessage message)
        {
            Console.WriteLine($"JoinSessionHandler. PeerId: {message.Client.NetPeer.Id}");

            byte[] sessionId = new byte[12];
            message.Reader.GetBytes(sessionId, 12);

            NetDataWriter writer = new NetDataWriter();
            writer.Put((byte)NetOperationCode.JoinSession);

            if (message.Client.CurrentSessionId != null || !SessionCache.Instance.JoinSession(sessionId, message.Client))
                writer.Put((byte)NetErrorCode.SessionConnectedFailed);
            else
            {
                writer = new NetDataWriter();
                writer.Put((byte)NetOperationCode.JoinSession);
                writer.Put((byte)NetErrorCode.Success);

                writer.Put(message.Client.Units.Count);
                foreach (var u in message.Client.Units)
                    writer.Put(MessageSerializerService.SerializeObjectOfType(u.Value.UnitData));
            }

            message.Client.NetPeer.Send(writer, DeliveryMethod.ReliableOrdered);
            return true;
        }
    }
}

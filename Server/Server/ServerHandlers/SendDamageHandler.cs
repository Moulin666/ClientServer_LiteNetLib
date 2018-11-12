using System;
using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon;
using NetCommon.Codes;
using Server.GameLogic.Session;
using Server.Message.Interfaces;


namespace Server.ServerHandlers
{
    public class SendDamageHandler : INetMessageHandler
    {
        public NetOperationCode Code => NetOperationCode.SendDamage;

        public bool HandleMessage (INetMessage message)
        {
            Console.WriteLine($"SendDamageHandler. PeerId: {message.Client.NetPeer.Id}");

            if (message.Client.CurrentSessionId == null)
                return true;

            var session = SessionCache.Instance.GetSessionById(message.Client.CurrentSessionId);
            if (session == null)
                return true;

            var targetId = message.Reader.GetInt();
            var senderId = message.Reader.GetInt();

            if (!session.Units.ContainsKey(targetId) || !session.Units.ContainsKey(senderId))
                return true;

            var target = session.Units[targetId];
            var sender = session.Units[senderId];

            target.SendDamage(sender);

            NetDataWriter _dataWriter = new NetDataWriter();
            _dataWriter.Reset();
            _dataWriter.Put((byte)NetOperationCode.SendDamage);
            _dataWriter.Put(target.UnitData.UnitId);
            _dataWriter.Put(sender.UnitData.UnitId);
            _dataWriter.Put(target.UnitData.Health);

            Console.WriteLine($"{target.UnitData.UnitId} / {sender.UnitData.UnitId} / {target.UnitData.Health}");

            session.SendToAll(_dataWriter, DeliveryMethod.Sequenced);
            return true;
        }
    }
}

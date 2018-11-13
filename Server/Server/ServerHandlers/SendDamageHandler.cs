using System;
using LiteNetLib;
using LiteNetLib.Utils;
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
            if (message.Client.CurrentSessionId == null)
                return true;

            var session = SessionCache.Instance.GetSessionById(message.Client.CurrentSessionId);
            if (session == null)
                return true;

            var senderId = message.Reader.GetInt();
            var targetId = message.Reader.GetInt();

            if (!session.Units.ContainsKey(senderId) || !session.Units.ContainsKey(targetId))
                return true;

            lock (session.Units)
            {
                var sender = session.Units[senderId];
                var target = session.Units[targetId];

                target.GetDamage(sender);

                NetDataWriter _dataWriter = new NetDataWriter();
                _dataWriter.Reset();
                _dataWriter.Put((byte)NetOperationCode.SendDamage);
                _dataWriter.Put(senderId);
                _dataWriter.Put(targetId);
                _dataWriter.Put(target.UnitData.Health);

                Console.WriteLine($"{senderId} / {targetId} / {target.UnitData.Health}");

                session.SendToAll(_dataWriter, DeliveryMethod.Sequenced);
                return true;
            }
        }
    }
}

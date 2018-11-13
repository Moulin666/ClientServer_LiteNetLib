using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon.Codes;
using Server.GameLogic.Session;
using Server.Message.Interfaces;


namespace Server.ServerHandlers
{
    public class SendAttackUnitHandler : INetMessageHandler
    {
        public NetOperationCode Code => NetOperationCode.SendAttackUnit;

        public bool HandleMessage(INetMessage message)
        {
            if (message.Client.CurrentSessionId == null)
                return true;

            var session = SessionCache.Instance.GetSessionById(message.Client.CurrentSessionId);
            if (session == null)
                return true;

            var senderId = message.Reader.GetInt();
            var targetId = message.Reader.GetInt();

            if (!session.Units.ContainsKey(targetId) || !session.Units.ContainsKey(senderId))
                return true;

            NetDataWriter _dataWriter = new NetDataWriter();
            _dataWriter.Reset();
            _dataWriter.Put((byte)NetOperationCode.SendAttackUnit);
            _dataWriter.Put(senderId);
            _dataWriter.Put(targetId);

            session.SendToAll(_dataWriter, DeliveryMethod.Sequenced);

            return true;
        }
    }
}

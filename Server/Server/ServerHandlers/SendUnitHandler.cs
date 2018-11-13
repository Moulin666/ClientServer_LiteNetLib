using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon;
using NetCommon.Codes;
using NetCommon.MessageObjects;
using Server.GameLogic.Session;
using Server.Message.Interfaces;


namespace Server.ServerHandlers
{
    class SendUnitHandler : INetMessageHandler
    {
        public NetOperationCode Code => NetOperationCode.SendUnit;

        public bool HandleMessage(INetMessage message)
        {
            int unitId = message.Reader.GetInt();

            var serializePositionData = message.Reader.GetString();
            PositionData positionData =
                MessageSerializerService.DeserializeObjectOfType<PositionData>(serializePositionData);

            var session = SessionCache.Instance.GetSessionById(message.Client.CurrentSessionId);
            if (session == null || !session.IsStarted)
                return true;

            if (session.Units.ContainsKey(unitId))
            {
                NetDataWriter _dataWriter = new NetDataWriter();
                _dataWriter.Reset();
                _dataWriter.Put((byte)NetOperationCode.SendUnit);
                _dataWriter.Put(unitId);
                _dataWriter.Put(serializePositionData);

                foreach(var p in session.Players)
                    session.SendToAll(_dataWriter, DeliveryMethod.Sequenced);
            }

            return true;
        }
    }
}

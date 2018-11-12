using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon;
using NetCommon.Codes;
using NetCommon.MessageObjects;
using Server.GameLogic.Session;
using Server.Message.Interfaces;


namespace Server.ServerHandlers
{
    public class MoveUnitHandler : INetMessageHandler
    {
        public NetOperationCode Code => NetOperationCode.MoveUnit;

        public bool HandleMessage(INetMessage message)
        {
            int unitId = message.Reader.GetInt();

            var serializePositionData = message.Reader.GetString();
            PositionData positionData =
                MessageSerializerService.DeserializeObjectOfType<PositionData>(serializePositionData);

            var session = SessionCache.Instance.GetSessionById(message.Client.CurrentSessionId);
            if (session == null)
                return true;

            if (session.Units.ContainsKey(unitId))
            {
                session.Units[unitId].UnitData.PositionData = positionData;

                NetDataWriter _dataWriter = new NetDataWriter();
                _dataWriter.Reset();
                _dataWriter.Put((byte)NetOperationCode.MoveUnit);
                _dataWriter.Put(unitId);
                _dataWriter.Put(serializePositionData);

                foreach (var p in session.Players)
                    if (p.Value.NetPeer.Id != message.Client.NetPeer.Id)
                        p.Value.NetPeer.Send(_dataWriter, DeliveryMethod.Sequenced);
            }

            return true;
        }
    }
}

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
            if (session == null || !session.IsStarted)
                return true;

            if (session.Units.ContainsKey(unitId))
                session.Units[unitId].UnitData.PositionData = positionData;

            return true;
        }
    }
}

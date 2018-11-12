using NetCommon;
using NetCommon.Codes;
using NetCommon.MessageObjects;
using Server.GameLogic.Session;
using Server.Message.Interfaces;
using System;


namespace Server.ServerHandlers
{
    public class SetUnitPositionHandler : INetMessageHandler
    {
        public NetOperationCode Code => NetOperationCode.SetUnitPosition;

        public bool HandleMessage (INetMessage message)
        {
            Console.WriteLine($"SetUnitPositionHandler. PeerId: {message.Client.NetPeer.Id}");

            if (message.Client.CurrentSessionId == null)
                return true;

            var session = SessionCache.Instance.GetSessionById(message.Client.CurrentSessionId);
            if (session == null)
                return true;

            int unitId = message.Reader.GetInt();
            PositionData positionData = MessageSerializerService.DeserializeObjectOfType<PositionData>(message.Reader.GetString());

            if (session.Units.ContainsKey(unitId))
                session.Units[unitId].UnitData.PositionData = positionData;

            return true;
        }
    }
}

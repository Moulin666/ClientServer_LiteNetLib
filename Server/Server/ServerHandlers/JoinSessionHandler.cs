using NetCommon.Codes;
using Server.GameData;
using Server.Message.Interfaces;


namespace Server.ServerHandlers
{
    public class JoinSessionHandler : INetMessageHandler
    {
        public NetOperationCode Code => NetOperationCode.JoinSession;

        public bool HandleMessage(INetMessage message)
        {
            byte[] sessionId = new byte[12];
            message.Reader.GetBytes(sessionId, 12);

            SessionCache.Instance.JoinSession(sessionId, message.Client);

            return true;
        }
    }
}

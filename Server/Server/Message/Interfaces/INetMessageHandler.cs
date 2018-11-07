using NetCommon.Codes;


namespace Server.Message.Interfaces
{
    public interface INetMessageHandler
    {
        NetOperationCode Code { get; }

        bool HandleMessage(INetMessage message);
    }
}

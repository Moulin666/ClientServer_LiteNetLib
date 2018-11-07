using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon.Codes;


namespace Server.Message.Interfaces
{
    public interface INetMessage
    {
        NetOperationCode Code { get; }

        Client Client { get; }

        NetDataReader Reader { get; }

        DeliveryMethod DeliveryMethod { get; }
    }
}

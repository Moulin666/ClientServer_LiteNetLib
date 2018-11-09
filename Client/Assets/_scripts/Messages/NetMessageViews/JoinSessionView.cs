using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon.Codes;


public class JoinSessionView
{
    public void JoinSession ()
    {
        // TODO : get this from API server.
        byte[] sessionId = new byte[12]
        {
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1
        };

        NetDataWriter writer = new NetDataWriter();
        writer.Put((byte)NetOperationCode.JoinSession);

        foreach (var s in sessionId)
            writer.Put(s);

        ClientNetEventListener.Instance.SendOperation(writer, DeliveryMethod.ReliableOrdered);
    }
}

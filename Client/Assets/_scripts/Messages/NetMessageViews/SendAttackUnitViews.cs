using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon.Codes;


public class SendAttackUnitViews
{
    public void SendAttackUnit (int senderId, int targetId)
    {
        NetDataWriter writer = new NetDataWriter();
        writer.Reset();
        writer.Put((byte)NetOperationCode.SendAttackUnit);
        writer.Put(senderId);
        writer.Put(targetId);

        ClientNetEventListener.Instance.SendOperation(writer, DeliveryMethod.Sequenced);
    }
}

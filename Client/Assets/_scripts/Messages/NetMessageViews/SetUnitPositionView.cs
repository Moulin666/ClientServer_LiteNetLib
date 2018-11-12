using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon;
using NetCommon.Codes;
using NetCommon.MessageObjects;
using UnityEngine;


public class SetUnitPositionView
{
    public void SetPosition (int unitId, Vector3 newPosition)
    {
        PositionData positionData = new PositionData(newPosition.x, newPosition.y, newPosition.z);

        NetDataWriter writer = new NetDataWriter();
        writer.Put((byte)NetOperationCode.SetUnitPosition);
        writer.Put(unitId);
        writer.Put(MessageSerializerService.SerializeObjectOfType(positionData));

        ClientNetEventListener.Instance.SendOperation(writer, DeliveryMethod.ReliableOrdered);
    }
}

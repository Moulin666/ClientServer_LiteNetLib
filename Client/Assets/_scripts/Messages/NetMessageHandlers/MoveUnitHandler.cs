using LiteNetLib.Utils;
using NetCommon;
using NetCommon.MessageObjects;
using UnityEngine;


public class MoveUnitHandler : NetMessageHandler
{
    /// <summary>
    /// Handle message.
    /// </summary>
    /// <param name="reader">Reader of parameters</param>
    protected override void OnHandleMessage(NetDataReader reader)
    {
        var id = reader.GetInt();
        PositionData positionData = MessageSerializerService.DeserializeObjectOfType<PositionData>(reader.GetString());

        Vector3 newPosition = new Vector3(positionData.X, positionData.Y, positionData.Z);
        ClientNetEventListener.Instance.OnMoveEvent(id, newPosition);

        Debug.Log(id + " / " + newPosition);
    }
}

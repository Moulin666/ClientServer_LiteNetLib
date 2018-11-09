using LiteNetLib.Utils;
using NetCommon;
using NetCommon.MessageObjects;
using UnityEngine;


public class WorldEnterHandler : NetMessageHandler
{
    /// <summary>
    /// Handle message.
    /// </summary>
    /// <param name="reader">Reader of parameters</param>
    protected override void OnHandleMessage(NetDataReader reader)
    {
        //PlayerData playerData = MessageSerializerService.DeserializeObjectOfType<PlayerData>(reader.GetString());

        //PlayerController newPlayer = ((GameObject)Instantiate(Resources.Load("Objects/Player"),
        //    new Vector3(playerData.PositionData.X, playerData.PositionData.Y, playerData.PositionData.Z),
        //    Quaternion.identity)).GetComponent<PlayerController>();

        //NetObject netObject = newPlayer.gameObject.GetComponent<NetObject>();
        //netObject.Id = playerData.Id;
        //netObject.IsMine = true;

        //newPlayer.Health = playerData.Health;
        //newPlayer.MoveSpeed = playerData.MoveSpeed;
        //newPlayer.Damage = playerData.Damage;
        //newPlayer.AttackRadius = playerData.AttackRadius;

        //ClientNetEventListener.Instance.NetObjects.Add(playerData.Id, netObject);

        Debug.LogFormat("WorldEnter handler");
    }
}

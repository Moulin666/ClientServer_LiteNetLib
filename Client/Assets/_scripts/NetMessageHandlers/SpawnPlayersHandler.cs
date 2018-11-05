using LiteNetLib.Utils;
using NetCommon;
using NetCommon.MessageObjects;
using UnityEngine;


public class SpawnPlayersHandler : NetMessageHandler
{
    /// <summary>
    /// Handle message.
    /// </summary>
    /// <param name="reader">Reader of parameters</param>
    protected override void OnHandleMessage(NetDataReader reader)
    {
        int playerCount = reader.GetInt();

        for (int i = 0; i < playerCount; i++)
        {
            var p = reader.GetString();

            Debug.Log($"SpawnPlayer... PlayerInfo: {p}");

            PlayerData playerData = MessageSerializerService.DeserializeObjectOfType<PlayerData>(p);

            PlayerController newPlayer = ((GameObject)Instantiate(Resources.Load("Objects/Player"),
                new Vector3(playerData.PositionData.X, playerData.PositionData.Y, playerData.PositionData.Z),
                Quaternion.identity)).GetComponent<PlayerController>();

            NetObject netObject = newPlayer.gameObject.GetComponent<NetObject>();
            netObject.Id = playerData.Id;

            newPlayer.Health = playerData.Health;
            newPlayer.MoveSpeed = playerData.MoveSpeed;
            newPlayer.Damage = playerData.Damage;
            newPlayer.AttackRadius = playerData.AttackRadius;

            ClientNetEventListener.Instance.NetObjects.Add(playerData.Id, netObject);
        }

        Debug.LogFormat("SpawnPlayers. Count: {0}", playerCount);
    }
}

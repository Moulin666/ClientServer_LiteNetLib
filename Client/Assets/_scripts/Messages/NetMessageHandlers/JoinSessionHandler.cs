using LiteNetLib.Utils;
using NetCommon;
using NetCommon.Codes;
using NetCommon.MessageObjects;
using UnityEngine;


public class JoinSessionHandler : NetMessageHandler
{
    /// <summary>
    /// Handle message.
    /// </summary>
    /// <param name="reader">Reader of parameters</param>
    protected override void OnHandleMessage(NetDataReader reader)
    {
        NetErrorCode errorCode = (NetErrorCode)reader.GetByte();

        if (errorCode == NetErrorCode.SessionConnectedFailed)
        {
            Debug.Log("Session connected failed");
            return;
        }
       
        int unitCount = reader.GetInt();
        for (int i = 0; i < unitCount; i++)
        {
            UnitData unitData = MessageSerializerService.DeserializeObjectOfType<UnitData>(reader.GetString());

            UnitController newUnit = ((GameObject)Instantiate(Resources.Load("Objects/Player"),
                new Vector3(unitData.PositionData.X, unitData.PositionData.Y, unitData.PositionData.Z),
                Quaternion.identity)).GetComponent<UnitController>();

            NetObject netObject = newUnit.gameObject.GetComponent<NetObject>();
            netObject.Id = unitData.UnitId;
            netObject.IsMine = true;

            newUnit.Health = unitData.Health;
            newUnit.MoveSpeed = unitData.MoveSpeed;
            newUnit.MinDamage = unitData.MinDamage;
            newUnit.MaxDamage = unitData.MaxDamage;
            newUnit.AttackRadius = unitData.AttackRadius;

            ClientNetEventListener.Instance.NetObjects.Add(unitData.UnitId, netObject);
            BattleManager.Instance.PlayerUnits.Add(i, newUnit);
        }

        BattleManager.Instance.JoinSessionSuccess();
    }
}

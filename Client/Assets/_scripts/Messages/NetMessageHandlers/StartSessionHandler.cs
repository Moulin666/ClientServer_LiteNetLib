using LiteNetLib.Utils;
using NetCommon;
using NetCommon.MessageObjects;
using UnityEngine;


public class StartSessionHandler : NetMessageHandler
{
    protected override void OnHandleMessage(NetDataReader reader)
    {
        int unitCount = reader.GetInt();
        for (int i = 0; i < unitCount; i++)
        {
            UnitData unitData = MessageSerializerService.DeserializeObjectOfType<UnitData>(reader.GetString());

            UnitController newUnit = ((GameObject)Instantiate(Resources.Load("Objects/Player"),
                new Vector3(unitData.PositionData.X, unitData.PositionData.Y, unitData.PositionData.Z),
                Quaternion.identity)).GetComponent<UnitController>();

            NetObject netObject = newUnit.gameObject.GetComponent<NetObject>();
            netObject.Id = unitData.UnitId;

            newUnit.Health = unitData.Health;
            newUnit.MoveSpeed = unitData.MoveSpeed;
            newUnit.MinDamage = unitData.MinDamage;
            newUnit.MaxDamage = unitData.MaxDamage;
            newUnit.AttackRadius = unitData.AttackRadius;

            ClientNetEventListener.Instance.NetObjects.Add(unitData.UnitId, netObject);
            BattleManager.Instance.EnemyUnits.Add(i, newUnit);
        }

        BattleManager.Instance.SessionStarted = true;
    }
}

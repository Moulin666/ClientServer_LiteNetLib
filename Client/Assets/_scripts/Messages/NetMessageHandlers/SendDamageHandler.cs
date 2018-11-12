using LiteNetLib.Utils;
using UnityEngine;


public class SendDamageHandler : NetMessageHandler
{
    protected override void OnHandleMessage(NetDataReader reader)
    {
        var senderId = reader.GetInt();
        var targetId = reader.GetInt();
        var newHealth = reader.GetFloat();

        if (ClientNetEventListener.Instance.NetObjects.ContainsKey(senderId)
            && ClientNetEventListener.Instance.NetObjects.ContainsKey(targetId))
        {
            var sender = ClientNetEventListener.Instance.NetObjects[senderId].GetComponent<UnitController>();
            var target = ClientNetEventListener.Instance.NetObjects[targetId].GetComponent<UnitController>();

            sender.MakeDamage();

            var damage = Mathf.Abs(target.Health - newHealth);
            target.Health = newHealth;

            sender.MakeDamage();
            target.GetDamage(damage);
        }
    }
}

using LiteNetLib.Utils;


public class SendAttackUnitHandler : NetMessageHandler
{
    protected override void OnHandleMessage (NetDataReader reader)
    {
        var senderId = reader.GetInt();
        var targetId = reader.GetInt();

        if (ClientNetEventListener.Instance.NetObjects.ContainsKey(senderId)
            && ClientNetEventListener.Instance.NetObjects.ContainsKey(targetId))
        {
            var sender = ClientNetEventListener.Instance.NetObjects[senderId].GetComponent<UnitController>();
            var target = ClientNetEventListener.Instance.NetObjects[targetId].gameObject;

            sender.AttackUnit(target);
        }
    }
}

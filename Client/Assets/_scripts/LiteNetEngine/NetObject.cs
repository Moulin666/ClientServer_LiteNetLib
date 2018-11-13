using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon;
using NetCommon.Codes;
using NetCommon.MessageObjects;
using System.Collections;
using UnityEngine;


public class NetObject : MonoBehaviour
{
    public int Id;

    public bool IsStatic;

    public bool IsMine;

    private void Start ()
    {
        if (!IsStatic)
            ClientNetEventListener.Instance.OnMove += MoveToPosition;
    }

    public void StartSynchronization ()
    {
        if (IsMine && !IsStatic)
            StartCoroutine("SendMyPosition");
    }

    public void StopSynchronization () => StopCoroutine("SendMyPosition");

    public void MoveToPosition (long id, Vector3 newPosition)
    {
        if (Id == id)
            GetComponent<UnitController>().MoveToNewPosition(newPosition);
    }

    public void SendNewDestination (Vector3 newDestination)
    {
        PositionData positionData = new PositionData(newDestination.x, newDestination.y, newDestination.z);

        NetDataWriter dataWriter = new NetDataWriter();
        dataWriter.Reset();
        dataWriter.Put((byte)NetOperationCode.SendUnit);
        dataWriter.Put(Id);
        dataWriter.Put(MessageSerializerService.SerializeObjectOfType(positionData));

        ClientNetEventListener.Instance.SendOperation(dataWriter, DeliveryMethod.Sequenced);
    }

    private IEnumerator SendMyPosition ()
    {
        yield return new WaitForSeconds(0.1f);

        PositionData positionData = new PositionData(transform.position.x, transform.position.y, transform.position.z);

        NetDataWriter dataWriter = new NetDataWriter();
        dataWriter.Reset();
        dataWriter.Put((byte)NetOperationCode.MoveUnit);
        dataWriter.Put(Id);
        dataWriter.Put(MessageSerializerService.SerializeObjectOfType(positionData));

        ClientNetEventListener.Instance.SendOperation(dataWriter, DeliveryMethod.Sequenced);

        StartCoroutine("SendMyPosition");
    }
}

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

    private Vector3 serverPosition;

    private void Start ()
    {
        serverPosition = transform.position;

        if (!IsMine && !IsStatic)
            ClientNetEventListener.Instance.OnMove += MoveToPosition;
    }

    private void FixedUpdate ()
    {
        if (!IsMine && !IsStatic)
        {
            if (Vector3.Distance(transform.position, serverPosition) < 3f)
                transform.position = Vector3.Lerp(transform.position, serverPosition, 9f * Time.deltaTime);
            else
                transform.position = serverPosition;
        }
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
            serverPosition = newPosition;
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

using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon;
using NetCommon.Codes;
using NetCommon.MessageObjects;
using System.Collections;
using UnityEngine;


public class NetObject : MonoBehaviour
{
    public long Id;

    public bool IsStatic;

    public bool IsMine;

    private Vector3 serverPosition;

    private void Start()
    {
        if (IsMine && !IsStatic)
            StartCoroutine("SendMyPosition");
        else if (!IsMine && !IsStatic)
            ClientNetEventListener.Instance.OnMove += MoveToPosition;
    }

    private void FixedUpdate()
    {
        if (!IsMine && !IsStatic)
        {
            if (Vector3.Distance(transform.position, serverPosition) < 3f)
                transform.position = Vector3.Lerp(transform.position, serverPosition, 9f * Time.deltaTime);
            else
                transform.position = serverPosition;
        }
    }

    public void MoveToPosition(long id, Vector3 newPosition)
    {
        if (Id == id)
            serverPosition = newPosition;
    }

    private IEnumerator SendMyPosition()
    {
        yield return new WaitForSeconds(0.1f);

        PositionData positionData = new PositionData(transform.position.x, transform.position.y, transform.position.z);

        NetDataWriter dataWriter = new NetDataWriter();
        dataWriter.Reset();
        dataWriter.Put((byte)NetOperationCode.MovePlayerCode);
        dataWriter.Put(Id);
        dataWriter.Put(MessageSerializerService.SerializeObjectOfType(positionData));

        ClientNetEventListener.Instance.SendOperation(dataWriter, SendOptions.Sequenced);

        StartCoroutine("SendMyPosition");
    }
}
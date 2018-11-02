using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon;
using NetCommon.Codes;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public PlayerData playerData;

    private NavMeshAgent _agent;

    private Vector3 serverPosition;

	private void Start ()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (playerData.IsMine)
            StartCoroutine("SendMyPosition");
    }

    private void Update()
    {
        if (playerData.IsMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    _agent.destination = hit.point;
            }
        }
        else
        {
            // TODO : Get positin, check new position set, new position.
        }
    }

    private void FixedUpdate()
    {
        if (!playerData.IsMine)
        {
            if (Vector3.Distance(transform.position, serverPosition) < 3f)
                transform.position = Vector3.Lerp(transform.position, serverPosition, 9f * Time.deltaTime);
            else
                transform.position = serverPosition;
        }

        playerData.X = transform.position.x;
        playerData.Y = transform.position.y;
        playerData.Z = transform.position.z;
    }

    public void MoveToPosition(Vector3 newPosition)
    {
        serverPosition = newPosition;
    }

    private IEnumerator SendMyPosition()
    {
        yield return new WaitForSeconds(0.1f);

        NetDataWriter dataWriter = new NetDataWriter();
        dataWriter.Reset();
        dataWriter.Put((byte)NetOperationCode.MovePlayerCode);
        dataWriter.Put(MessageSerializerService.SerializeObjectOfType(playerData));

        ClientNetEventListener.Instance.SendOperation(dataWriter, SendOptions.Sequenced);

        StartCoroutine("SendMyPosition");
    }
}

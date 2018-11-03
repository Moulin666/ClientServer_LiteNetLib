using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon;
using NetCommon.Codes;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class PlayerController : MonoBehaviour
{
    private NetObject _netObject;

    private NavMeshAgent _agent;

	private void Start ()
    {
        _netObject = GetComponent<NetObject>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (_netObject.IsMine)
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
}

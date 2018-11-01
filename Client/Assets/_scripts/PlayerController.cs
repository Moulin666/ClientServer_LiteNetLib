using UnityEngine;
using UnityEngine.AI;


public class PlayerController : MonoBehaviour
{
    public bool isMine = false;

    private NavMeshAgent _agent;

	private void Start ()
    {
        _agent = GetComponent<NavMeshAgent>();
	}

    private void Update()
    {
        if (isMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    _agent.destination = hit.point;
                    // TODO : Send operation to the server about new position.
                }
            }
        }
        else
        {
            // TODO : Get positin, check new position set, new position.
        }
    }
}

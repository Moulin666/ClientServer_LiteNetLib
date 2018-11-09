using UnityEngine;
using UnityEngine.AI;


public class PlayerController : MonoBehaviour
{
    private NetObject _netObject;

    private NavMeshAgent _agent;

    public float Health;

    public float Damage;

    public float MoveSpeed;

    public float AttackRadius;

    private GameObject _target;
    private bool _isAttack = false;

    private void Start ()
    {
        _netObject = GetComponent<NetObject>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.speed = MoveSpeed;

        if (_netObject.IsMine)
            GetComponent<Renderer>().material.color = Color.cyan;
    }

    private void Update ()
    {
        if (_netObject.IsMine)
        {
            InputUpdate();

            if (_isAttack && _target != null)
            { 
                if (Vector3.Distance(transform.position, _target.transform.position) > AttackRadius)
                    _agent.SetDestination(_target.transform.position);
                else
                    _agent.SetDestination(transform.position);
            }
        }
    }

    private void InputUpdate ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_target == null)
                return;

            _isAttack = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (hit.collider.tag == "Player")
                {
                    if (!hit.collider.gameObject.GetComponent<NetObject>().IsMine)
                    {
                        if (_target != null)
                            _target.GetComponent<Renderer>().material.color = Color.green;

                        _target = hit.collider.gameObject;
                        _target.GetComponent<Renderer>().material.color = Color.magenta;

                        _isAttack = true;

                        return;
                    }
                }

                _isAttack = false;
                _agent.SetDestination(hit.point);
            }
        }
    }
}

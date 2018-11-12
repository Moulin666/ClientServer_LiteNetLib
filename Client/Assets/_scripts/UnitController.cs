using LiteNetLib;
using LiteNetLib.Utils;
using NetCommon.Codes;
using UnityEngine;
using UnityEngine.AI;


public class UnitController : MonoBehaviour
{
    private NetObject _netObject;

    private NavMeshAgent _agent;

    public float Health;

    public float MinDamage;

    public float MaxDamage;

    public float MoveSpeed;

    public float AttackRadius;

    public bool IsSelected = false;

    private GameObject _target;

    private bool _isAttack = false;
    private bool _canAttack = true;

    private void Start ()
    {
        _netObject = GetComponent<NetObject>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.speed = MoveSpeed;
    }

    private void Update ()
    {
        if (_netObject.IsMine)
        {
            if (IsSelected)
                InputUpdate();

            if (_isAttack && _target != null)
            {
                if (Vector3.Distance(transform.position, _target.transform.position) > AttackRadius)
                    _agent.SetDestination(_target.transform.position);
                else
                {
                    _agent.SetDestination(transform.position);

                    if (_canAttack)
                    {
                        _canAttack = false;

                        var targetId = _target.GetComponent<NetObject>().Id;

                        NetDataWriter dataWriter = new NetDataWriter();
                        dataWriter.Put((byte)NetOperationCode.SendDamage);
                        dataWriter.Put(_netObject.Id);
                        dataWriter.Put(targetId);

                        ClientNetEventListener.Instance.SendOperation(dataWriter, DeliveryMethod.Sequenced);

                        Invoke("AttackCooldown", 5f);
                    }
                }
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
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (!BattleManager.Instance.SessionStarted)
                {
                    transform.position = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);

                    SetUnitPositionView setUnitPositionView = new SetUnitPositionView();
                    setUnitPositionView.SetPosition(_netObject.Id, transform.position);
                }

                if (hit.collider.tag == "Player")
                {
                    if (!hit.collider.gameObject.GetComponent<NetObject>().IsMine)
                    {
                        _target = hit.collider.gameObject;

                        _isAttack = true;

                        return;
                    }
                }

                _isAttack = false;
                _agent.SetDestination(hit.point);
            }
        }
    }

    private void AttackCooldown () => _canAttack = true;

    public void MakeDamage ()
    {
        // TODO : Perfect damage animation and number of damage from the sky.
    }

    public void GetDamage (float damage)
    {
        Debug.Log("GetDamage: " + damage);
        // TODO : Perfect damage animation and number of damage from the sky.
    }
}

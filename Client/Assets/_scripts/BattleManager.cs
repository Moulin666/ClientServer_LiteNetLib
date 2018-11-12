using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance = null;

    public bool SessionStarted = false;

    public Button JoinSessionButton;

    public Button[] UnitButtons;

    [SerializeField]
    public Dictionary<int, UnitController> PlayerUnits = new Dictionary<int, UnitController>();

    [SerializeField]
    public Dictionary<int, UnitController> EnemyUnits = new Dictionary<int, UnitController>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void OnDestroy() => Instance = null;

    public void JoinSessionSuccess ()
    {
        JoinSessionButton.gameObject.SetActive(false);

        foreach (var b in UnitButtons)
            b.gameObject.SetActive(true);

        Debug.Log("Join session succes");
    }

    public void StartSession ()
    {
        SessionStarted = true;

        foreach (var u in PlayerUnits.Values)
            u.GetComponent<NetObject>().StartSynchronization();
    }

    #region UI button click handlers

    public void OnJoinSessionClick ()
    {
        JoinSessionView joinSessionView = new JoinSessionView();
        joinSessionView.JoinSession();
    }

    public void OnUnitClick (int unitId)
    {
        if (!PlayerUnits.ContainsKey(unitId))
            return;

        foreach (var u in PlayerUnits)
            u.Value.IsSelected = false;

        PlayerUnits[unitId].IsSelected = true;
    }

    #endregion
}
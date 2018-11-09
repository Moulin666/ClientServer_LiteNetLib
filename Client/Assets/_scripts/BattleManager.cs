using UnityEngine;
using UnityEngine.UI;


public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance = null;

    public Button JoinSessionButton;

    public Button[] UnitButtons;

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

        Debug.Log("Success");
    }

    #region UI button click handlers

    public void OnJoinSessionClick ()
    {
        JoinSessionView joinSessionView = new JoinSessionView();
        joinSessionView.JoinSession();
    }

    public void OnUnitClick (int unitId)
    {
        Debug.Log(unitId);
    }

    #endregion
}
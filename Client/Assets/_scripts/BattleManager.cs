using UnityEngine;


public class BattleManager : MonoBehaviour
{
    public void OnJoinSessionClick ()
    {
        JoinSessionView joinSessionView = new JoinSessionView();
        joinSessionView.JoinSession();
    }
}
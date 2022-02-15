using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;


public class UserDataListener : MonoBehaviourPun
{
    public UserData userData;
    
    public UnityEvent<List<int>,List<int>> CardChangeStartEvent;
    public UnityEvent CardChangeEndEvent;
    public UnityEvent CardsetResetEvent;
    public UnityEvent<int, Rank, int, int> BlockSetEvent;
    public UnityEvent<int, int, object> BingoEvent;
    public UnityEvent<int, int, object> UnbingoEvent;
    public UnityEvent<int> StageStartEvent;
    public UnityEvent<GameObject> EnemyDieEvent;
    public UnityEvent<int> GetDamageEvent;
    public UnityEvent<int> UpdateChangeChanceEvent;
    public UnityEvent WinEvent;
    public UnityEvent DefeatEvent;
    private void OnEnable()
    {
        userData = Resources.Load<UserData>("ScriptableObject/UserData");
        userData.AddListener(this);
    }

    private void OnDisable()
    {
        userData.RemoveListener(this);
    }
}

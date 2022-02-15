using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "UserData", menuName = "Scriptable Object/UserData")]
public class UserData : ScriptableObject
{
    
#region Event

    public List<UserDataListener> listeners = new List<UserDataListener>();

    public void AddListener(UserDataListener listener) => listeners.Add(listener);
    public void RemoveListener(UserDataListener listener) => listeners.Remove(listener);

    public void CardChangeStartEvent(List<int> suits, List<int> numbers)
    {
        for (int i = 0; i < listeners.Count; i++) listeners[i].CardChangeStartEvent.Invoke(suits, numbers);
    }
    
    public void CardChangeEndEvent()
    {
        for (int i = 0; i < listeners.Count; i++) listeners[i].CardChangeEndEvent.Invoke();
    }

    public void BlockSetEvent(int photonViewId, Rank rank, int row, int column)
    {
        for (int i = 0; i < listeners.Count; i++) listeners[i].BlockSetEvent.Invoke(photonViewId, rank, row, column);
    }
    
    public void CardsetResetEvent()
    {
        for (int i = 0; i < listeners.Count; i++) listeners[i].CardsetResetEvent.Invoke();
    }

    public void EnemyDieEvent(GameObject enemy)
    {
        for (int i = 0; i < listeners.Count; i++) listeners[i].EnemyDieEvent.Invoke(enemy);
    }

    public void StageStart(int level)
    {
        for (int i = 0; i < listeners.Count; i++) listeners[i].StageStartEvent.Invoke(level);
    }
    
    public void GetDamageEvent(int damage)
    {
        for (int i = 0; i < listeners.Count; i++) listeners[i].GetDamageEvent.Invoke(damage);
    }

    public void UpdateChangeChanceEvent()
    {
        for (int i = 0; i < listeners.Count; i++) listeners[i].UpdateChangeChanceEvent.Invoke(ChangeChance);
    }
    
    public void WinEvent()
    {
        for (int i = 0; i < listeners.Count; i++) listeners[i].WinEvent.Invoke();
    }
    
    public void DefeatEvent()
    {
        for (int i = 0; i < listeners.Count; i++) listeners[i].DefeatEvent.Invoke();
    }
    
#endregion

    [SerializeField]
    private int changeChance;
    public int ChangeChance
    {
        get => changeChance;
        set => changeChance = value;
    }
    
    private void OnEnable()
    {
        ChangeChance = 10;
    }
    
    [SerializeField]
    public Rank handRank;

    public List<int> suits;
    public List<int> numbers;
    public List<int> reset;
    public void CardChange(List<int> suits, List<int> numbers)
    {
        if (ChangeChance > 0)
        {
            ChangeChance -= 1;
            CardChangeStartEvent(suits, numbers);
        } 
        if (suits.Equals(reset) && numbers.Equals(reset)) CardChangeStartEvent(suits, numbers);
    }

    public void ResetCard()
    {
        handRank.top = 0;
        CardsetResetEvent();
    }

    public void SetChangeChance(int chance)
    {
        changeChance = chance;
        UpdateChangeChanceEvent();
    }
    
    public void AddChangeChance(int chance)
    {
        changeChance += chance;
        UpdateChangeChanceEvent();
    }
}

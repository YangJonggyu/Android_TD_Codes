using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RefreshButton : MonoBehaviour
{
    public UserData userData;
    public UserDataListener userDataListener;
    public PhotonView photonView;

    public bool available = true;
    private void OnEnable()
    {
        userData = Resources.Load("ScriptableObject/UserData", typeof(UserData)) as UserData;
        userDataListener = GetComponent<UserDataListener>();
        photonView = GetComponent<PhotonView>();
        
        userDataListener.CardChangeStartEvent.AddListener(RefreshLock);
        userDataListener.CardChangeEndEvent.AddListener(RefreshUnLock);
    }

    public void RefreshLock(List<int> a, List<int> b) => available = false;
    public void RefreshUnLock() => available = true;
    
    private void OnMouseDown()
    {
        if (photonView.IsMine && available == true) userData.CardChange(userData.suits, userData.numbers);
    }

}

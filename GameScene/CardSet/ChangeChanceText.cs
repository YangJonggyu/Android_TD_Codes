using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ChangeChanceText : MonoBehaviourPun, IPunObservable
{
    public UserDataListener userDataListener;
    public UserData userData;
    public TextMeshPro text;
    
    void OnEnable()
    {
        userData = Resources.Load<UserData>("ScriptableObject/UserData");
        userDataListener = GetComponent<UserDataListener>();
        text = GetComponent<TextMeshPro>();
        text.text = userData.ChangeChance.ToString();
        
        if (photonView.IsMine) userDataListener.CardChangeStartEvent.AddListener(ChangeTextEvent);
        if (photonView.IsMine) userDataListener.UpdateChangeChanceEvent.AddListener(ChangeTextEvent);
    }

    void ChangeTextEvent(List<int> suits, List<int> numbers)
    {
        text.text = userData.ChangeChance.ToString();
    }
    
    void ChangeTextEvent(int chance)
    {
        text.text = userData.ChangeChance.ToString();
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(text.text);
        }
        else
        {
            this.text.text = (string)stream.ReceiveNext();

        }
    }
}

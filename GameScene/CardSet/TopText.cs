using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class TopText : MonoBehaviour, IPunObservable
{
    public PhotonView photonView;
    public UserDataListener userDataListener;

    public CardSetScript cardSetScript;
    public TextMeshPro text;
    void OnEnable()
    {
        photonView = GetComponent<PhotonView>();
        userDataListener = GetComponent<UserDataListener>();
        text = GetComponent<TextMeshPro>();
        if (photonView.IsMine)
        {
            userDataListener.CardChangeEndEvent.AddListener(SetText);
            userDataListener.CardsetResetEvent.AddListener(ResetText);
        }
    }
    
    public void SetText() => text.text = cardSetScript.GetRank().TopToString();

    public void ResetText() => text.text = "";

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

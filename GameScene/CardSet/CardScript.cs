using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CardScript : MonoBehaviourPun, IPunObservable
{
    private CardSetScript cardSet;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public CardImage cardImage;
    public UserData userData;

    public UserDataListener userDataListener;


    [Serializable]
    public struct Card
    {
        public enum Shape { None, Spade, Diamond, Heart, Clover};
        public int shape;
        public int number;
    }

    public Card cardData;

    public bool isLocked = false;
    public GameObject Lock;
    
    void OnEnable()
    {
        cardImage = Resources.Load<CardImage>("ScriptableObject/CardImage");
        cardSet = transform.parent.gameObject.GetComponent<CardSetScript>();
        userDataListener = GetComponent<UserDataListener>();
        userData = Resources.Load<UserData>("ScriptableObject/UserData");
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (photonView.IsMine)
        {
            userDataListener.CardChangeStartEvent.AddListener(CardChangeEvent);
            userDataListener.CardsetResetEvent.AddListener(CardResetEvent);
        }
    }



    private void OnMouseDown()
    {
        Lockin();
    }

    public void Lockin()
    {
        if (cardData.shape == 0 && cardData.number == 0) return;
        isLocked = !isLocked;
        photonView.RPC("LockinRpc",RpcTarget.All,isLocked);
    }

    [PunRPC]
    void LockinRpc(bool isLocked)
    {
        Lock.SetActive(isLocked);
    }

    public void CardChangeEvent(List<int> suits, List<int> numbers)
    {
        if (isLocked) return;
        cardData.shape = suits[Random.Range(0, suits.Count)];
        cardData.number = numbers[Random.Range(0, numbers.Count)]; // 카드의 숫자는 A를 14로
        CardChangeMotion();
    }
    
    public void CardResetEvent()
    {
        if (isLocked) Lockin();
        cardData.shape = 0;
        cardData.number = 0;
        CardChangeMotion();
    }
    
    public void CardChangeMotion()
    {
        if (cardData.shape == 0 && cardData.number == 0) animator.SetBool("Opened",false);
        else
        {
            if (animator.GetBool("Opened")) animator.SetBool("Flip", true);
            animator.SetBool("Opened",true);
            
        }
        
    }

    public void HalfFlip()
    {
        spriteRenderer.sprite = cardImage.GetCardImage(cardData.shape, cardData.number);
    }

    public void FlipEnd()
    {
        animator.SetBool("Flip", false);
        if (photonView.IsMine) userData.CardChangeEndEvent();
    } 


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(cardData.shape);
            stream.SendNext(cardData.number);
        }
        else
        {
            // Network player, receive data
            this.cardData.shape = (int)stream.ReceiveNext();
            this.cardData.number = (int)stream.ReceiveNext();
        }
    }
}

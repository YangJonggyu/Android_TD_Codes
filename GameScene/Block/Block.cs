using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.Events;

public class Block : MonoBehaviourPun
{
    public int row;
    public int column;
    
    public CardImage cardImage;
    public UserData userData;
    
    public SpriteRenderer background;
    public SpriteRenderer suit;
    public TextMeshPro rankText;
    public TextMeshPro topText;

    public Sprite[] suits;

    public Rank rank;

    private void OnEnable()
    {
        if (!photonView.IsMine) transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
        userData = Resources.Load<UserData>("ScriptableObject/UserData");
        cardImage = Resources.Load<CardImage>("ScriptableObject/CardImage");
        background = GetComponent<SpriteRenderer>();
    }

    public void SetLocation(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public void SetBlock(Rank rank)
    {
        this.rank = rank;
        photonView.RPC("SetBlockRpc",RpcTarget.All,rank.isFlush,(int)rank.numberRank,rank.top,rank.suit);
        userData.BlockSetEvent(photonView.ViewID, rank, row, column);
    }

    [PunRPC]
    void SetBlockRpc(bool isFlush,int numberRank,int top,int suit)
    {
        this.rank = new Rank(isFlush, (NumberRank)numberRank, top, suit);
        background.color = cardImage.GetSuitColor(rank.suit);
        this.suit.sprite = cardImage.GetSuitImage(rank.suit);
        rankText.text = rank.RankToString();
        topText.text = rank.TopToString();
    }
    

    private void OnMouseDown()
    {
        if (photonView.IsMine && userData.handRank.top != 0)
        {
            SetBlock(userData.handRank);
            userData.ResetCard();
        }
    }


}

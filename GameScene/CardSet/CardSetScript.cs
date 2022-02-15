using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using TMPro;
using Photon.Pun;

public class CardSetScript : MonoBehaviour
{
    public PhotonView photonView;
    
    public UserData userData;
    public UserDataListener userDataListener;
    
    public GameObject[] cards;
    public GameObject refreshButton;
    public TextMeshPro topText;
    public TextMeshPro rankText;
    public TextMeshPro changeChanceText;
    
    public bool isopen;

    private void OnEnable()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine) transform.position = new Vector3(0, -6, 0);
        else transform.position = new Vector3(0, 6, 0);

        userData = Resources.Load<UserData>("ScriptableObject/UserData");
        userDataListener = GetComponent<UserDataListener>();
        if (photonView.IsMine) userDataListener.CardChangeEndEvent.AddListener(SaveRankToUserData);
    }

    public void SaveRankToUserData()
    {
        userData.handRank = GetRank();
    }


    #region public void GetRank

    public List<int> Suits = new List<int>();
    public List<int> Numbers = new List<int>();
    private Dictionary<int, int> NumberCount = new Dictionary<int, int>();

    private NumberRank numberRank;
    private bool isFlush;
    private int top;
    private int suit;
    
    public Rank GetRank()
    {
        Suits.Clear();
        Numbers.Clear();
        for (int i = 0; i < cards.Length; i++)
        {
            Suits.Add(cards[i].GetComponent<CardScript>().cardData.shape);
            Numbers.Add(cards[i].GetComponent<CardScript>().cardData.number);
        }

        suit = 0;
        isFlush = false;
        
        if (Suits.Distinct().Count() == 1) 
        {
            isFlush = true; //플러시
            suit = Suits[0];
        }

        numberRank = NumberRank.NoPair;
        top = Numbers.Max();
        Numbers.Sort();
        
        if (Numbers.Distinct().Count() == 5) //다 다른 숫자일 때
        {
            
            if (Numbers[0] + 4 == Numbers[4]) numberRank = NumberRank.Straight;
            if (Numbers[3] == 5 && Numbers[4] == 14) numberRank = NumberRank.BackStraight;
            if (Numbers[0] == 10 && Numbers[4] == 14) numberRank = NumberRank.RoyalStraight;
            top = Numbers[4];
        }
        
        NumberCount.Clear();
        for (int i = 2; i <= 14; i++) NumberCount[i] = Numbers.Count(n => n == i); //각 숫자의 개수를 세서 NumberCount에 저장
        NumberCount = NumberCount.OrderBy(node => node.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

        if (Numbers.Distinct().Count() == 4)
        {
            numberRank = NumberRank.OnePair;
            top = NumberCount.Keys.Last();
        }
        if (Numbers.Distinct().Count() == 3)
        {
            if (NumberCount.Values.Count(n => n == 2) == 2)
            {
                numberRank = NumberRank.TwoPair;
                top = NumberCount.Keys.ToList()[NumberCount.Keys.ToList().Count - 2] > NumberCount.Keys.Last()
                    ? NumberCount.Keys.ToList()[NumberCount.Keys.ToList().Count - 2]: NumberCount.Keys.Last();
            }

            if (NumberCount.Values.Count(n => n == 3) == 1)
            {
                numberRank = NumberRank.ThreeCard;
                top = NumberCount.Keys.Last();
            }
        }
        if (Numbers.Distinct().Count() == 2)
        {
            if (NumberCount.Values.Count(n => n == 4) == 1) numberRank = NumberRank.FourCard;
            if (NumberCount.Values.Count(n => n == 3) == 1) numberRank = NumberRank.FullHouse;
            top = NumberCount.Keys.Last();
        }
        
        if (Numbers.Distinct().Count() == 1)
        {
            numberRank = NumberRank.FiveCard;
            top = NumberCount.Keys.Last();
        }
        
        //출력
        return new Rank(isFlush, numberRank, top, suit);
    }

    #endregion
    
    
}

public enum NumberRank { Straight, BackStraight, RoyalStraight, FullHouse, FiveCard, FourCard, ThreeCard, TwoPair, OnePair, NoPair }

[Serializable]
public struct Rank
{
    public bool isFlush;
    public NumberRank numberRank;
    public int top;
    public int suit;

    public Rank(bool isFlush, NumberRank numberRank, int top, int suit)
    {
        this.isFlush = isFlush;
        this.numberRank = numberRank;
        this.top = top;
        this.suit = suit;
    }

    public string RankToString()
    {
        if (isFlush && numberRank == NumberRank.FiveCard) return "5CF";
        if (isFlush && numberRank == NumberRank.RoyalStraight) return "RSF";
        if (isFlush && numberRank == NumberRank.BackStraight) return "BSF";
        if (isFlush && numberRank == NumberRank.Straight) return "SF";
        if (isFlush && numberRank == NumberRank.FullHouse) return "FHF";
        if (numberRank == NumberRank.FiveCard) return "5C";
        if (numberRank == NumberRank.RoyalStraight) return "RS";
        if (numberRank == NumberRank.BackStraight) return "BS";
        if (numberRank == NumberRank.FourCard) return "4C";
        if (numberRank == NumberRank.Straight) return "S";
        if (isFlush) return "F";
        if (numberRank == NumberRank.FullHouse) return "FH";
        if (numberRank == NumberRank.ThreeCard) return "3C";
        if (numberRank == NumberRank.TwoPair) return "2P";
        if (numberRank == NumberRank.OnePair) return "1P";
        return "T";
    }

    public string TopToString()
    {
        if (top == 14) return "A";
        if (top == 13) return "K";
        if (top == 12) return "Q";
        if (top == 11) return "J";
        return top.ToString();
    }

    public int GetAttack()
    {
        return 0;
    }
}
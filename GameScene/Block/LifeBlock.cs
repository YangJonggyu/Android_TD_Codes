using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class LifeBlock : MonoBehaviourPun, IPunObservable
{
    public UserDataListener userDataListener;
    public CanvasScript canvas;
    
    public GameObject hpGauge;
    private SpriteRenderer hpGaugeSprite;
    public TextMeshPro hpText;

    public int maxHp = 10000;
    public int hp = 10000;

    private void OnEnable()
    {
        userDataListener = GetComponent<UserDataListener>();
        if (photonView.IsMine) userDataListener.GetDamageEvent.AddListener(Damage);
        canvas = GameObject.FindWithTag("Canvas").GetComponent<CanvasScript>();
        maxHp = 1000;
        hp = 1000;
        hpText.text = hp.ToString();
        hpGaugeSprite = hpGauge.GetComponent<SpriteRenderer>();
        if (photonView.IsMine)
        {
            gameObject.transform.position = new Vector3(3.5f, 0, -1);
            hpGaugeSprite.color = new Color32(50,148,126,255);
        }
        else
        {
            gameObject.transform.position = new Vector3(-3.5f, 0, -1);
            hpGaugeSprite.color = new Color32(147,50,50,255);
        }
    }

    private void Update()
    {
        hpGauge.transform.localPosition = new Vector3(0, (float) hp / (float) maxHp * 0.9f - 0.9f, -1);
        if (hp <= 0)
        {
            if (photonView.IsMine) Defeat();
            else Win();
        }
    }

    public void Damage(int damage)
    {
        hp -= damage;
        hpText.text = hp.ToString();
        hpGauge.transform.localPosition = new Vector3(0,(float)hp / (float)maxHp * 0.9f - 0.9f,-1);
    }

    public void Win()
    {
        if(!canvas.Defeat.activeSelf) canvas.Win.SetActive(true);
    }
    
    public void Defeat()
    {
        if(!canvas.Win.activeSelf) canvas.Defeat.SetActive(true);
    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
            stream.SendNext(hpText.text);
            stream.SendNext(hpGauge.transform.localPosition);

        }
        else
        {
            this.hp = (int)stream.ReceiveNext();
            hpText.text = (string)stream.ReceiveNext();
            hpGauge.transform.localPosition = (Vector3)stream.ReceiveNext();
            
        }
    }
}

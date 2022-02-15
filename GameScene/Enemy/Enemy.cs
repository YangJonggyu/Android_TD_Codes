using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Timeline;
using TMPro;
using UnityEngine.Events;

public class Enemy : MonoBehaviourPun, IPunObservable, IPunOwnershipCallbacks
{
    public GameManager gameManager;
    public UserData userData;

    public EnemyLine line;

    public TextMeshPro hpText;

    private int _health;

    public bool isDying;

    public int health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0 && !isDying)
            {
                isDying = true;
                if (photonView.IsMine) Die();
            }
        }
    }
    public int shield = 0;
    public int defence = 100;
    
    public float position = 0;
    public int speed = 100;

    public int damage;

    public bool ReadyToTransfer = false;
    

    private void OnEnable()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        userData = Resources.Load<UserData>("ScriptableObject/UserData");
        line = GameObject.FindGameObjectWithTag("Line").GetComponent<EnemyLine>();
        damage = 100;
        position = 0;
        isDying = false;
        ReadyToTransfer = false;
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SetEnemyData(EnemyData enemyData)
    {
        this.health = enemyData.health;
        this.shield = enemyData.shield;
        this.defence = enemyData.defence;
        this.speed = enemyData.speed;
        position = 0;
        gameObject.SetActive(true);
    }
    
    private void Update()
    {
        hpText.text = health.ToString();
        
        position = Mathf.Clamp01(position + Time.deltaTime / 12 * speed / 100f);
        if (!photonView.IsMine)
        {
            if (position > 0.1f && position < 0.9f && !ReadyToTransfer) ReadyToTransfer = true;
            else if(Mathf.Approximately(position, 1) && ReadyToTransfer)
            {
                ReadyToTransfer = false;
                photonView.RequestOwnership();
            }
        }
        
        
        if (photonView.IsMine) transform.position = line.GetPosition(position);
        else transform.position = new Vector3(-line.GetPosition(position).x, -line.GetPosition(position).y ,0);
        
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (gameObject != targetView.gameObject) return;
        if (targetView.IsMine) photonView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (gameObject != targetView.gameObject) return;
        if (previousOwner.UserId == PhotonNetwork.LocalPlayer.UserId) userData.GetDamageEvent(damage);
        if (targetView.IsMine) gameManager.myEnemys.Add(gameObject);
        else gameManager.myEnemys.Remove(gameObject);
        position = 0;
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        if (gameObject != targetView.gameObject) return;
        if (PhotonNetwork.LocalPlayer.UserId == senderOfFailedRequest.UserId) photonView.RequestOwnership();
    }

    public void Die()
    {
        userData.AddChangeChance(1);
        PhotonNetwork.Destroy(gameObject);
        userData.EnemyDieEvent(gameObject);
    }

    public void Damage(int damage)
    {
        if(photonView.IsMine)health -= damage;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(health);
            stream.SendNext(shield);
            stream.SendNext(defence);
            stream.SendNext(position);
            stream.SendNext(speed);
        }
        else
        {
            // Network player, receive data
            this.health = (int)stream.ReceiveNext();
            this.shield = (int)stream.ReceiveNext();
            this.defence = (int)stream.ReceiveNext();
            var a = (float) stream.ReceiveNext();
            this.position = Mathf.Approximately(a, 1) ? position : a + Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp))/ 12f * speed / 100f;
            this.speed = (int)stream.ReceiveNext();

        }
    }


    
}

public struct EnemyData
{
    public int health;
    public int shield;
    public int defence;
    public int speed;

    public EnemyData(int health, int shield, int defence, int speed)
    {
        this.health = health;
        this.shield = shield;
        this.defence = defence;
        this.speed = speed;
    }
}
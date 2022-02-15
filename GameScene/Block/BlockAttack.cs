using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class BlockAttack : MonoBehaviourPun, IPunObservable
{
    public UserDataListener userDataListener;
    public GameManager gameManager;
    public ServerData serverData;

    public bool isSet = false;
    public int damage;

    //test
    public GameObject enemy;
    private void OnEnable()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        serverData = Resources.Load<ServerData>("ScriptableObject/ServerData");
        userDataListener = GetComponent<UserDataListener>();
        if(photonView.IsMine) userDataListener.BlockSetEvent.AddListener(SetBlock);
    }

    public void SetBlock(int photonViewId, Rank rank, int row, int column)
    {
        if (photonViewId != photonView.ViewID) return;
        isSet = true;
        damage = serverData.damages[rank.RankToString()];
        StartCoroutine(Attacking());
    }

    public void Attack(Transform target)
    {
        PhotonNetwork.Instantiate("Prefabs/Bullet", transform.position, quaternion.identity).GetComponent<Bullet>().Fire(transform, target, damage);
    }


    IEnumerator Attacking()
    {
        while (true)
        {
            enemy = gameManager.FindTargetEnemy();
            if (enemy) Attack(enemy.transform);
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(damage);
        }
        else
        {
            this.damage = (int)stream.ReceiveNext();

        }
    }
    
}

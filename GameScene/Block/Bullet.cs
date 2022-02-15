using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviourPun, IPunObservable
{
    public GameManager gameManager;
    public UserDataListener userDataListener;
    
    public Transform startPoint;
    public Transform target;
    public GameObject bulletImage;
    
    public float speed = 1;
    public float position = 0;

    public int damage;

    public bool isDying = false;
    public bool isHit = false;

    private void OnEnable()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        userDataListener = GetComponent<UserDataListener>();
        isDying = false;
        isHit = false;
        position = 0;
    }

    public void Fire(Transform startPoint, Transform target, int damage)
    {
        this.startPoint = startPoint;
        this.target = target;
        if (!target.gameObject.activeSelf) gameManager.myEnemys.Remove(target.gameObject);
        this.damage = damage;
        bulletImage.SetActive(true);
        photonView.RPC("FireRpc",RpcTarget.Others,startPoint.gameObject.GetPhotonView().ViewID,target.gameObject.GetPhotonView().ViewID,damage);
    }

    [PunRPC]
    public void FireRpc(int startPointPhotonViewId, int targetPhotonViewId, int damage)
    {
        startPoint = PhotonNetwork.GetPhotonView(startPointPhotonViewId).transform;
        try
        {
            target = PhotonNetwork.GetPhotonView(targetPhotonViewId).transform;
        }
        catch
        {
            return;
        }
        this.damage = damage;
        bulletImage.SetActive(true);
    }

    public void Hit()
    {
        isHit = true;
        target.gameObject.GetComponent<Enemy>().Damage(damage);
        PhotonNetwork.Destroy(gameObject);
        isDying = true;
    }

    public void EnemyDie()
    {
        isDying = true;
        PhotonNetwork.Destroy(gameObject);
    }


    private float len;
    void Update()
    {
        if (isDying||isHit) return;
        if ((target == null || !target.GetComponent<PhotonView>().IsMine) && photonView.IsMine)
        {
            EnemyDie();
            return;
        }
        len = Vector3.Distance(startPoint.position, target.position);
        position += Time.deltaTime * 15 / len;
        position = Mathf.Clamp01(position);
        transform.position = Vector3.Lerp(startPoint.position, target.position, position);
        if (photonView.IsMine && Mathf.Approximately(position,1) && !isHit) Hit();

    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(position);
        }
        else
        {
            this.position = (float) stream.ReceiveNext() + (float)(PhotonNetwork.Time - info.timestamp) * 15 / len;
            position = Mathf.Clamp01(position);
        }
    }
}

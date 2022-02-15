using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPun
{
    [SerializeField]
    public int stage;

    public bool stageChanging;

    public GameObject hpGauge;

    public Vector3 spawnPosition = new Vector3(-3.5f,0,0);
    
    public List<GameObject> myEnemys = new List<GameObject>();

    public ServerData serverData;
    public UserData userData;
    public UserDataListener userDataListener;

    private void OnEnable()
    {
        stageChanging = true;
        userData = Resources.Load<UserData>("ScriptableObject/UserData");
        userDataListener = GetComponent<UserDataListener>();
        serverData = Resources.Load<ServerData>("ScriptableObject/ServerData");
        serverData.LoadData();
        userData.SetChangeChance(10);

        stage = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Prefabs/Stage", Vector3.zero, Quaternion.identity);


            StartNextStage();
            
        }
        
        PhotonNetwork.Instantiate("Prefabs/CardSet", Vector3.zero, Quaternion.identity);
        hpGauge = PhotonNetwork.Instantiate("Prefabs/LifeBlock", Vector3.zero, Quaternion.identity);
        
        userDataListener.EnemyDieEvent.AddListener(RemoveEnemy);

        StartCoroutine(SpawnEnemyCoroutine());
        StartCoroutine(CheckStageFinishCoroutine());

    }

    public void RemoveEnemy(GameObject enemy)
    {
        myEnemys.Remove(enemy);
    }

    public IEnumerator CheckStageFinishCoroutine()
    {
        while (true)
        {
            if (spawnQueue.Count == 0 && myEnemys.Count == 0 && !stageChanging)
            {
                photonView.RPC("StartNextStageRpc", RpcTarget.All, PhotonNetwork.Time);
            }            
            yield return new WaitForSeconds(1);
        }
        
    }
    
    public void StartNextStage()
    {
        StartCoroutine(WaitForLoading());
    }
    
    public IEnumerator WaitForLoading()
    {
        yield return new WaitForSeconds(1);
        photonView.RPC("StartNextStageRpc",RpcTarget.All,PhotonNetwork.Time);
    }
    
    [PunRPC]
    public void StartNextStageRpc(double timestamp)
    {
        stageChanging = true;
        StartCoroutine(StartNextStageCoroutine(timestamp));
        
    }
    
    public IEnumerator StartNextStageCoroutine(double timestamp)
    {
        Debug.Log(2f - PhotonNetwork.Time + timestamp);
        yield return new WaitForSeconds((float)(2f - PhotonNetwork.Time + timestamp));
        stage += 1;
        var enemyData = new EnemyData(200 * stage,0,100,100);
        for (int i = 0; i < stage; i++) spawnQueue.Enqueue(enemyData);
        userData.StageStart(stage);
        stageChanging = false;
    }

    public GameObject FindTargetEnemy()
    {
        if (myEnemys.Count == 0) return null;
        float position = 0;
        GameObject firstEnemy = null;
        foreach (var enemy in myEnemys)
        {
            float enemyPosition = enemy.GetComponent<Enemy>().position;
            if (position < enemyPosition && enemyPosition < 0.95)
            {
                position = enemyPosition;
                firstEnemy = enemy;
            }
        }

        return firstEnemy;
    }


    public Queue<EnemyData> spawnQueue = new Queue<EnemyData>();
    public void EnQueueEnemy(int health, int shield, int defence, int speed)
    {
        EnemyData enemyData = new EnemyData(health,shield,defence,speed);
        spawnQueue.Enqueue(enemyData);
    }
    

    IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            if (spawnQueue.Count != 0)
            {
                var enemy = PhotonNetwork.Instantiate("Prefabs/Enemy", spawnPosition, Quaternion.identity);
                enemy.SetActive(false);
                myEnemys.Add(enemy);
                enemy.GetComponent<Enemy>().SetEnemyData(spawnQueue.Dequeue());
                yield return new WaitForSeconds(1.0f);
            }
            else yield return null;
        }
    }

    public void LeaveRoomTest()
    {
        PhotonNetwork.LoadLevel("Main Scene");
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Defeat : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(ExitGame());
    }

    public IEnumerator ExitGame()
    {
        Debug.Log("Exit defeat");
        yield return new WaitForSeconds(2);
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("Main Scene");
    }
}

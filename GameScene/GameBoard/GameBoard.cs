using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameBoard : MonoBehaviour
{
    public Vector3[] blockPositions = new Vector3[]
    {
        new Vector3(-2.5f, -0.5f, -0.1f),
        new Vector3(-1.5f, -0.5f, -0.1f),
        new Vector3(-0.5f, -0.5f, -0.1f),
        new Vector3(0.5f, -0.5f, -0.1f),
        new Vector3(1.5f, -0.5f, -0.1f),
        new Vector3(-2.5f, -1.5f, -0.1f),
        new Vector3(-1.5f, -1.5f, -0.1f),
        new Vector3(-0.5f, -1.5f, -0.1f),
        new Vector3(0.5f, -1.5f, -0.1f),
        new Vector3(1.5f, -1.5f, -0.1f),
        new Vector3(-2.5f, -2.5f, -0.1f),
        new Vector3(-1.5f, -2.5f, -0.1f),
        new Vector3(-0.5f, -2.5f, -0.1f),
        new Vector3(0.5f, -2.5f, -0.1f),
        new Vector3(1.5f, -2.5f, -0.1f)
    };
    
    public Vector3[] bingoPositions = new Vector3[]
    {
        new Vector3(-2.5f, -3.5f, -0.1f),
        new Vector3(-1.5f, -3.5f, -0.1f),
        new Vector3(-0.5f, -3.5f, -0.1f),
        new Vector3(0.5f, -3.5f, -0.1f),
        new Vector3(1.5f, -3.5f, -0.1f),
        
        new Vector3(2.5f, -0.5f, -0.1f),
        new Vector3(2.5f, -1.5f, -0.1f),
        new Vector3(2.5f, -2.5f, -0.1f)
    };

    public GameObject[][] blocks;
    
    
    private void OnEnable()
    {
        for (var i = 1; i <= 15; i++)
        {
            var block = PhotonNetwork.Instantiate("Prefabs/Block", blockPositions[i-1], Quaternion.identity);
            block.transform.parent = gameObject.transform;
            block.GetComponent<Block>().SetLocation((i - 1)/5 + 1 , (i - 1)%5 + 1);
        }
        
        
        for (var i = 1; i <= 5; i++)
        {
            var block = PhotonNetwork.Instantiate("Prefabs/BingoBlock", bingoPositions[i-1], Quaternion.identity);
            block.transform.parent = gameObject.transform;
            block.GetComponent<BingoBlock>().SetLocation(0 , i);
        }
        
        for (var i = 1; i <= 3; i++)
        {
            var block = PhotonNetwork.Instantiate("Prefabs/BingoBlock", bingoPositions[i-1+5], Quaternion.identity);
            block.transform.parent = gameObject.transform;
            block.GetComponent<BingoBlock>().SetLocation(i , 0);
        }
    }
}

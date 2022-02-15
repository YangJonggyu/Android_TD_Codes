using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class StageNumber : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI number;
    public UserDataListener userDataListener;

    private void OnEnable()
    {
        panel.SetActive(false);
        userDataListener = GetComponent<UserDataListener>();
        userDataListener.StageStartEvent.AddListener(SetNumber);
        transform.SetParent(GameObject.FindWithTag("Canvas").transform);
        transform.localPosition = Vector3.zero;
    }

    public void SetNumber(int number)
    {
        panel.SetActive(true);
        this.number.text = number.ToString();
        
        StopCoroutine(SetNumberCo());
        StartCoroutine(SetNumberCo());
    }
    
    public IEnumerator SetNumberCo()
    {
        yield return new WaitForSeconds(2);
        panel.SetActive(false);
    }


}

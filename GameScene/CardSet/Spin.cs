using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spin : MonoBehaviour
{
    public PhotonView photonView;
    
    public UserDataListener userDataListener;
    public Animator animator;
    void OnEnable()
    {
        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        userDataListener = GetComponent<UserDataListener>();
        if (photonView.IsMine) userDataListener.CardChangeStartEvent.AddListener(SpinAni);
    }
    
    void SpinAni(List<int> suits, List<int> numbers) => animator.SetBool("Spin", true);

    void EndSpin() => animator.SetBool("Spin", false);

}

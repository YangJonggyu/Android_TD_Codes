using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class MatchingManager : MonoBehaviourPunCallbacks
{
    public GameObject beforeLoading;
    public GameObject matchingPopUp;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.IsConnected) return;
        PhotonNetwork.AutomaticallySyncScene = true;

        
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
        PhotonNetwork.AuthValues.UserId = AuthManager.User.UserId;
        
        Debug.Log("포톤 아이디 : " + PhotonNetwork.AuthValues.UserId);
        PhotonNetwork.LocalPlayer.NickName = AuthManager.User.DisplayName;
        Debug.Log("포톤 닉네임 : " + PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.ConnectUsingSettings();

    }
    

    public void PushMatchingButton()
    {
        matchingPopUp.SetActive(true);
        Debug.Log("matching start");
        var roomOptions = new ExitGames.Client.Photon.Hashtable(){};
        PhotonNetwork.JoinRandomRoom(roomOptions,2);
    }

    public void CancelMatching()
    {
        matchingPopUp.SetActive(false);
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
    }

    public override void OnConnectedToMaster()
    {
        beforeLoading.SetActive(false);
        Debug.Log("connect to master server");
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.CleanupCacheOnLeave = false;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        beforeLoading.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("create new room :" + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("join room :" + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("new player entered : " + PhotonNetwork.PlayerListOthers[0].UserId);
            PhotonNetwork.LoadLevel("Game Scene");
        }
        
    }


}

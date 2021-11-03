using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public string email;
    public GameObject playerobj;
    public GameObject waitPanel;
    public GameObject canclePanel;
    public Text waittext;
    public int playercount;

    public void Connect() => PhotonNetwork.ConnectUsingSettings();
    // PhotonNetwork.ConnectUsingSettings();//설정한 포톤 서버에 때라 마스터 서버에 연결    
    private void Start()
    {
        waitPanel.SetActive(false);
        playerobj = GameObject.Find("profile");
        email = playerobj.GetComponent<Player_profile>().name_;
        PhotonNetwork.ReconnectAndRejoin();
    }
    

    public void Connect_cancle()
    {
        PhotonNetwork.Disconnect();
        waitPanel.SetActive(false);
    }
    private void Update()
    {
        playercount = PhotonNetwork.PlayerList.Length;
        // Debug.Log(playercount);
        OnJoinedRoom();
    }

    public override void OnConnectedToMaster()//마스터서버에 연결시 작동됨
    {
        waitPanel.SetActive(true);
        waittext.text = "방 참가중...";
        PhotonNetwork.LocalPlayer.NickName = email;
        PhotonNetwork.JoinRandomRoom();
    }
        public override void OnJoinRandomFailed(short returnCode, string message)
    {
        waittext.text = "빈 방 없음, 새로운 방 생성...";
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = 3});
    }
        public override void OnJoinedRoom()
    {
        waittext.text ="대기인원"+playercount+"명 /3명";
        if (playercount==3)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("main");
        }
    }
    
    public override void onDisconnected(DisconnectCause cause)
    {
        Debug.Log("실패");
        PhotonNetwork.ConnectUsingSettings();
    }
    
    
}

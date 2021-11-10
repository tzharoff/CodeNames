using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

public class GameLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField tmpNickname;
    [SerializeField] private TMP_Text tmpConnection;
    [SerializeField] private TMP_Text tmpMyTeam;

    [Header("Panels")]
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject ConnectionPanel;
    [SerializeField] private GameObject JoinPanel;
    [SerializeField] private GameObject RoomPanel;

    [Header("Buttons")]
    [SerializeField] private GameObject StartButton;


    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            OpenMainMenuPanel();
        }
        else
        {
            OpenJoinPanel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetTeam1()
    {
        GameManager.instance.MyTeam = Card.TeamType.Team1;
        tmpMyTeam.text = "My Team: 1";
    }

    public void SetTeam2()
    {
        GameManager.instance.MyTeam = Card.TeamType.Team2;
        tmpMyTeam.text = "My Team: 2";
    }

    public void ConnectToServer()
    {
        if (!string.IsNullOrEmpty(tmpNickname.text))
        {
            PhotonNetwork.LocalPlayer.NickName = tmpNickname.text;
            PhotonNetwork.ConnectUsingSettings();
            tmpConnection.text = "Connecting to server...";
            OpenConnectionPanel();
        }
    }

    public void ConnectToRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        OpenRoomPanel();
        StartButton.SetActive(PhotonNetwork.IsMasterClient);
        Debug.Log($"I'm in {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"Maximum Players: {PhotonNetwork.CurrentRoom.MaxPlayers}");
        Debug.Log($"Current Players: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOps = new RoomOptions();
        roomOps.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(null, roomOps, null);
    }



    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        StartButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnConnectedToMaster()
    {
        tmpConnection.text = "Joining Lobby";
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        tmpConnection.text = "Joined Lobby";
        OpenJoinPanel();
    }

    private void CloseMenus()
    {
        ConnectionPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        JoinPanel.SetActive(false);
        RoomPanel.SetActive(false);
    }

    private void OpenMainMenuPanel()
    {
        CloseMenus();
        MainMenuPanel.SetActive(true);
    }

    private void OpenConnectionPanel()
    {
        CloseMenus();
        ConnectionPanel.SetActive(true);
    }

    private void OpenJoinPanel()
    {
        CloseMenus();
        JoinPanel.SetActive(true);
    }

    private void OpenRoomPanel()
    {
        CloseMenus();
        RoomPanel.SetActive(true);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

}

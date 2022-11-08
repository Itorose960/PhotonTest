using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System;

public class ConnectPhoton : MonoBehaviourPunCallbacks
{
    [Header("Panels")]
    public GameObject welcomePanel;
    public GameObject createRoomPanel;
    public GameObject roomManagerPanel;
    public GameObject createdRoomPanel;
    public GameObject joinRoomPanel;


    [Header("GUI State")]
    public TextMeshProUGUI txtNickname, txtState;

    [Header("GUI Welcome")]
    public TMP_InputField inputNickname;
    public Button btnSubmit;

    [Header("GUI Create Room")]
    public TMP_InputField inputRoomName;
    public TMP_InputField inputMinPlayers;
    public TMP_InputField inputMaxPlayers;

    [Header("GUI Created Room")]
    public TextMeshProUGUI txtRoomDetails, txtPlayerList;
    public GameObject playerListContent;
    public Button playerBtn;

    [Header("GUI Join Room")]
    public TMP_InputField inputJoinRoomName;


    private void Start()
    {
        ChangeCurrentPanel(welcomePanel);
    }

    public void OnClickConnectToServer()
    {
        if(!(string.IsNullOrEmpty(inputNickname.text) || string.IsNullOrWhiteSpace(inputNickname.text)))
        {
            if(!(PhotonNetwork.IsConnected))
            {
                btnSubmit.interactable = false;
                PhotonNetwork.NickName = inputNickname.text;
                PhotonNetwork.ConnectUsingSettings();
                changeState("Connecting...");
            }
            txtNickname.text = inputNickname.text;
            ChangeCurrentPanel(roomManagerPanel);
        } else
        {
            changeState("Nickname not valid");
        }
    }

    public void OnClickCreateRoom()
    {
        int min, max;
        min = int.Parse(inputMinPlayers.text);
        max = int.Parse(inputMaxPlayers.text);

        if (!(string.IsNullOrEmpty(inputRoomName.text) || string.IsNullOrWhiteSpace(inputRoomName.text)))
        {
            if(min > 0 && max >= min)
            {
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = (byte)max;
                roomOptions.IsVisible = true;
                roomOptions.IsOpen = true;
                PhotonNetwork.CreateRoom(inputRoomName.text, roomOptions, TypedLobby.Default);
            }
            else
            {
                changeState("Players limits not valid");
            }
            
        } else
        {
            changeState("Room name not valid");
        }
    }

    public void TryToJoinRoomOfName()
    {
        if(!string.IsNullOrWhiteSpace(inputJoinRoomName.text))
        {
            PhotonNetwork.JoinRoom(inputJoinRoomName.text);
        } else
        {
            changeState("Room Name Not Valid!");
        }
    }

    #region Photon Events

    public override void OnConnected()
    {     
        base.OnConnected();
        changeState("Connected");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        changeState("Room created succesfully");
        ChangeCurrentPanel(createdRoomPanel);
        SetRoomDetails();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        ChangeCurrentPanel(createdRoomPanel);
        SetRoomDetails();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        changeState("Couldn't join room: " + message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        changeState("Couldn't create room: " + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        SetRoomDetails();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        SetRoomDetails();
    }
    #endregion

    private void SetRoomDetails()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        string roomInfo = "Room Information";
        roomInfo += "\n\nName: " + currentRoom.Name;
        roomInfo += "\nPlayers: " + currentRoom.PlayerCount + "/" + currentRoom.MaxPlayers;
        roomInfo += "\nOpen: " + (currentRoom.IsOpen ? "Open" : "Closed");
        txtRoomDetails.text = roomInfo;

        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        if (playerListContent.transform.childCount > 0)
        {
            foreach (Transform transform in playerListContent.transform)
            {
                Destroy(transform.gameObject);
            }
        }
        foreach (Player player in currentRoom.Players.Values)
        {
            
            string playerName = player.NickName;
            string team = "Team";
            Button btn = Instantiate(playerBtn, playerListContent.transform);
            btn.transform.Find("name").GetComponent<TextMeshProUGUI>().text = playerName;
            btn.transform.Find("team").GetComponent<TextMeshProUGUI>().text = "Team";
        }
    }

    public void OnClickGoToCreateRoom()
    {
        ChangeCurrentPanel(createRoomPanel);
    }

    public void OnClickGoToJoinRoom()
    {
        ChangeCurrentPanel(joinRoomPanel);
    }

    public void GoBack(GameObject lastRoom)
    {
        if(PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }
        ChangeCurrentPanel(lastRoom);
    }

    private void ChangeCurrentPanel (GameObject targetPanel)
    {

        welcomePanel.SetActive(false);
        createRoomPanel.SetActive(false);
        roomManagerPanel.SetActive(false);
        createdRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);

        targetPanel.SetActive(true);
    }

    private void changeState(string text)
    {
        txtState.text = text;
    }

}

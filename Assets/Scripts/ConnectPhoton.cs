using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System;
using ExitGames.Client.Photon;

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
    public Button StartGame;

    [Header("GUI Join Room")]
    public TMP_InputField inputJoinRoomName;


    private void Start()
    {
        ChangeCurrentPanel(welcomePanel);
        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(this.gameObject);

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
        } else
        {
            changeState("Nickname not valid");
        }
    }

    public void OnClickCreateRoom()
    {
        int min, max;

        if(!string.IsNullOrWhiteSpace(inputMinPlayers.text) && !string.IsNullOrWhiteSpace(inputMaxPlayers.text))
        {
            min = int.Parse(inputMinPlayers.text);
            max = int.Parse(inputMaxPlayers.text);

            if (!(string.IsNullOrEmpty(inputRoomName.text) || string.IsNullOrWhiteSpace(inputRoomName.text)))
            {
                if (min > 0 && max >= min)
                {
                    RoomOptions roomOptions = new RoomOptions();
                    roomOptions.MaxPlayers = (byte)max;
                    roomOptions.IsVisible = true;
                    roomOptions.IsOpen = true;
                    roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
                    roomOptions.CustomRoomProperties.Add("min", min);
                    PhotonNetwork.CreateRoom(inputRoomName.text, roomOptions, TypedLobby.Default);
                }
                else
                {
                    changeState("Players limits not valid");
                }

            }
            else
            {
                changeState("Room name not valid");
            }
        } else
        {
            changeState("Player limits not defined!");
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
        ChangeCurrentPanel(roomManagerPanel);
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
        
        if(PhotonNetwork.IsMasterClient && currentRoom.PlayerCount >= int.Parse(currentRoom.CustomProperties["min"].ToString()))
        {
            StartGame.gameObject.SetActive(true);
        } else
        {
            StartGame.gameObject.SetActive(false);
        }
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

    public void OnClickStartGame()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
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

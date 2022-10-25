using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class ConnectPhoton : MonoBehaviourPunCallbacks
{
    [Header("Panels")]
    public GameObject welcomePanel;
    public GameObject createRoomPanel;
    public GameObject roomManagerPanel;

    [Header("GUI State")]
    public TextMeshProUGUI txtNickname, txtState;

    [Header("GUI Welcome")]
    public TMP_InputField inputNickname;
    public Button btnSubmit;

    [Header("GUI Create Room")]
    public TMP_InputField inputRoomName;
    public TMP_InputField inputMinPlayers;
    public TMP_InputField inputMaxPlayers;

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
                roomOptions.IsOpen = false;
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
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        changeState("Couldn't create room: " + message);
    }

    #endregion

    public void OnClickGoToCreateRoom()
    {
        ChangeCurrentPanel(createRoomPanel);
    }

    private void ChangeCurrentPanel (GameObject targetPanel)
    {
        welcomePanel.SetActive(false);
        createRoomPanel.SetActive(false);
        roomManagerPanel.SetActive(false);

        targetPanel.SetActive(true);
    }

    private void changeState(string text)
    {
        txtState.text = text;
    }

}

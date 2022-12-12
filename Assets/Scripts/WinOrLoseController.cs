using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinOrLoseController : MonoBehaviourPunCallbacks
{

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] Teams winningTeam;
    [SerializeField] private Button button;
    private void Start()
    {
        if(photonView.IsMine)
        {
            if((Teams)((int)PhotonNetwork.LocalPlayer.CustomProperties["team"]) == winningTeam)
            {
                text.text = "Your team wins!";
            } else
            {
                text.text = "Your team lost...";
            }
        }
    }

    public void GoBack()
    {
        PhotonNetwork.LeaveRoom();
        button.enabled = false;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene("Menu");
    }
}

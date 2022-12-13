using Photon.Pun;
using Photon.Realtime;
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
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void GoBack()
    {
        PhotonNetwork.LeaveRoom();
        button.enabled = false;
        FindObjectOfType<ConnectPhoton>().canvas.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene("Menu");
    }
}

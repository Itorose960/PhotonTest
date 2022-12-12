using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int props = 0;
    private float time = 0;
    void Start()
    {

        if ((Teams)((int)PhotonNetwork.LocalPlayer.CustomProperties["team"]) == Teams.PROP)
        {
            GameObject prop = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-3f, 3f), 8, Random.Range(-3f, 3f)), Quaternion.identity); 
            
        }
        else
        {
            PhotonNetwork.Instantiate("Hunter", new Vector3(Random.Range(61f, 64f), 8, Random.Range(-3f, 3f)), Quaternion.identity);
        }
        Room currentRoom = PhotonNetwork.CurrentRoom;

        foreach (Player player in currentRoom.Players.Values)
        {
            if ((Teams)((int)player.CustomProperties["team"]) == Teams.PROP)
            {
                props++;
            }
        }
    }

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if(PhotonNetwork.IsMasterClient)
        {
            if (props == 0)
            {
                PhotonNetwork.LoadLevel(3);
            }
            else if (time >= 200)
            {
                PhotonNetwork.LoadLevel(2);
            }
        }
    }


}

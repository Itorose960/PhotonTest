using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        if ((Teams) ((int)PhotonNetwork.LocalPlayer.CustomProperties["team"]) == Teams.PROP)
            PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-3f, 3f), 8, Random.Range(-3f, 3f)), Quaternion.identity);
        else
            PhotonNetwork.Instantiate("Hunter", new Vector3(Random.Range(61f, 64f), 8, Random.Range(-3f, 3f)), Quaternion.identity);
    }

    
}

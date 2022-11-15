using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-30f, 30f), 8, Random.Range(-30f, 30f)), Quaternion.identity);
    }

    void Update()
    {
        
    }
}

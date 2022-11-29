using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasicCreatureController : MonoBehaviourPunCallbacks
{
    public float hp = 100;

    private TextMeshProUGUI hpText;

    private void Start()
    {
        hpText = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        hpText.gameObject.SetActive(true);
    }


    private void UpdateText()
    {
        hpText.text = hp.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            hp -= collision.gameObject.GetComponent<BulletController>().damage;
            if(hp < 0)
            {
                hp = 0;
                //TODO: Die, become spectator or end game.
            }      
            UpdateText();

        }
    }
}

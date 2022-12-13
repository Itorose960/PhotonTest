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
       if(photonView.IsMine)
        {
            hpText = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            hpText.gameObject.SetActive(true);
        }
    }


    private void UpdateText()
    {
        hpText.text = hp.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet") && photonView.IsMine)
        {
            hp -= collision.gameObject.GetComponent<BulletController>().damage;
            if(hp <= 0)
            {
                Camera.main.transform.SetParent(GameObject.Find("Hunter(Clone)").transform);
                Camera.main.transform.localPosition = new Vector3(0, 1.61000502f, 0.234768003f);
                Camera.main.transform.rotation = GameObject.Find("Hunter(Clone)").transform.rotation;
                photonView.RPC("updatePropNumber", RpcTarget.All);
                PhotonNetwork.Destroy(this.gameObject);
            }      
            UpdateText();

        }
    }

    [PunRPC]
    private void updatePropNumber()
    {
        FindObjectOfType<GameManager>().props--;
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMeshController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject body;
    [SerializeField] private List<GameObject> options;
    private Transform position;

    RaycastHit hit;

    private void Start()
    {
        position = body.transform;
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            ChangeMesh();
            if (body.transform.localPosition != new Vector3(0, body.transform.localPosition.y, 0))
            {
                body.transform.localPosition = new Vector3(0, body.transform.localPosition.y, 0);
            }
        }
        
    }

    private void ChangeMesh()
    {
        Ray aim = new Ray(transform.GetChild(0).transform.position, transform.GetChild(0).transform.forward);
        if(Physics.Raycast(aim, out hit, Mathf.Infinity, 1 << 3))
        {
            if(hit.collider != null)
            {
                if (hit.distance < 20 && Input.GetMouseButtonDown(0))
                {
                    position = body.transform;
                    photonView.RPC("LoseParent", RpcTarget.All, body.GetComponent<PhotonView>().ViewID);
                    //PhotonNetwork.Destroy(body.GetComponent<PhotonView>());
                    body = PhotonNetwork.Instantiate(GetObject(hit.collider.gameObject.name), position.position, position.rotation);
                    //body.transform.SetParent(transform.parent.transform);
                    
                    body.name = "Body";
                    body.layer = 2;
                    body.tag = "Player";
                    photonView.RPC("SetParent", RpcTarget.All, body.GetComponent<PhotonView>().ViewID, GetComponent<PhotonView>().ViewID);
                    
                }
            }
           
        }
    }

    private string GetObject(string name)
    {
        if (name.EndsWith(')'))
        { // It's a duplicate object
            do
            {
                name = name.Substring(0, name.Length - 1);
            } while (!name.EndsWith('('));
            name = name.Substring(0, name.Length - 2);
        }
        GameObject aux = new GameObject();
        foreach(GameObject obj in options)
        {    
            if (obj.name == name)
                aux = obj;
        }
        return aux.name;
    }

    
}

using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementController : MonoBehaviourPunCallbacks
{
    [Header("Movement")]
    private float speed = 5f;
    [SerializeField] private bool isGrounded = false;
    private bool hasJumped = false;
    [SerializeField] private float jumpForce = 4f;

    [Header("Camera")]
    private float mouseSensitivity = 3f;
    private float minX = -80f;
    private float maxX = 80f;
    private float rotationX;

    private Rigidbody rb;
    private GameObject cam;
    private Animator anim;
    RaycastHit hit;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = this.gameObject.transform.Find("PlayerCamera").gameObject;
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
        } else
        {
            Destroy(cam.gameObject);
            cam = gameObject;
        }
        transform.TryGetComponent<Animator>(out anim);
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            MovePlayer();
            JumpPlayer();
            MoveCamera();
        }
    }

    private void JumpPlayer()
    {
        if(Input.GetButtonDown("Jump") && CheckGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        } 
    }

    private void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;

        Vector3 direction =transform.right * x + transform.forward * z;
        direction.y = rb.velocity.y;
        rb.velocity = direction; 
    }

    private void MoveCamera()
    {
        float y = Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationX += Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX = Mathf.Clamp(rotationX, minX, maxX);

        cam.transform.localRotation = Quaternion.Euler(-rotationX, 0, 0);
        transform.eulerAngles += Vector3.up * y;
    }

    private bool CheckGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            Debug.Log("Name:" + hit.collider.name + "\nDistance: " + (transform.GetChild(transform.childCount - 1).transform.position.y - hit.collider.transform.position.y));
            if((transform.GetChild(transform.childCount - 1).transform.position.y - hit.collider.transform.position.y) < 1f)
            {
                return true;
            }
        }
        return false;
    }

    [PunRPC]
    public void SetParent(int body, int parent)
    {

        PhotonView.Find(body).gameObject.transform.SetParent(PhotonView.Find(parent).gameObject.transform);

    }

    [PunRPC]
    public void LoseParent(int body)
    {

        PhotonView.Find(body).gameObject.transform.SetParent(GameObject.Find("ChildKiller").transform);
    }

    
}

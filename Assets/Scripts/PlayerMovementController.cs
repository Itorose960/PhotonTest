using Photon.Pun;
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
    [SerializeField] private float jumpForce = 10f;

    [Header("Camera")]
    private float mouseSensitivity = 3f;
    private float minX = -80f;
    private float maxX = 80f;
    private float rotationX;

    private Rigidbody rb;
    private Camera cam;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = this.gameObject.transform.Find("PlayerCamera").GetComponent<Camera>();
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
        } else
        {
            Destroy(cam.gameObject);
        }
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
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            hasJumped = true;
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

    private void OnTriggerStay(Collider other)
    {
        
        if(photonView.IsMine)
        {
            if (other.CompareTag("Ground") || other.CompareTag("Interacteable"))
            {
                Debug.Log("IsGrounded + " + isGrounded);
                isGrounded = true;
                hasJumped = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
       if(photonView.IsMine)
        {
           
            if (other.CompareTag("Ground") || other.CompareTag("Interacteable"))
            {
                Debug.Log("Left Trigger");
                if (!hasJumped)
                {
                    StopCoroutine(jumpWindow());
                    StartCoroutine(jumpWindow());
                }
                else
                {
                    isGrounded = false;
                }
            }
        }
    }

    IEnumerator jumpWindow()
    {
        yield return new WaitForSeconds(0.2f);
        isGrounded = false;
    }
}

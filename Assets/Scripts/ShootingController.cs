using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootingController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform leftCannon;
    [SerializeField] private Transform rightCannon;
    [SerializeField] private GameObject bullet;
    private TextMeshProUGUI ammoText;

    private int current_ammo;
    private bool canShoot = true;

    #region constants

    private const int MAX_AMMO = 20;
    private const float SHOOT_SPEED = 0.5f;
    private const float RELOAD_SPEED = 2f;

    #endregion

    private void Start()
    {
        if(photonView.IsMine)
        {
            current_ammo = MAX_AMMO;
            ammoText = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            ammoText.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            if (Input.GetMouseButton(0))
            {
                Shoot();
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                StopCoroutine(ReloadGun());
                StartCoroutine(ReloadGun());
            }
        }
    }

    private void Shoot()
    {
        if(canShoot && current_ammo > 0)
        {
            canShoot = false;
            current_ammo--;
            UpdateAmmo();
            Vector3 aimSpot = Camera.main.transform.position;
            aimSpot += Camera.main.transform.forward * 50.0f;
            photonView.RPC("InstantiateBullet", RpcTarget.All, aimSpot);  
            StartCoroutine(ShootingCooldown());
        }
    }

    private void UpdateAmmo()
    {
        ammoText.text = current_ammo + "/" + MAX_AMMO;
    }

    private IEnumerator ShootingCooldown()
    {
        yield return new WaitForSeconds(SHOOT_SPEED);
        canShoot = true;
    }
    private IEnumerator ReloadGun()
    {
        yield return new WaitForSeconds(RELOAD_SPEED);
        current_ammo = MAX_AMMO;
    }

    [PunRPC]
    public void InstantiateBullet(Vector3 aimSpot)
    {
        Instantiate(bullet, rightCannon.position, rightCannon.rotation).transform.LookAt(aimSpot);
    }
}

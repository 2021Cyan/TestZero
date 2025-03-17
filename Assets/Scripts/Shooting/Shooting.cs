using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;


public class Shooting : MonoBehaviour
{
    [SerializeField] GameObject gunPoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] CinemachineCamera mainCamera;
    [SerializeField] ParticleSystem muzzleFlash_single;
    [SerializeField] ParticleSystem muzzleFlash_smg;

    private Dictionary<string, int> WeaponType = new Dictionary<string, int>()
    {
        { "gun_grip_pistol", 0 },
        { "gun_grip_smg", 1 },
        { "gun_grip_handcannon", 2 }
    };


    private PlayerController playerController;
    private InputManager _input;
    private AudioManager _audio;

    private float nextFireTime = 0f;
    private float currentSpreadAngle = 0f;
    private bool isReloading = false;

    private void Start()
    {
        playerController = PlayerController.Instance;
        _input = InputManager.Instance;
        _audio = AudioManager.Instance;
    }

    private void Update()
    {
        if (!playerController.IsAlive())
        {
            return;
        }

        if ((_input.ClickInput) && playerController.gripType == "gun_grip_smg" && playerController.currentAmmo > 0 && !GunCollideCheck())
        {
            if (!muzzleFlash_smg.isPlaying)
            {
                muzzleFlash_smg.Play();
            }
        }
        else
        {
            muzzleFlash_smg.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }


        bool isFiring = (_input.ClickInput) && Time.time >= nextFireTime && playerController.currentAmmo > 0 && !isReloading;
        if (isFiring)
        {
            Shoot();
            
            nextFireTime = Time.time + 1f / playerController.fireRate;
        }
        else
        {
            // Gradually reduce the spread when not firing
            currentSpreadAngle = Mathf.Max(currentSpreadAngle - playerController.spreadResetSpeed * Time.deltaTime, 0f);
        }

        if ((playerController.currentAmmo <= 0 || _input.ReloadInput) && (playerController.currentAmmo != playerController.maxAmmo))
        {
            Reload();
        }
    }

    private void Shoot()
    {
        //TODO: optimize this later
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(_input.MouseInput);
        mousePos.z = 0f;

        if (GunCollideCheck())
        {
            return;
        }

        CameraScript cameraScript = mainCamera.GetComponent<CameraScript>();
        // Trigger screen shake
        if (cameraScript != null)
        {
            float multiplier = 1f;

            if (playerController.gripType == "gun_grip_smg")
            {
                multiplier = 1.5f;
            }
            else if (playerController.gripType == "gun_grip_pistol")
            {
                multiplier = 2.5f;
            }
            else if (playerController.gripType == "gun_grip_handcannon")
            {
                multiplier = 3.5f;
            }
            cameraScript.StartShake(0.1f * multiplier, 0.5f);
        }

        if (playerController.gripType != "gun_grip_smg")
        {
            muzzleFlash_single.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash_single.Play();
        }

        
        bool shouldFlipBullet = (mousePos.x < gunPoint.transform.position.x && transform.localScale.x > 0) ||
                                (mousePos.x > gunPoint.transform.position.x && transform.localScale.x < 0);

        
        Quaternion originalRotation = gunPoint.transform.rotation;
        float randomSpread = Random.Range(-currentSpreadAngle / 4, currentSpreadAngle);
        gunPoint.transform.Rotate(0, 0, randomSpread);

        Quaternion bulletRotation = gunPoint.transform.rotation;
        if (shouldFlipBullet)
        {
            bulletRotation = Quaternion.Euler(0, 0, bulletRotation.eulerAngles.z + 180f);
        }

        if (playerController.bulletType == 5)
        {
            int pelletCount = 5; 
            float spreadAngle = 40f; 

            for (int i = 0; i < pelletCount; i++)
            {
                float angleOffset = Random.Range(-spreadAngle / 2, spreadAngle / 2);
                Quaternion pelletRotation = bulletRotation * Quaternion.Euler(0, 0, angleOffset);
                Instantiate(bulletPrefab, gunPoint.transform.position, pelletRotation);
            }
        }
        else
        {
            Instantiate(bulletPrefab, gunPoint.transform.position, bulletRotation);
        }

        gunPoint.transform.rotation = originalRotation;
        playerController.currentAmmo--;
        currentSpreadAngle = Mathf.Min(currentSpreadAngle + playerController.spreadIncreaseRate, playerController.maxSpreadAngle);

        _audio.SetParameterByName("WeaponType", GetWeaponType());
        _audio.PlayOneShot(_audio.Shot, transform.position); 
    }

    // Return the current spread angle
    public float GetCurrentSpread()
    {
        return currentSpreadAngle;
    }
    private int GetWeaponType()
    {
        return WeaponType[playerController.gripType];
    }

    public void Reload()
    {
        if (!isReloading)
        {
            _audio.PlayOneShot(_audio.Reload);
            StartCoroutine(ReloadAmmo());
        }
    }

    IEnumerator ReloadAmmo()
    {
        isReloading = true;

        ReticleController reticle = FindFirstObjectByType<ReticleController>();
        
        if (reticle != null)
        {
            reticle.SetReloadUI(true);
        }

        yield return new WaitForSeconds(playerController.reloadSpeed);
        playerController.currentAmmo = playerController.maxAmmo;
        isReloading = false;

        if (reticle != null)
        {
            reticle.SetReloadUI(false);
        }
    }

    public bool getIsReloading()
    {
        return isReloading;
    }

    private bool GunCollideCheck()
    {
        bool gunCollide = false;

        BoxCollider2D gunPointCollider = gunPoint.GetComponent<BoxCollider2D>();
        if (gunPointCollider != null)
        {
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(gunPointCollider.bounds.center, gunPointCollider.bounds.size, 0f);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Terrain"))
                {
                    // Handle the collision with the specific tag
                    gunCollide = true;
                    break;
                }
            }
        }
        return gunCollide;
    }
}

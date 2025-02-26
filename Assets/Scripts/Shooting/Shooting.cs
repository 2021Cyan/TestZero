using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class Shooting : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
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

        if ((_input.AimInput && _input.ClickInput) && playerController.gripType == "gun_grip_smg" && playerController.currentAmmo > 0)
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


        bool isFiring = (_input.AimInput && _input.ClickInput) && Time.time >= nextFireTime && playerController.currentAmmo > 0 && !isReloading;
        if (isFiring)
        {
            Shoot();
            _audio.SetParameterByName("WeaponType", GetWeaponType());
            _audio.PlayOneShot(_audio.Shot);
            nextFireTime = Time.time + 1f / playerController.fireRate;
        }
        else
        {
            // Gradually reduce the spread when not firing
            currentSpreadAngle = Mathf.Max(currentSpreadAngle - playerController.spreadResetSpeed * Time.deltaTime, 0f);
        }

        if (playerController.currentAmmo <= 0 || _input.ReloadInput)
        {
            Reload();
        }
    }

    private void Shoot()
    {
        //TODO: optimize this later

        // Trigger screen shake
        CameraScript cameraScript = Camera.main.GetComponent<CameraScript>();
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


        // Calculate random spread within the current spread angle
        float randomSpread = Random.Range(-currentSpreadAngle / 4, currentSpreadAngle);

        // Adjust firePoint rotation temporarily for the spread
        Quaternion originalRotation = firePoint.rotation;
        firePoint.Rotate(0, 0, randomSpread);

        // Instantiate the bullet with the adjusted rotation
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Restore firePoint to its original rotation
        firePoint.rotation = originalRotation;

        // Reduce ammo
        playerController.currentAmmo--;

        // Increase spread angle
        currentSpreadAngle = Mathf.Min(currentSpreadAngle + playerController.spreadIncreaseRate, playerController.maxSpreadAngle);
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
        yield return new WaitForSeconds(playerController.reloadSpeed);
        playerController.currentAmmo = playerController.maxAmmo;
        isReloading = false;
    }
}

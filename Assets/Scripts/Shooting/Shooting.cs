using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;

    public PlayerController playerController;

    private float nextFireTime = 0f;
    private float currentSpreadAngle = 0f; // Current spread angle

    void Start()
    {
    }

    void Update()
    {
        if (!PlayerController.Instance.IsAlive())
        {
            return;
        }

        if ((Input.GetKey(KeyCode.Mouse0) && Input.GetMouseButton(1)) && Time.time >= nextFireTime && playerController.currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + 1f / playerController.fireRate;
        }
        else
        {
            // Gradually reduce the spread when not firing
            currentSpreadAngle = Mathf.Max(currentSpreadAngle - playerController.spreadResetSpeed * Time.deltaTime, 0f);
        }

        if (playerController.currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            Reload(playerController.maxAmmo);
        }
    }

    void Shoot()
    {
        // Calculate random spread within the current spread angle
        float randomSpread = Random.Range(-currentSpreadAngle/4, currentSpreadAngle);

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

    public void Reload(int amount)
    {
        playerController.currentAmmo += amount;
    }
}

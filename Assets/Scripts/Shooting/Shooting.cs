using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Text ammoText;
    [SerializeField] float fireRate = 10.0f;            // How fast gun can shoot
    [SerializeField] int ammo = 100;                    // Maximum ammo count
    [SerializeField] float maxSpreadAngle = 20f;        // Maximum possible spread angle (-20 deg ~ 20deg from mouse point)
    [SerializeField] float spreadIncreaseRate = 2.5f;   // How fast the spread grows per shot
    [SerializeField] float spreadResetSpeed = 20f;      // How quickly the spread resets when not firing

    private float nextFireTime = 0f;
    private float currentSpreadAngle = 0f; // Current spread angle

    void Start()
    {
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.Mouse0) && Input.GetMouseButton(1)) && Time.time >= nextFireTime && ammo > 0)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
        else
        {
            // Gradually reduce the spread when not firing
            currentSpreadAngle = Mathf.Max(currentSpreadAngle - spreadResetSpeed * Time.deltaTime, 0f);
        }

        if (ammo <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            Reload(100);
        }
    }

    void Shoot()
    {
        // Calculate random spread within the current spread angle
        float randomSpread = Random.Range(-currentSpreadAngle, currentSpreadAngle);

        // Adjust firePoint rotation temporarily for the spread
        Quaternion originalRotation = firePoint.rotation;
        firePoint.Rotate(0, 0, randomSpread);

        // Instantiate the bullet with the adjusted rotation
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Restore firePoint to its original rotation
        firePoint.rotation = originalRotation;

        // Reduce ammo
        ammo--;

        // Increase spread angle
        currentSpreadAngle = Mathf.Min(currentSpreadAngle + spreadIncreaseRate, maxSpreadAngle);
    }

    public float GetCurrentSpread()
    {
        return currentSpreadAngle; // Return the current spread angle
    }

    public void Reload(int amount)
    {
        ammo += amount;
    }

    void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + ammo;
        }
    }
}

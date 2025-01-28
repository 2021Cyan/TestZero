using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float fireRate = 10.0f;
    [SerializeField] int ammo = 100;
    [SerializeField] Text ammoText; 

    private float nextFireTime = 0f;

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
        if (ammo <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            Reload(100);
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        ammo--;
    }

    public void Reload(int amount)
    {
        ammo += amount;
    }
}

using UnityEngine;

public class GunScript : MonoBehaviour
{
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    public Rarity gunRarity;
    public int maxPartLevel;
    public string gripType;
    public int barrelLevel;
    public int frameLevel;
    public int magazineLevel;

    private readonly string[] gripTypes = { "gun_grip_handcannon", "gun_grip_pistol", "gun_grip_smg" };

    public float damage;
    public float fireRate;
    public int maxAmmo;
    public float maxSpreadAngle;
    public float spreadIncreaseRate;
    public float spreadResetSpeed;
    public float reloadSpeed;

    private Transform player;
    private PlayerController playerController;
    private bool isPlayerNearby = false;
    private GunCreate gunCreateStation;

    public void SetGunCreateStation(GunCreate gunCreate)
    {
        gunCreateStation = gunCreate;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            EquipGun();
        }
    }
    public void EquipGun()
    {
        CalculateStats();
        playerController.gripType = gripType;
        playerController.barrelLevel = barrelLevel;
        playerController.frameLevel = frameLevel;
        playerController.magazineLevel = magazineLevel;
        playerController.damage = damage;
        playerController.fireRate = fireRate;
        playerController.maxAmmo = maxAmmo;
        playerController.currentAmmo = maxAmmo;
        playerController.maxSpreadAngle = maxSpreadAngle;
        playerController.spreadIncreaseRate = spreadIncreaseRate;
        playerController.spreadResetSpeed = spreadResetSpeed;
        playerController.reloadSpeed =reloadSpeed;
        if (gunCreateStation != null)
        {
            gunCreateStation.ReleaseSlot(transform.position);
        }
        Destroy(gameObject);
    }

    public void AssignRarityWithPity(int totalGunsCreated)
    {
        if (totalGunsCreated % 50 == 0)
        {
            gunRarity = Rarity.Legendary;
            maxPartLevel = 0;
            return;
        }
        else if (totalGunsCreated % 25 == 0)
        {
            gunRarity = Rarity.Rare;
            maxPartLevel = 10;
            return;
        }
        else if (totalGunsCreated % 10 == 0)
        {
            gunRarity = Rarity.Uncommon;
            maxPartLevel = 6;
            return;
        }
        AssignRarity();
    }

    public void AssignRarity()
    {
        float rng = Random.value * 100;

        if (rng <= 85f)
        {
            gunRarity = Rarity.Common;
            maxPartLevel = 3;
        }
        else if (rng <= 95f)
        {
            gunRarity = Rarity.Uncommon;
            maxPartLevel = 6;
        }
        else if (rng <= 99.4f)
        {
            gunRarity = Rarity.Rare;
            maxPartLevel = 10;
        }
        else
        {
            gunRarity = Rarity.Legendary;
            maxPartLevel = 0;
        }
    }

    public void DistributePartLevels()
    {
        if (gunRarity == Rarity.Legendary)
        {
            return;
        }

        int remainingPoints = maxPartLevel;

        barrelLevel = 1;
        frameLevel = 1;
        magazineLevel = 1;

        while (remainingPoints > 0)
        {
            int part = Random.Range(0, 3);

            switch (part)
            {
                case 0:
                    if (barrelLevel < 5)
                    {
                        barrelLevel++;
                        remainingPoints--;
                    }
                    break;
                case 1:
                    if (frameLevel < 5)
                    {
                        frameLevel++;
                        remainingPoints--;
                    }
                    break;

                case 2:
                    if (magazineLevel < 5)
                    {
                        magazineLevel++;
                        remainingPoints--;
                    }
                    break;
            }
        }
    }

    public void AssignGripType()
    {
        gripType = gripTypes[Random.Range(0, gripTypes.Length)];
    }

    private void SetBaseStats()
    {
        switch (gripType)
        {
            case "gun_grip_handcannon":
                damage = 15.0f;
                fireRate = 2.0f;
                maxAmmo = 6;
                maxSpreadAngle = 20.0f;
                spreadIncreaseRate = 10.0f;
                spreadResetSpeed = 20.0f;
                reloadSpeed = 2.0f;
                break;

            case "gun_grip_pistol":
                damage = 6.0f; 
                fireRate = 4.0f;
                maxAmmo = 12;
                maxSpreadAngle = 30.0f;
                spreadIncreaseRate = 6.0f;
                spreadResetSpeed = 20.0f;
                reloadSpeed = 2.0f;
                break;

            case "gun_grip_smg":
                damage = 3.0f;
                fireRate = 10.0f;
                maxAmmo = 20;
                maxSpreadAngle = 40.0f;
                spreadIncreaseRate = 4.0f;
                spreadResetSpeed = 30.0f;
                reloadSpeed = 2.0f;
                break;
        }
    }
    private void ApplyPartBonuses()
    {
        damage += damage * (barrelLevel - 1) * 0.2f;
        fireRate += fireRate * (barrelLevel - 1) * 0.2f;
        maxAmmo += Mathf.RoundToInt(maxAmmo * (magazineLevel - 1) * 0.2f);
        maxSpreadAngle -= maxSpreadAngle * (frameLevel - 1) * 0.1f;
        spreadIncreaseRate -= spreadIncreaseRate * (frameLevel - 1) * 0.1f;
        spreadResetSpeed += spreadResetSpeed * (frameLevel - 1) * 0.2f;
        reloadSpeed -= reloadSpeed * (magazineLevel - 1) * 0.1f;
    }

    public void CalculateStats()
    {
        SetBaseStats();
        ApplyPartBonuses();
    }
}
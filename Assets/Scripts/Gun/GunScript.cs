using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class LegendaryGunData
{
    // Name of the Gun
    public string gunName;

    // Level of parts and grip Type
    public int frameLevel;
    public int barrelLevel;
    public int magazineLevel;
    public string gripType;

    // Gun stat
    public float damage;
    public float fireRate;
    public int maxAmmo;
    public float maxSpreadAngle;
    public float spreadIncreaseRate;
    public float spreadResetSpeed;
    public float reloadSpeed;
    public int bulletType;
}

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
    public int bulletType;

    private Transform player;
    private PlayerController playerController;
    private InputManager _input;
    private AudioManager _audio;
    private bool isPlayerNearby = false;
    private GunCreate gunCreateStation;
    private GunInfoScript info;
    private GunInfoRender infoRender;
    [SerializeField] Light2D gunLight;

    private LegendaryGunData[] legendaryGuns = new LegendaryGunData[]
    {
        // Add more later...
        new LegendaryGunData() {
            gunName = "Smart Pistol",
            gripType = "gun_grip_pistol",
            barrelLevel = 5,
            frameLevel = 5,
            magazineLevel = 2,
            
            damage = 50f,
            fireRate = 6.0f,
            maxAmmo = 14,
            maxSpreadAngle = 30.0f,
            spreadIncreaseRate = 6.0f,
            spreadResetSpeed = 20.0f,
            reloadSpeed = 2.0f,
            bulletType = 10,
        },new LegendaryGunData() {
            gunName = "Combo",
            gripType = "gun_grip_smg",
            barrelLevel = 5,
            frameLevel = 5,
            magazineLevel = 2,
            damage = 5.0f,
            fireRate = 20.0f,
            maxAmmo = 100,
            maxSpreadAngle = 10.0f,
            spreadIncreaseRate = 2.0f,
            spreadResetSpeed = 30.0f,
            reloadSpeed = 2.0f,
            bulletType = 11,
        },new LegendaryGunData() {
            gunName = "Railgun",
            gripType = "gun_grip_handcannon",
            barrelLevel = 5,
            frameLevel = 5,
            magazineLevel = 1,
            damage = 150f,
            fireRate = 2.0f,
            maxAmmo = 6,
            maxSpreadAngle = 30.0f,
            spreadIncreaseRate = 6.0f,
            spreadResetSpeed = 20.0f,
            reloadSpeed = 2.0f,
            bulletType = 12,
        },
    };

    public void SetInfoPanel(GunInfoScript infoPanel)
    {
        info = infoPanel;
    }

    public void SetInfoRenderer(GunInfoRender infoRenderer)
    {
        infoRender = infoRenderer;
    }
    public void SetGunCreateStation(GunCreate gunCreate)
    {
        gunCreateStation = gunCreate;
    }

    private void OnMouseEnter()
    {
        isPlayerNearby = true;
        info.ShowGunStats(this);
        infoRender.UpdateGunSprites(this);
    }

    private void OnMouseExit()
    {
        isPlayerNearby = false;
        info.HideGunStats();
        infoRender.HideGunSprites();
    }

    private void SetLightColor()
    {
        switch (gunRarity)
        {
            case Rarity.Legendary:
                gunLight.color = new Color(231f / 255f, 76f / 255f, 60f / 255f);
                _audio.SetParameterByName("Rarity", 2);
                _audio.PlayOneShot(_audio.Rarity);
                break;
            case Rarity.Rare:
                gunLight.color = new Color(241f / 255f, 196f / 255f, 15f / 255f);
                _audio.SetParameterByName("Rarity", 1);
                _audio.PlayOneShot(_audio.Rarity);
                break;
            case Rarity.Uncommon:
                gunLight.color = new Color(142f / 255f, 68f / 255f, 173f / 255f);
                _audio.SetParameterByName("Rarity", 0);
                _audio.PlayOneShot(_audio.Rarity);
                break;
            case Rarity.Common:
                gunLight.color = new Color(52f / 255f, 152f / 255f, 219f / 255f);
                break;
        }

    }

    void Start()
    {
        // _audio = AudioManager.Instance;
        // _input = InputManager.Instance;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
    }

    void Awake()
    {
        _audio = AudioManager.Instance;
        _input = InputManager.Instance;
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            _audio.PlayOneShot(_audio.Pickup);
            EquipGun();
        }
    }
    public void EquipGun()
    {
        if (gunRarity != Rarity.Legendary)
        {
            CalculateStats();
        }
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
        playerController.bulletType = bulletType;
        if (gunCreateStation != null)
        {
            gunCreateStation.ReleaseSlot(transform.position);
        }
        info.HideGunStats();
        infoRender.HideGunSprites();
        Destroy(gameObject);
    }

    public void SetLegendaryStats()
    {
        if (legendaryGuns != null && legendaryGuns.Length > 0)
        {
            int index = Random.Range(0, legendaryGuns.Length);
            LegendaryGunData data = legendaryGuns[index];
            barrelLevel = data.barrelLevel;
            frameLevel = data.frameLevel;
            magazineLevel = data.magazineLevel;
            gripType = data.gripType;
            damage = data.damage;
            fireRate = data.fireRate;
            maxAmmo = data.maxAmmo;
            maxSpreadAngle = data.maxSpreadAngle;
            spreadIncreaseRate = data.spreadIncreaseRate;
            spreadResetSpeed = data.spreadResetSpeed;
            reloadSpeed = data.reloadSpeed;
            bulletType = data.bulletType;
        }
    }

    public void AssignRarityWithPity(int totalGunsCreated)
    {
        if (totalGunsCreated % 30 == 0)
        {
            gunRarity = Rarity.Legendary;
            maxPartLevel = 0;
            return;
        }
        else if (totalGunsCreated % 20 == 0)
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
            SetLegendaryStats();
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

        if (gunRarity == Rarity.Uncommon || gunRarity == Rarity.Rare)
        {
            bulletType = Random.Range(1, 6);
        }
        else if (gunRarity == Rarity.Common)
        {
            bulletType = 0;
        }
    }

    public void AssignGripType()
    {
        if(gunRarity == Rarity.Legendary)
        {
            SetLightColor();
            return;
        }
        gripType = gripTypes[Random.Range(0, gripTypes.Length)];
        SetLightColor();
    }

    private void SetBaseStats()
    {
        switch (gripType)
        {
            case "gun_grip_handcannon":
                damage = 50.0f;
                fireRate = 2.0f;
                maxAmmo = 5;
                maxSpreadAngle = 20.0f;
                spreadIncreaseRate = 10.0f;
                spreadResetSpeed = 20.0f;
                reloadSpeed = 2.0f;
                break;

            case "gun_grip_pistol":
                damage = 20.0f; 
                fireRate = 4.0f;
                maxAmmo = 10;
                maxSpreadAngle = 30.0f;
                spreadIncreaseRate = 6.0f;
                spreadResetSpeed = 20.0f;
                reloadSpeed = 2.0f;
                break;

            case "gun_grip_smg":
                damage = 10.0f;
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
        damage += damage * (barrelLevel) * 0.2f;
        fireRate += fireRate * (barrelLevel) * 0.2f;
        maxAmmo += Mathf.RoundToInt(maxAmmo * (magazineLevel) * 0.2f);
        maxSpreadAngle -= maxSpreadAngle * (frameLevel) * 0.1f;
        spreadIncreaseRate -= spreadIncreaseRate * (frameLevel) * 0.1f;
        spreadResetSpeed += spreadResetSpeed * (frameLevel) * 0.2f;
        reloadSpeed -= reloadSpeed * (magazineLevel) * 0.1f;
    }

    public void CalculateStats()
    {
        SetBaseStats();
        ApplyPartBonuses();
    }
}
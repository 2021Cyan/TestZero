using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUI : MonoBehaviour
{

    public static PlayerUI Instance;

    [Header("Ammo UI")]
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Resource UI")]
    [SerializeField] private TextMeshProUGUI resourceText;

    [Header("Health UI")]
    [SerializeField] private Image healthBarFill;

    [Header("Energy UI")]
    [SerializeField] private Image energyBarFill;

    [Header("Screen Filter(BulletTime)")]
    [SerializeField] private Image bulletTimeFilter;

    [Header("Screen Filter(Hurt)")]
    [SerializeField] private Image hurtFilter;

    [Header("Screen Filter(Restore)")]
    [SerializeField] private Image restoreFilter;

    [SerializeField] private Image gripImage;
    [SerializeField] private Image barrelImage;
    [SerializeField] private Image frameImage;

    private PlayerController player;
    private Coroutine hurtCoroutine;
    private Coroutine restoreCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        player = PlayerController.Instance;
        bulletTimeFilter.gameObject.SetActive(false);
        hurtFilter.gameObject.SetActive(false);
        restoreFilter.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }
        //UpdateHP();
        //UpdateImage();
        //UpdateAmmoText();
        //UpdateResourceText();
        //UpdateEnergy();
        UpdateBulletTimeFilter();
    }

    void UpdateAmmoText()
    {
        Shooting shooting = FindFirstObjectByType<Shooting>();

        if (shooting != null && shooting.getIsReloading())
        {
            int dotCount = Mathf.FloorToInt(Time.time * 4) % 3 + 1;
            string dots = new string('.', dotCount);
            ammoText.text = $"RELOADING{dots}";
        }
        else
        {
            ammoText.text = $"{player.currentAmmo} / {player.maxAmmo}";
        }
    }

    void UpdateImage()
    {
        int barrelLevel = player.barrelLevel;
        int frameLevel = player.frameLevel;
        string gripType = player.gripType;

        string barrelPath = $"Sprites/GunParts/Barrels/gun_barrel_lv{barrelLevel}";
        string framePath = $"Sprites/GunParts/Frames/gun_frame_lv{frameLevel}";
        string gripPath = $"Sprites/GunParts/Grips/{gripType}";

        Sprite barrelSprite = Resources.Load<Sprite>(barrelPath);
        barrelImage.sprite = barrelSprite;
        Sprite frameSprite = Resources.Load<Sprite>(framePath);
        frameImage.sprite = frameSprite;
        Sprite gripSprite = Resources.Load<Sprite>(gripPath);
        gripImage.sprite = gripSprite;
        barrelImage.SetNativeSize();
        frameImage.SetNativeSize();
        gripImage.SetNativeSize();
    }

    void UpdateResourceText()
    {
        resourceText.text = $"{player.resource}";
    }

    void UpdateHP()
    {
        float healthPercent = player.hp / player.max_hp;
        healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, healthPercent, Time.deltaTime * 10);
    }

    void UpdateEnergy()
    {
        float energyPercent = player.bulletTimeGauge / player.bulletTimeMaxGauge;
        energyBarFill.fillAmount = Mathf.Lerp(energyBarFill.fillAmount, energyPercent, Time.deltaTime * 10);
    }

    void UpdateBulletTimeFilter()
    {
        if (player.isBulletTimeActive)
        {
            if (!bulletTimeFilter.gameObject.activeSelf)
                bulletTimeFilter.gameObject.SetActive(true);

            Color c = bulletTimeFilter.color;
            c.a = Mathf.Lerp(c.a, 0.05f, Time.deltaTime * 10);
            bulletTimeFilter.color = c;
        }
        else
        {
            Color c = bulletTimeFilter.color;
            c.a = Mathf.Lerp(c.a, 0f, Time.deltaTime * 10);
            bulletTimeFilter.color = c;
            if (c.a < 0.01f && bulletTimeFilter.gameObject.activeSelf)
            {
                bulletTimeFilter.gameObject.SetActive(false);
            }
        }
    }

    public void ShowHurtEffect()
    {
        if (hurtCoroutine != null)
        {
            StopCoroutine(hurtCoroutine);
        }
        hurtCoroutine = StartCoroutine(HurtFlash());
    }

    private IEnumerator HurtFlash()
    {
        hurtFilter.gameObject.SetActive(true);
        Color c = hurtFilter.color;
        float duration = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            c.a = Mathf.Lerp(0.5f, 0f, elapsedTime / duration);
            hurtFilter.color = c;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hurtFilter.gameObject.SetActive(false);
    }

    public void ShowRestoreEffect()
    {
        if (restoreCoroutine != null)
        {
            StopCoroutine(restoreCoroutine);
        }
        restoreCoroutine = StartCoroutine(RestoreFlash());
    }

    private IEnumerator RestoreFlash()
    {
        restoreFilter.gameObject.SetActive(true);
        Color c = restoreFilter.color;
        float duration = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            c.a = Mathf.Lerp(0.5f, 0f, elapsedTime / duration);
            restoreFilter.color = c;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        restoreFilter.gameObject.SetActive(false);
    }
}

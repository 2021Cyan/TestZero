using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    [Header("Ammo UI")]
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Resource UI")]
    [SerializeField] private TextMeshProUGUI resourceText;

    [Header("Health UI")]
    [SerializeField] private Image healthBarFill;

    private PlayerController player;

    void Start()
    {
        player = PlayerController.Instance;
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }
        UpdateHP();
        UpdateAmmoText();
        UpdateResourceText();
    }

    void UpdateAmmoText()
    {
        if (player.currentAmmo == 0)
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

    void UpdateResourceText()
    {
        resourceText.text = $"{player.resource}";
    }

    void UpdateHP()
    {
        float healthPercent = player.hp / player.max_hp;
        healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, healthPercent, Time.deltaTime * 10);
    }
}

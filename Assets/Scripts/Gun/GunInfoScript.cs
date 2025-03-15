using TMPro;
using System.Collections;
using UnityEngine;
using static GunScript;

public class GunInfoScript : MonoBehaviour
{
    private PlayerController playerController;
    private TextMeshPro statText;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string DefaultText = $"<mspace=1.5>Press <color=#00FF00>F</color> to Generate 1 (160)</mspace>" +
                                 $"\n" +
                                 $"\n<mspace=1.5>Hold  <color=#00FF00>F</color> to Generate 10 (1600)</mspace>" +
                                 $"\n" +
                                 $"\n<mspace=1.5>Hold  <color=#00FF00>G</color> to Recycle ALL Guns</mspace>";


    void Start()
    {
        playerController = PlayerController.Instance;
        statText = GetComponent<TextMeshPro>();
        statText.text = DefaultText;
    }

    public void ShowGunStats(GunScript gun)
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }
        string stats = GenerateGunStats(gun);
        typingCoroutine = StartCoroutine(TypeText(stats));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        statText.text = "";

        string tempText = "";
        bool insideTag = false;

        for (int i = 0; i < text.Length; i++)
        {
            char letter = text[i];

            if (letter == '<')
            {
                insideTag = true;
            }

            tempText += letter;

            if (letter == '>')
            {
                insideTag = false;
            }

            if (!insideTag)
            {
                statText.text = tempText;
                yield return new WaitForSeconds(0.00001f); 
            }
        }
        isTyping = false;
    }

    private string GenerateGunStats(GunScript gun)
    {
        string name = gun.gripType switch
        {
            "gun_grip_handcannon" => "HandCannon",
            "gun_grip_pistol" => "Pistol",
            "gun_grip_smg" => "Sub Machine Gun",
            _ => "Unknown Gun"
        };

        string rarityColor = "";
        string rarityText = "";

        switch (gun.gunRarity)
        {
            case GunScript.Rarity.Common:
                rarityColor = "#3498DB"; 
                rarityText = "Standard";
                break;
            case GunScript.Rarity.Uncommon:
                rarityColor = "#8E44AD";  
                rarityText = "Augmented";
                break;
            case GunScript.Rarity.Rare:
                rarityColor = "#F1C40F";  
                rarityText = "Overclocked";
                break;
            case GunScript.Rarity.Legendary:
                rarityColor = "#E74C3C";  
                rarityText = "Prototype";
                break;
        }

        string bulletType = "";

        switch (gun.bulletType)
        {
            case 0:
                bulletType = "";
                break;
            case 1:
                bulletType = $"<color={rarityColor}>RICOCHET</color>";
                break;
            case 2:
                bulletType = $"<color={rarityColor}>PIERCING</color>";
                break;
            case 3:
                bulletType = $"<color={rarityColor}>BIO SIPHON</color>";
                break;
            case 4:
                bulletType = $"<color={rarityColor}>CORROSIVE</color>";
                break;
            case 5:
                bulletType = $"<color={rarityColor}>SCATTER</color>";
                break;
            case 10:
                bulletType = $"<color={rarityColor}>TRACKING</color>";
                break;
            case 11:
                bulletType = $"<color={rarityColor}>CHAIN</color>";
                break;
            case 12:
                bulletType = $"<color={rarityColor}>INFINITE PIERCING</color>";
                break;
            default:
                bulletType = "";
                break;
        }

        if(gun.bulletType == 10)
        {
            name = "Smart Pistol";
        }

        int playerBarrel = 0;
        int playerFrame = 0;
        int playerMagazine = 0;

        if (playerController != null)
        {
            playerBarrel = playerController.barrelLevel;
            playerFrame = playerController.frameLevel;
            playerMagazine = playerController.magazineLevel;
        }
        string barrelDiff = CompareStat(gun.barrelLevel, playerBarrel);
        string frameDiff = CompareStat(gun.frameLevel, playerFrame);
        string magazineDiff = CompareStat(gun.magazineLevel, playerMagazine);


        return $" <color={rarityColor}>{rarityText}</color> {name}\n" +
               $" <mspace=1.5>Firepower    :  LV{gun.barrelLevel}{barrelDiff}</mspace>\n" +
               $" <mspace=1.5>Accuracy     :  LV{gun.frameLevel}{frameDiff}</mspace>\n" +
               $" <mspace=1.5>Ammo Count   :  LV{gun.magazineLevel}{magazineDiff}</mspace>\n" +
               $" <mspace=1.5>{bulletType}</mspace>\n" +
               $" <mspace=1.5>Press <color=#00FF00>E</color> to Equip";

    }


    public void HideGunStats()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }
        statText.text = DefaultText;
    }

    private string CompareStat(int newStat, int currentStat)
    {
        int diff = newStat - currentStat;
        if (diff > 0) return $" <color=#00FF00>(+{diff})</color>";
        if (diff < 0) return $" <color=#FF0000>({diff})</color>";
        return "";
    }
}

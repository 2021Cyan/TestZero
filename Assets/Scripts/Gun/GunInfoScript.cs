using TMPro;
using System.Collections;
using UnityEngine;

public class GunInfoScript : MonoBehaviour
{
    private TextMeshPro statText;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Start()
    {
        statText = GetComponent<TextMeshPro>();
        statText.text = "";
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

        foreach (char letter in text.ToCharArray())
        {
            statText.text += letter;
            yield return new WaitForSeconds(0.01f);
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

        string rarity = "";

        if (gun.gunRarity == GunScript.Rarity.Common)
        {
            rarity = "Standard";
        }else if (gun.gunRarity == GunScript.Rarity.Uncommon)
        {
            rarity = "Augumented";
        }else if (gun.gunRarity == GunScript.Rarity.Rare)
        {
            rarity = "Overclocked";
        }
        else if (gun.gunRarity == GunScript.Rarity.Legendary)
        {
            rarity = "Prototype";
        }

        string bulletType = "";

        switch (gun.bulletType)
        {
            case 0:
                bulletType = "Standard";
                break;
            case 1:
                bulletType = "Ricochet";
                break;
            case 2:
                bulletType = "Piercing";
                break;
            default:
                bulletType = "Standard";
                break;
        }

        return $" \n{rarity} {name}\n\n" +
               $" Barrel:       {("[ " + GetStatBar(gun.barrelLevel) + " ]")}\n\n" +
               $" Frame:      {("[ " + GetStatBar(gun.frameLevel) + " ]")}\n\n" +
               $" Magazine: {("[ " + GetStatBar(gun.magazineLevel) + " ]")}\n\n" +
               $" Bullet Type: {bulletType}\n";
    }

    private string GetStatBar(int level)
    {
        int maxLevel = 5;
        int filledBlocks = level;
        int emptyBlocks = maxLevel - filledBlocks;
        string bar = string.Join(" ", new string('█', filledBlocks).ToCharArray()) +
                     " " +
                     string.Join(" ", new string('▒', emptyBlocks).ToCharArray());
        return bar.Trim();
    }

    public void HideGunStats()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }
        statText.text = "";
    }
}

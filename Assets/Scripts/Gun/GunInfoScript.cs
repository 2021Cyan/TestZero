using TMPro;
using System.Collections;
using UnityEngine;
using static GunScript;

public class GunInfoScript : MonoBehaviour
{
    private TextMeshPro statText;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string DefaultText = $"\nPress F to Generate 1\n" +
                                 $"(160)\n" +
                                 $"\n\nHold F to Generate 10\n" +
                                 $"(1600)\n" +
                                 $"\n\nHold  X to Recycle ALL\n";

    void Start()
    {
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
                bulletType = "Standard";
                break;
            case 1:
                bulletType = "Ricochet";
                break;
            case 2:
                bulletType = "Piercing";
                break;
            case 10:
                bulletType = "Tracking";
                break;
            case 11:
                bulletType = "PIERCING";
                break;
            default:
                bulletType = "Standard";
                break;
        }

        if(bulletType == "Tracking")
        {
            name = "Smart Pistol";
        }

        return $" \n<color={rarityColor}>{rarityText}</color> {name}\n\n" +
               $" Barrel:      {(" [ " + GetStatBar(gun.barrelLevel) + " ]")}\n\n" +
               $" Frame:       {(" [ " + GetStatBar(gun.frameLevel) + " ]")}\n\n" +
               $" Magazine:    {("[ " + GetStatBar(gun.magazineLevel) + " ]")}\n\n" +
               $" Bullet Type: {bulletType}\n\n" +
               $" Press E to Equip";
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
        statText.text = DefaultText;
    }
}

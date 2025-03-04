using TMPro;
using System.Collections;
using UnityEngine;
using static ItemScript;

public class ItemInfoScript : MonoBehaviour
{
    private TextMeshPro statText;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string DefaultText = $"<mspace=1.5>Exchange Data for Upgrades</mspace>";


    void Start()
    {
        statText = GetComponent<TextMeshPro>();
        statText.text = DefaultText;
    }

    public void ShowItemStats(ItemScript item)
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }
        string stats = GenerateItemStats(item);
        typingCoroutine = StartCoroutine(TypeText(stats));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        statText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            statText.text += letter;
            yield return new WaitForSeconds(0.00001f);
        }
        isTyping = false;
    }

    private string GenerateItemStats(ItemScript item)
    {
        string itemName = "";
        string itemEffect = "";
        string colorCode = "#FFFFFF";
        string priceText = $"<mspace=1.5>Cost: {item.price}</mspace>";

        switch (item.itemType)
        {
            case 0:
                itemName = "Nano-Heal";
                itemEffect = "Restores health to <color=#00FF00>full</color>";
                colorCode = "#00FF00"; 
                break;
            case 1:
                itemName = "Bio-Enhancer";  
                itemEffect = "<color=#00FF00>+20</color> maximum health";
                colorCode = "#3498DB"; 
                break;
            case 2:
                itemName = "Neuro Boost";  
                itemEffect = "<color=#00FF00>+1</color> Synaptic Boost duration";
                colorCode = "#3498DB"; 
                break;
            case 3:
                itemName = "Drone Combat Module";  
                itemEffect = "Enhance Pod <color=#00FF00>Attack</color>   capabilities";
                colorCode = "#8E44AD"; 
                break;
            case 4:
                itemName = "Drone Support Module";  
                itemEffect = "Enhances Pod <color=#00FF00>Support</color> capabilities";
                colorCode = "#8E44AD"; 
                break;
            default:
                itemName = "Unknown Item";
                itemEffect = "Effect not recognized.";
                break;
        }

        return $"<color={colorCode}>{itemName}</color>\n" +
               $"<mspace=1.5>{itemEffect}</mspace>\n" +
               $"{priceText}\n" +
               $"<mspace=1.5>Press <color=#00FF00>E</color> to Exchange</mspace>";
    }



    public void HideItemStats()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }
        statText.text = DefaultText;
    }

}

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
}
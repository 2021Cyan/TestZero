using UnityEngine;

public class RandomizeSprites : MonoBehaviour
{
    [SerializeField] private SpriteRenderer gripRenderer;
    [SerializeField] private SpriteRenderer barrelRenderer;
    [SerializeField] private SpriteRenderer frameRenderer;

    private GunScript gunScript;

    void Start()
    {
        gunScript = GetComponent<GunScript>();
        ApplySprites();
    }

    private void ApplySprites()
    {
        int barrelLevel = gunScript.barrelLevel;
        int frameLevel = gunScript.frameLevel;
        string gripType = gunScript.gripType;

        string barrelPath = $"Sprites/GunParts/Barrels/gun_barrel_lv{barrelLevel}";
        string framePath = $"Sprites/GunParts/Frames/gun_frame_lv{frameLevel}";
        string gripPath = $"Sprites/GunParts/Grips/{gripType}";

        Sprite barrelSprite = Resources.Load<Sprite>(barrelPath);
        barrelRenderer.sprite = barrelSprite;
        Sprite frameSprite = Resources.Load<Sprite>(framePath);
        frameRenderer.sprite = frameSprite;
        Sprite gripSprite = Resources.Load<Sprite>(gripPath);
        gripRenderer.sprite = gripSprite;
    }
}

using UnityEngine;

public class RandomizeSprites : MonoBehaviour
{
    [SerializeField] private SpriteRenderer gripRenderer;
    [SerializeField] private SpriteRenderer barrelRenderer;
    [SerializeField] private SpriteRenderer frameRenderer;

    private Sprite[] gripSprites;
    private Sprite[] barrelSprites;
    private Sprite[] frameSprites;

    void Start()
    {
        gripSprites = Resources.LoadAll<Sprite>("Sprites/GunParts/Grips");
        barrelSprites = Resources.LoadAll<Sprite>("Sprites/GunParts/Barrels");
        frameSprites = Resources.LoadAll<Sprite>("Sprites/GunParts/Frames");
    }

    public void RandomizeParts()
    {
        if (gripSprites != null && gripSprites.Length > 0)
        {
            int rng = Random.Range(0, gripSprites.Length);
            gripRenderer.sprite = gripSprites[rng];
        }

        if (barrelSprites != null && barrelSprites.Length > 0)
        {
            int rng = Random.Range(0, barrelSprites.Length);
            barrelRenderer.sprite = barrelSprites[rng];
        }

        if (frameSprites != null && frameSprites.Length > 0)
        {
            int rng = Random.Range(0, frameSprites.Length);
            frameRenderer.sprite = frameSprites[rng];
        }
    }
}

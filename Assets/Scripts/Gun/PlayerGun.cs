using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private SpriteRenderer gripRenderer;
    [SerializeField] private SpriteRenderer barrelRenderer;
    [SerializeField] private SpriteRenderer frameRenderer;

    private Transform player;
    private PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplySprites();
    }

    private void ApplySprites()
    {
        int barrelLevel = playerController.barrelLevel;
        int frameLevel = playerController.frameLevel;
        string gripType = playerController.gripType;

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

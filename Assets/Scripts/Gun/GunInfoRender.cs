using UnityEngine;

public class GunInfoRender : MonoBehaviour
{
    [SerializeField] private SpriteRenderer gripRenderer;
    [SerializeField] private SpriteRenderer barrelRenderer;
    [SerializeField] private SpriteRenderer frameRenderer;
    public void UpdateGunSprites(GunScript gun)
    {
        if (gun == null)
        {
            return;
        }

        string barrelPath = $"Sprites/GunParts/Barrels/gun_barrel_lv{gun.barrelLevel}";
        string framePath = $"Sprites/GunParts/Frames/gun_frame_lv{gun.frameLevel}";
        string gripPath = $"Sprites/GunParts/Grips/{gun.gripType}";

        barrelRenderer.sprite = Resources.Load<Sprite>(barrelPath);
        frameRenderer.sprite = Resources.Load<Sprite>(framePath);
        gripRenderer.sprite = Resources.Load<Sprite>(gripPath);
    }
    public void HideGunSprites()
    {
        gripRenderer.sprite = null;
        barrelRenderer.sprite = null;
        frameRenderer.sprite = null;
    }
}

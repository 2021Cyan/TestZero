using UnityEngine;

public class ItemInfoRender : MonoBehaviour
{
    private SpriteRenderer itemRenderer;
    private void Start()
    {
        itemRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateItemSprites(ItemScript item)
    {
        if (item == null || itemRenderer == null)
        {
            return;
        }
        string itemPath = $"Sprites/Objects/items_{item.itemType}";
        itemRenderer.sprite = Resources.Load<Sprite>(itemPath);
    }
    public void HideGunSprites()
    {
        itemRenderer.sprite = null;
    }
}

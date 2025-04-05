using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public int itemType;
    public int price;

    private Transform player;
    private PlayerController playerController;
    private InputManager _input;
    private AudioManager _audio;
    private bool isPlayerNearby = false;

    private SpriteRenderer spriteRenderer;
    private ItemInfoScript info;
    private ItemInfoRender infoRender;

    public void SetInfoPanel(ItemInfoScript infoPanel)
    {
        info = infoPanel;
    }

    public void SetInfoRenderer(ItemInfoRender infoRenderer)
    {
        infoRender = infoRenderer;
    }

    private void OnMouseEnter()
    {
        if (Time.timeScale == 0) return;
        isPlayerNearby = true;
        info.ShowItemStats(this);
        infoRender.UpdateItemSprites(this);
    }

    private void OnMouseExit()
    {
        if (Time.timeScale == 0) return;
        isPlayerNearby = false;
        info.HideItemStats();
        infoRender.HideGunSprites();
    }

    void Start()
    {
        _audio = AudioManager.Instance;
        _input = InputManager.Instance;
        _input.OnInteractPressed += HandleItemInteract;

        spriteRenderer = GetComponent<SpriteRenderer>();
        string itemPath = $"Sprites/Objects/items_{itemType}";
        spriteRenderer.sprite = Resources.Load<Sprite>(itemPath);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
    }

    private void HandleItemInteract()
    {
        if (isPlayerNearby)
        {
            EquipItem();
        }
    }

    public void EquipItem()
    {
        if(playerController.resource < price){
            return;
        }
        _audio.PlayOneShot(_audio.Pickup);
        playerController.resource -= price;

        switch (itemType)
        {
            case 0:
                playerController.Restore(10000);
                break;
            case 1:
                playerController.max_hp += 20;
                playerController.Restore(10000);
                break;
            case 2:
                playerController.bulletTimeDuration += 1;
                break;
            case 3:
                if (PodScript.Instance != null)
                {
                    PodScript.Instance.weaponlevel += 1;
                }
                break;
            case 4:
                if (PodScript.Instance != null)
                {
                    PodScript.Instance.heallevel += 1;
                }
                break;
        }

        info.HideItemStats();
        infoRender.HideGunSprites();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _input.OnInteractPressed -= HandleItemInteract;
    }
}
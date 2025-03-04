using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class VendorScript : MonoBehaviour
{

    public GameObject itemPrefab1;
    public GameObject itemPrefab2;
    public GameObject itemPrefab3;
    public GameObject itemPrefab4;
    public GameObject itemPrefab5;

    private ItemScript itemScript1;
    private ItemScript itemScript2;
    private ItemScript itemScript3;
    private ItemScript itemScript4;
    private ItemScript itemScript5;
    private bool soldout = false;
    private Animator animator;

    void Start()
    {
        // Ugly hack but yeah...this will work 
        if (itemPrefab1 != null)
        {
            itemScript1 = itemPrefab1.GetComponent<ItemScript>();
        }

        if (itemPrefab2 != null)
        {
            itemScript2 = itemPrefab2.GetComponent<ItemScript>();
        }

        if (itemPrefab3 != null)
        {
            itemScript3 = itemPrefab3.GetComponent<ItemScript>();
        }

        if (itemPrefab4 != null)
        {
            itemScript4 = itemPrefab4.GetComponent<ItemScript>();
        }

        if(itemPrefab5 != null)
        {
            itemScript5 = itemPrefab5.GetComponent<ItemScript>();
        }

        ItemInfoScript infoScript = GetComponentInChildren<ItemInfoScript>(true);
        if (infoScript != null)
        {
            itemScript1.SetInfoPanel(infoScript);
            itemScript2.SetInfoPanel(infoScript);
            itemScript3.SetInfoPanel(infoScript);
            itemScript4.SetInfoPanel(infoScript);
            itemScript5.SetInfoPanel(infoScript);
        }

        ItemInfoRender infoRender = GetComponentInChildren<ItemInfoRender>(true);
        if (infoRender != null)
        {
            itemScript1.SetInfoRenderer(infoRender);
            itemScript2.SetInfoRenderer(infoRender);
            itemScript3.SetInfoRenderer(infoRender);
            itemScript4.SetInfoRenderer(infoRender);
            itemScript5.SetInfoRenderer(infoRender);
        }

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!soldout)
        {

        }
        
    }

}

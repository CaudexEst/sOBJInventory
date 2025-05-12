/*******************************************************************
* COPYRIGHT       : 2025
* PROJECT         : 24-25 Production
* FILE NAME       : UpgradeSlot.cs
* DESCRIPTION     : Slots to hold the upgrades in the shop
*                    
* REVISION HISTORY:
* Date 			Author    		        Comments
* ---------------------------------------------------------------------------
* 2025/03/10		Ben Jenkins    		 Created
* 
*
/******************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    public Upgrades Upgrade;
    public Image Sprite;
    public Image BoughtSprite;

    public SlotType SlotType;

    void Start()
    {
        if (Upgrade == null)
        {
            Sprite.enabled = false;
        }
        else
        {
            Sprite.sprite = Upgrade.Artwork;

        }

        if (Upgrade != null && Upgrade.Acquired)
        {
            BoughtSprite.enabled = true;
        }
    }

    private void Update()
    {
        if(Upgrade!=null && Upgrade.Acquired == true && SlotType==SlotType.Inventory)
        {
            BoughtSprite.enabled = false;
        }
    }

    public void Bought()
    {
        BoughtSprite.enabled = true;
        this.gameObject.GetComponent<Button>().interactable = false;
    }

    public void DisplayScrap()
    {
        if (SlotType == SlotType.Inventory)
            UpgradeManager.s_Instance.DisplayItem(Upgrade);
        else if (SlotType == SlotType.Shop)
        {
            UpgradeShop.s_Instance.DisplayItem(Upgrade,this);
        }
    }

    public void UpdateUpgrade(Upgrades UpgradeNew)
    {
        Upgrade = UpgradeNew;
        Sprite.enabled = true;
        Sprite.sprite = Upgrade.Artwork;
    }

}
public enum SlotType
{
    Inventory,
    Shop
}
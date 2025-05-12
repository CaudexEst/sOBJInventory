/*******************************************************************
* COPYRIGHT       : Year
* PROJECT         : Name of Project or Assignment script is used for.
* FILE NAME       : UpgradeShop.cs
* DESCRIPTION     : Short Description of script.
*                    
* REVISION HISTORY:
* Date 			Author    		        Comments
* ---------------------------------------------------------------------------
* 2000/01/01		Developer's Name    		 Created <short comment of changes>
* 
*
/******************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShop : MonoBehaviour
{

    public TextMeshProUGUI DisplayName;
    public TextMeshProUGUI DisplayDescription;
    public TextMeshProUGUI DisplayCost;
    public Image DisplaySprite;
    private Upgrades CurrentSelected;
    public UpgradeSlot LastSlot;
    private static UpgradeShop s_instance;
    public static UpgradeShop s_Instance { get { return s_instance; } }
    private UpgradeManager s_manager;
    private InventoryManager s_inventoryManager;

    void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        s_instance = this;
        FindUpgradeManager();
    }

    private void FindUpgradeManager()
    {
        foreach (UpgradeManager upg in Resources.FindObjectsOfTypeAll<UpgradeManager>())
        {
            if (upg.gameObject.CompareTag("UpgradeManager"))
            {
                s_manager = upg;
            }
        }
        foreach (InventoryManager inv in Resources.FindObjectsOfTypeAll<InventoryManager>())
        {
            if (inv.gameObject.CompareTag("DisplayBoard"))
            {
                s_inventoryManager = inv;
            }
        }


    }

    public void DisplayItem(Upgrades Upgrade,UpgradeSlot US)
    {
        if (Upgrade == null)
            return;
        LastSlot = US;
        CurrentSelected = Upgrade;
        DisplayName.text = Upgrade.Name;
        DisplayDescription.text = Upgrade.Description;
        DisplayCost.text = "$" + Upgrade.Cost.ToString();
        DisplaySprite.sprite = Upgrade.Artwork;
        DisplaySprite.color = new Color(DisplaySprite.color.r, DisplaySprite.color.g, DisplaySprite.color.b, 1f);

        //NewSelected?.Invoke(CurrentSelected);
    }

    public void ClearDisplay()
    {
        CurrentSelected = null;
        DisplayName.text = null;
        DisplayDescription.text = null;
        DisplayCost.text = null;
        DisplaySprite.sprite = null;
        DisplaySprite.color = new Color(DisplaySprite.color.r, DisplaySprite.color.g, DisplaySprite.color.b, 0f);
        //NewSelected?.Invoke(CurrentSelected);
    }

    public void BoughtUpgrade()
    {
        if (CurrentSelected == null || CurrentSelected.Acquired == true || InventoryManager.s_Instance.GetTonsks()<CurrentSelected.Cost)
            return;
        s_manager.AddtoUpgrades(CurrentSelected);
        s_manager.Invoke(CurrentSelected.InvokeMethod, 0f);
        CurrentSelected.Acquired = true;
        s_inventoryManager.SubtractTonsk(CurrentSelected.Cost);
        LastSlot.Bought();
    }


}

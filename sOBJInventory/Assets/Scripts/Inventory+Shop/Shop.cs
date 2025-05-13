/*******************************************************************
* COPYRIGHT       : 2024
* PROJECT         : 24-25 Production
* FILE NAME       : Shop.cs
* DESCRIPTION     : Shop for selling scrap to NPCs
*                    
* REVISION HISTORY:
* Date 			Author    		        Comments
* ---------------------------------------------------------------------------
* 2024/12/05		Ben Jenkins    		 Created 
* 
*
/******************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Shop : InventoryManager
{
    //Add to NPC shop gameobject and set content to whatever contains the InventorySlots
    public GameObject PlayerInventorySpot;

    [SerializeField]
    private InventoryManager _pInventoryManager;
    [SerializeField]
    private Scrap CurrentlySelected;
    [SerializeField]
    private GameObject _playerInventory;
    private Vector3 _previousPISpot;
    private Vector3 _previousPIScale;
    private Vector3 _DBScale;
    [SerializeField]
    private GameObject _SIC;
    private GameObject _DB;
    [SerializeField]
    private GameObject _DBSH; //Sell Identifier
    public ShopPreference ShopOwner;
    private float _sellMult = 1f;

    // Awake is called once at instantiation
    void Awake()
    {
        FindInventory();
        //_previousPISpot = _playerInventory.transform.position;
        //_previousPIScale = _playerInventory.transform.localScale;
        //_DBScale = _DB.transform.localScale;
    }

    //When enabled, subscribe to display board selection
    private void OnEnable()
    {
        //Subscribes to view selected item
        if (_pInventoryManager != null)
        {
            InventoryManager.s_Instance.NewSelected += UpdateSelected;
        }
        _playerInventory.SetActive(true);
        
        //Moves player inventory to correct spot and size
        _playerInventory.GetComponent<RectTransform>().position = PlayerInventorySpot.GetComponent<RectTransform>().position/*new Vector3(-320f, 260f, 0f)*/;
        _playerInventory.GetComponent<RectTransform>().transform.localScale = PlayerInventorySpot.GetComponent<RectTransform>().transform.localScale/*new Vector3(2f, 2f, 2f)*/;
        _DB.GetComponent<RectTransform>().transform.localScale = new Vector3(2f, 2f, 2f);
        //_DB.GetComponent<Image>().enabled = true;
        _SIC.SetActive(true);
        Debug.Log(_SIC.activeSelf);
        //turns off extra images and gameobjects not needed for the shop
        foreach(GameObject gameObject in Object.FindObjectsOfType(typeof(GameObject), includeInactive: true))
        {
            //Debug.Log(gameObject.name);
            if(gameObject.CompareTag("TurnOff"))
            {
                gameObject.SetActive(false);

                //Debug.Log("Enable Turn off: " +gameObject.name);
            }
            if(gameObject.CompareTag("TurnOffImage"))
            {
                gameObject.GetComponent<Image>().enabled = false;

                //Debug.Log("Enable turn off image: " + gameObject.name);
            }
        }


    }

    //When disabled, unsubscribe
    private void OnDisable()
    {
        //unsubscribes, moves player inventory back, fixes scale, and turns objects back on
        Debug.Log("OnDisable");
        if (_pInventoryManager != null)
        {
            InventoryManager.s_Instance.NewSelected -= UpdateSelected;
        }
        
        _playerInventory.GetComponent<RectTransform>().transform.position = _previousPISpot;
        _playerInventory.GetComponent<RectTransform>().transform.localScale = _previousPIScale;
        _DB.GetComponent<RectTransform>().transform.localScale = _DBScale;
        _DB.GetComponent<Image>().enabled = false;
        

        //hard coded for demo
        foreach (GameObject gameObject in Object.FindObjectsOfType(typeof(GameObject), includeInactive: true))
        {
            if (gameObject.CompareTag("TurnOff"))
            {
                //Debug.Log("Disable Turn off");
                gameObject.SetActive(true);
                
            }
            if (gameObject.CompareTag("TurnOffImage"))
            {
                gameObject.GetComponent<Image>().enabled = true;
                //Debug.Log("Disable turn off image");
            }
        }
        _DBSH.SetActive(false);
        _playerInventory.SetActive(false);
        _SIC.SetActive(false);
    }

    private void FindInventory()
    {
        foreach(InventoryManager inv in Resources.FindObjectsOfTypeAll<InventoryManager>())
        {
            if (inv.gameObject.CompareTag("DisplayBoard"))
            {
                _pInventoryManager = inv;
                _DB = inv.gameObject;
            }
        }
        foreach(GameObject gameObject in Object.FindObjectsOfType(typeof(GameObject), includeInactive: true))
        {
            if(gameObject.CompareTag("Inventory"))
            {
                _playerInventory = gameObject;
            }
            if (gameObject.CompareTag("ShipInteriorCanvas"))
            {
                _SIC = gameObject;
            }
            if(gameObject.CompareTag("SellHint"))
            {
                _DBSH = gameObject;
            }
        }
    }

    //Subscription
    private void UpdateSelected(Scrap scrap)
    {
        //_DBSH.SetActive(false);
        _sellMult = 1f;
        CurrentlySelected = scrap;
        if (_DBSH == null || CurrentlySelected == null)
            return;
        (bool ifShopMatches, ShopAmountLiked AmountLiked) ShopPref = CheckShopPrefList(scrap);

        if (ShopOwner == ShopPreference.Manager)
        {
            _pInventoryManager.SetSingleArrow();
            _DBSH.SetActive(true);
            _DBSH.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
            _sellMult = 0.80f;
        }
        else if (ShopOwner == ShopPreference.Otto) { _sellMult = 1f; }
        else if (ShopOwner == ShopPreference.Adanna) { _sellMult = 1.2f; }
        else if (ShopOwner == ShopPreference.Adder) { _sellMult = 1.2f; }
        else if (ShopOwner == ShopPreference.MirAi) { _sellMult = 1.4f; }
        else if (ShopOwner == ShopPreference.Scotoplanes) { _sellMult = 1.4f; }

        if (ShopPref.ifShopMatches)
        {
            _DBSH.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180);
            if(ShopPref.AmountLiked == ShopAmountLiked.Normal) { return; }
            else if (ShopPref.AmountLiked == ShopAmountLiked.Likes) { _sellMult+=0.25f; _pInventoryManager.SetSingleArrow(); }
            else if (ShopPref.AmountLiked == ShopAmountLiked.ReallyLikes) { _sellMult += 0.5f; _pInventoryManager.SetDoubleArrow(); }
            _DBSH.SetActive(true);
        }
    }

    private (bool ifShopMatches, ShopAmountLiked AmountLiked) CheckShopPrefList(Scrap scrap)
    {
        if (scrap.ShopPrefereneceList.Count == 0)
            return (false, 0);
        foreach(ShopPrefereneceList pref in scrap.ShopPrefereneceList)
        {
            if (ShopOwner == pref.pref)
                return (true, pref.AmountLiked);
        }
        return (false, 0);
    }
        

    //Put on sell button in shop. Uses subscribed scrap to add to shop inventory, remove from player, and give them tonsks
    public void Sold()
    {
        if(CurrentlySelected == null || CurrentlySelected.PlayerAmount <= 0 || CurrentlySelected.IsQuestItem)
        {
            Debug.Log("Nothing selected");
            return;
        }
        AddToInventory(CurrentlySelected);
        CurrentlySelected.SoldToShop();
        _pInventoryManager.AddTonsk((int)((float)CurrentlySelected.Cost * _sellMult));
        if(CurrentlySelected.PlayerAmount<=0)
        {
            _pInventoryManager.RemoveFromPlayerInventory(CurrentlySelected);
            _DBSH.SetActive(false);
        }
    }

    public void SellAll()
    {
        List<Scrap> scrapList = InventoryManager.s_Instance.ScrapInInventory.ToList();

        foreach (Scrap scrap in scrapList)
        {
            UpdateSelected(scrap);
            while(scrap.PlayerAmount>0)
            {
                Sold();
            }
        }
        
    }

    //modified AddToPlayerInventory
    public void AddToInventory(Scrap scrap)
    {
        (bool FoundBool, Scrap FoundScrap) FoundReturn = FindItemInList(scrap);

        if (!FoundReturn.FoundBool)//If scrap does not exist in the HashSet, add it
        {
            ScrapInInventory.Add(scrap);
            //Debug.Log("Added to hashset");
            //Debug.Log("increased amount");
            foreach (Transform child in Content.gameObject.transform)
            {
                //Debug.Log(child.gameObject.tag);
                if (child.gameObject.tag.Equals("InventorySlot"))
                {
                    //Debug.Log("Found slot");
                    if (child.gameObject.GetComponent<InventorySlots>().Scrap == null)
                    {
                        child.gameObject.GetComponent<InventorySlots>().UpdateScrap(scrap);
                        //Debug.Log("added to shop");
                        break;
                    }
                }
            }
        }
        else if (FoundReturn.FoundScrap)//If scrap does exist in the HashSet, increase the players amount
        {
            if (scrap == FoundReturn.FoundScrap)
            {
                Debug.Log("increased shop amount");
            }
        }
    }

}

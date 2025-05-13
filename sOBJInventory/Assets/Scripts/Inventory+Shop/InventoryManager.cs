/*******************************************************************
* COPYRIGHT       : 2024
* PROJECT         : 24-25 Production
* FILE NAME       : InventoryManager.cs
* DESCRIPTION     : Creates a list of all scriptable objects in this inventory
*                    
* REVISION HISTORY:
* Date 			Author    		        Comments
* ---------------------------------------------------------------------------
* 2024/10/02 | Ben Jenkins | Created the script.
* 2024/10/10 | Ariana Kim | Updated into a singleton.
* 2024/10/28 | William Pittenger | Added ScrapValueInPlayerInventory()
* 2024/12/05 | Ben Jenkins | Added selling functionality with observer pattern
*
/******************************************************************/

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager s_instance;
    public static InventoryManager s_Instance { get { return s_instance; } }

    //public List<Scrap> ScrapInInventory = new List<Scrap>();
    public HashSet<Scrap> ScrapInInventory = new HashSet<Scrap>();
    private List<string> _ScrapInHolding = new List<string>();
    //private GameManager gm;
    public int _tonsks=0;
    public TextMeshProUGUI DisplayName;
    public TextMeshProUGUI DisplayDescription;
    public TextMeshProUGUI DisplayCost;
    public TextMeshProUGUI DisplayPlayerAmount;
    public TextMeshProUGUI TonskAmount;
    public Image DisplayHint;
    private Scrap CurrentSelected;
    public Image DisplaySprite;
    public GameObject Quest;
    public GameObject Content;
    public event Action<Scrap> NewSelected;
    //private Health _PlayerHealth;
    public Sprite SingleArrow;
    public Sprite DoubleArrow;

    public UnityEvent<Scrap> ItemAdded;

    public Scrap[] scrapSOs;

    // Awake is called once at instantiation
    //Adds all scrap contained in the InventorySlots to the HashSet
    private void Awake()
    {
        if(s_instance != null && s_instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        s_instance = this;
        
        foreach(Transform child in Content.gameObject.transform)
        {
            if(child.gameObject.tag.Equals("InventorySlot"))
            {
                if(child.gameObject.GetComponent<PlayerInventorySlot>().Scrap!=null)
                    ScrapInInventory.Add(child.gameObject.GetComponent<PlayerInventorySlot>().Scrap);
                //Debug.Log(ScrapInInventory.Last().Name);
            }
        }
        TonskAmount.text = "Bal: " + _tonsks;
        /*
        SceneManager.activeSceneChanged += ActiveSceneChanged; //subscribes to event
        if(_PlayerHealth != null)
        {
            _PlayerHealth.Dead += RemoveHoldingfromHashSet;
        }*/
    }

    public int GetTonsks()
    {
        return _tonsks;
    }

    public void AddTonsk(int amount)
    {
        _tonsks += amount;
        TonskAmount.text = "Bal: " + _tonsks;
        //AkSoundEngine.PostEvent("MoneyGainedOrSpent", this.gameObject);
    }

    public void SubtractTonsk(int amount)
    {
        if(_tonsks-amount>=0)
        {
            _tonsks -= amount;
            TonskAmount.text = "Bal: " + _tonsks ;
            //AkSoundEngine.PostEvent("MoneyGainedOrSpent", this.gameObject);
        }
        else
        {
            _tonsks -= amount;
            TonskAmount.text = "Bal: " + _tonsks;
            //AkSoundEngine.PostEvent("MoneyGainedOrSpent", this.gameObject);
        }
        
    }

    public void DisplayItem(Scrap scrap)
    {
        if (scrap == null)
            return;
        CurrentSelected = scrap;
        DisplayName.text = scrap.Name;
        DisplayDescription.text = scrap.Description;
        DisplayCost.text = "$" + scrap.Cost.ToString();
        DisplayPlayerAmount.text = scrap.PlayerAmount.ToString();
        DisplaySprite.sprite = scrap.Artwork;
        Quest.SetActive(scrap.IsQuestItem);
        DisplaySprite.color = new Color(DisplaySprite.color.r, DisplaySprite.color.g, DisplaySprite.color.b, 1f);
        
        NewSelected?.Invoke(CurrentSelected);
    }

    //Check HashSet for if the scrap exists by its reference
    public (bool, Scrap found) FindItemInList(Scrap scrap)
    {
        foreach(Scrap Current in ScrapInInventory)
        {
            //Debug.Log(Current.name);
            if (Current.Equals(scrap))
                return (true,found: Current);
        }
        return (false,null);
    }//Returns a struct with a bool for if it was found and its reference if found

    //Check HashSet for if the scrap exists by its name
    public (bool, Scrap found) FindItemInList(string name)
    {
        foreach (Scrap Current in ScrapInInventory)
        {
            if (Current.name.Equals(name))
                return (true, found: Current);
        }
        return (false, null);
    }//Returns a struct with a bool for if it was found and its reference if found

    //Adds scrap to current Inventory, either by adding it to the HashSet or increasing the amount
    public void AddToPlayerInventory(Scrap scrap)
    {
        (bool FoundBool,Scrap FoundScrap) FoundReturn = FindItemInList(scrap);

        if(!FoundReturn.FoundBool)//If scrap does not exist in the HashSet, add it
        {
            ScrapInInventory.Add(scrap);
            //Debug.Log("Added to hashset");
            //Debug.Log("increased amount");
            scrap.PlayerAmount++;
            foreach (Transform child in Content.gameObject.transform)
            {
                //Debug.Log(child.gameObject.tag);
                if (child.gameObject.tag.Equals("InventorySlot"))
                {
                    //Debug.Log("Found slot");
                    if (child.gameObject.GetComponent<PlayerInventorySlot>().Scrap == null)
                    {
                        child.gameObject.GetComponent<PlayerInventorySlot>().UpdateScrap(scrap);
                        //Debug.Log("added to slot");
                        break;
                    }
                    
                }
            }
        }
        else if(FoundReturn.FoundScrap)//If scrap does exist in the HashSet, increase the players amount
        {
            if(scrap == FoundReturn.FoundScrap)
            {
                scrap.PlayerAmount++;
                //Debug.Log("increased amount");
            }
        }

        ItemAdded.Invoke(scrap);
    }

    public void SubtractFromPlayerInventory(Scrap scrap)
    {
        (bool FoundBool, Scrap FoundScrap) FoundReturn = FindItemInList(scrap);

        if (!FoundReturn.FoundBool)//If scrap does not exist in the HashSet, send a log
        {
            Debug.LogWarning("Scrap not found in inventory");
        
        }
        else if (FoundReturn.FoundScrap)//If scrap does exist in the HashSet, subtract the players amount
        {
            if (scrap == FoundReturn.FoundScrap)
            {
                scrap.PlayerAmount--;
            }
        }

        if(scrap.PlayerAmount == 0)
        {
            RemoveFromPlayerInventory(scrap);
        }

    }

    public void RemoveFromPlayerInventory(Scrap scrap)
    {
        (bool FoundBool, Scrap FoundScrap) FoundReturn = FindItemInList(scrap);
        AddToHoldingList(scrap);

        if (!FoundReturn.FoundBool)//If scrap does not exist in the HashSet, send a log
        {
            Debug.LogWarning("Scrap not found in inventory to remove");
        }
        else if (FoundReturn.FoundScrap)//If scrap does exist in the HashSet, remove it from the HashSet
        {
            if (scrap == FoundReturn.FoundScrap && scrap.PlayerAmount<=0)
            {
                Transform previous =null;
                bool found = false;
                ScrapInInventory.Remove(scrap);
                foreach (Transform child in Content.gameObject.transform)
                {
                    if (child.gameObject.tag.Equals("InventorySlot"))
                    {
                        if (child.gameObject.GetComponent<PlayerInventorySlot>().Scrap == scrap || found)
                        {
                            if (found && previous!=null && child.gameObject.GetComponent<PlayerInventorySlot>().Scrap != null)
                            {
                                previous.gameObject.GetComponent<PlayerInventorySlot>().UpdateScrap(child.gameObject.GetComponent<PlayerInventorySlot>().Scrap);
                            }
                            child.gameObject.GetComponent<PlayerInventorySlot>().Scrap=null;
                            child.gameObject.GetComponent<PlayerInventorySlot>().PlayerAmount.enabled = false;
                            child.gameObject.GetComponent<PlayerInventorySlot>().Sprite.enabled = false;

                            ClearDisplay();
                            previous = child;
                            found = true;
                            
                            //break;
                        }

                    }
                }
            }
        }
    }

    //Clears the display board if the referenced item is removed from the inventory
    public void ClearDisplay()
    {
        CurrentSelected = null;
        DisplayName.text = null;
        DisplayDescription.text = null;
        DisplayCost.text = null;
        DisplayPlayerAmount.text = null;
        DisplaySprite.sprite = null;
        DisplaySprite.color = new Color(DisplaySprite.color.r, DisplaySprite.color.g, DisplaySprite.color.b, 0f);
        NewSelected?.Invoke(CurrentSelected);
    }

    // Returns the total value of scrap in the player's inventory.
    public int ScrapValueInPlayerInventory()
    {
        // TODO: optimize if necessary, just store the value and update it when items are added/removed.

        int total = 0;
        foreach (Scrap scrap in ScrapInInventory)
        {
            total += scrap.PlayerAmount * scrap.Cost;
        }
        return total;
    }

    //Whenever the scene changes, clear the temp list and search for PlayerShipNew
    private void ActiveSceneChanged(Scene current, Scene loaded)
    {
        ClearHoldingList();
        foreach (GameObject gameObject in FindObjectsOfType(typeof(GameObject), includeInactive: true))
        {
            if(gameObject.CompareTag("Player"))
            {
                //_PlayerHealth = gameObject.GetComponent<Health>();
            }
        }
    }

    //Add to the temp list
    private void AddToHoldingList(Scrap scrap)
    {
        _ScrapInHolding.Add(scrap.name);
    }

    //Clear temp list
    private void ClearHoldingList()
    {
        _ScrapInHolding = new List<string>();
    }

    //Removes from the HashSet the items of the temp list
    private void RemoveHoldingfromHashSet()
    {
        foreach(string name in  _ScrapInHolding)
        {
            (bool FoundBool, Scrap FoundScrap) FoundReturn = FindItemInList(name);
            if (FoundReturn.FoundBool == true)
                RemoveFromPlayerInventory(FoundReturn.FoundScrap);
        }
        ClearHoldingList();
    }

    public void SetSingleArrow()
    {
        DisplayHint.sprite = SingleArrow;
    }
    public void SetDoubleArrow()
    {
        DisplayHint.sprite = DoubleArrow;
    }


    //Fix for items not going back into previous slots when previous item is removed: capture reference to previous slot during transform foreach,
    //and move scrap from current slot into previous one. Start loop with manual reference to first slot

    //Temp inventory for death: Separate Hashset that tracks what is added to the inventory from the start of a shift
    //Items still add normally, but if the player dies, items are removed from the inventory based on temp Hashset. Hashset is reset at the offshift


    //Shop is a subclass of inventory manager. At start of scene, make shop search for inventory manager. When "Sell" is pressed
    //get current display board item and sell, using Scrap.SellToShop


    
    public Scrap Geode;
    [ContextMenu("Add Geode")]
    public void AddGeode()
    {
        AddToPlayerInventory(Geode);
    }
    public void AddSoda()
    {
        AddToPlayerInventory(banjo);
    }
    
    public Scrap banjo;
    public void RemoveBanjo()
    {
        SubtractFromPlayerInventory(banjo);
    }
    

    /*
    public static InventoryData Save()
    {
        return new InventoryData
        {
            Scraps = s_Instance.scrapSOs.ToList(),
            Tonsks = s_Instance._tonsks,
        };
    }


#if UNITY_EDITOR
    [ContextMenu("Update Scrap List")]
    public void UpdateScrapList()
    {
        var scrapSOs = new List<Scrap>();

        // Load all scrap Scriptable Objects
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(Scrap).FullName, new[] { "Assets/Scriptable Objects/" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Scrap scrap = AssetDatabase.LoadAssetAtPath<Scrap>(path);
            scrapSOs.Add(scrap);
        }

        this.scrapSOs = scrapSOs.ToArray();
    }
#endif

    public static void Load(JObject data)
    {
        Dictionary<string, Scrap> tempScrap = new();
        foreach (var scrap in s_instance.scrapSOs) tempScrap.Add(scrap.name, scrap);


        JArray savedScarp = data["Scraps"].Value<JArray>();
        foreach (JObject scrapData in savedScarp)
        {
            string name = scrapData["Name"].Value<string>();
            if (tempScrap.TryGetValue(name, out Scrap scrap))
            {
                JsonUtility.FromJsonOverwrite(scrapData.ToString(), scrap);
                if (scrap.PlayerAmount > 0)
                {
                    scrap.PlayerAmount -= 1;
                    s_Instance.AddToPlayerInventory(scrap);
                }
                else
                {
                    s_Instance.RemoveFromPlayerInventory(scrap);
                }
            }
        }

        s_Instance._tonsks = data["Tonsks"].Value<int>();
        s_Instance.AddTonsk(0);
    }
    */
}

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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private static UpgradeManager s_instance;
    public static UpgradeManager s_Instance { get { return s_instance; } }
    public HashSet<Upgrades> Upgrades = new HashSet<Upgrades>();

    public TextMeshProUGUI DisplayName;
    public TextMeshProUGUI DisplayDescription;
    public TextMeshProUGUI DisplayCost;
    public Image DisplaySprite;
    private Upgrades CurrentSelected;
    public GameObject Content;
    public event Action<string> ColorBought; //Event for observer that sends a string
    public event Action ShipColorChanged; //Event that doesn't have a param

    public Upgrades[] upgradeSOs; //master list of all upgrades to save their name and acquired state

    // Awake is called once at instantiation
    void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        s_instance = this;
    }
    private void Start()
    {
        foreach (GameObject gameObject in FindObjectsOfType(typeof(GameObject), includeInactive: true))
        {
            if (gameObject.CompareTag("ColorManager"))
                return;
                    //gameObject.GetComponent<ColorManager>().FoundColorManager(s_Instance);
        }
        foreach (Upgrades upgrade in upgradeSOs)
            upgrade.Acquired = false;
    }


    public void DisplayItem(Upgrades Upgrade)
    {
        if (Upgrade == null)
            return;
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

    public void AddtoUpgrades(Upgrades Upgrade)
    {
        Upgrades.Add(Upgrade);
        foreach (Transform child in Content.gameObject.transform)
        {
            //Debug.Log(child.gameObject.tag);
            if (child.gameObject.tag.Equals("UpgradeSlot"))
            {
                //Debug.Log("Found slot");
                if (child.gameObject.GetComponent<UpgradeSlot>().Upgrade == null)
                {
                    child.gameObject.GetComponent<UpgradeSlot>().UpdateUpgrade(Upgrade);
                    //Debug.Log("added to slot");
                    break;
                }

            }
        }
    }
   /*
    public static UpgradeData Save()
    {
        Debug.Log($"UgradeData Saving {s_Instance == null}");
        return new UpgradeData
        {
            
            Upgrades = s_Instance.upgradeSOs.ToList()
        };
    }

    public static void Load(JObject data)
    {
        Dictionary<string, Upgrades> tempUpg = new();
        foreach (var Upgrade in s_instance.upgradeSOs) tempUpg.Add(Upgrade.Name, Upgrade);


        JArray savedUpgrades = data["Upgrades"].Value<JArray>();
        foreach (JObject upgradeData in savedUpgrades)
        {
            string name = upgradeData["Name"].Value<string>();
            if (tempUpg.TryGetValue(name, out Upgrades upgrade))
            {
                JsonUtility.FromJsonOverwrite(upgradeData.ToString(), upgrade);
                
                if(upgrade.Acquired)
                {
                    s_instance.Invoke(upgrade.InvokeMethod, 0f);
                    s_Instance.AddtoUpgrades(upgrade);
                }
                
            }
        }
    } */
   

    public void Test()
    {
        Debug.Log("Bought test");
    }

    //public ProjectileScript PlayerBullet;
    public GameObject[] PlayerBulletPrefabs;
    public Material NewBulletColor;
    public Material ShipColor;
    public bool TurnedOn = false;
    public bool LaserOn = false;
    public bool ShipOn = false;
    public void DamageUp()
    {
        //PlayerBullet.damage = 10f;
        foreach(GameObject go in PlayerBulletPrefabs)
        {
            go.GetComponent<MeshRenderer>().material = NewBulletColor;
        }
    }
    public void EnableShipLaserColor()
    {
        ColorBought?.Invoke("Laser");//Pings observer with param
        TurnedOn = true;
        LaserOn = true;
    }
    public void EnableShipColor()
    {
        ColorBought?.Invoke("Ship");
        TurnedOn = true;
        ShipOn = true;
    }
    public void ChangeLaserColor(Material m)
    {
        NewBulletColor = m;
    }
    public void ChangeShipColor(Material m)
    {
        ShipColor = m;
        ShipColorChanged?.Invoke(); //pings observer
    }

    public bool IsShipDashAcquired = false;
    public void EnableShipDash()
    {
        IsShipDashAcquired = true;
    }

    public bool IsJougAcquired = false;
    public void EnableJoug()
    {
        IsJougAcquired = true;
    }

    public bool IsHealAcquired = false;
    public void EnableHeal()
    {
        IsHealAcquired = true;
    }


}

/*******************************************************************
* COPYRIGHT       : 2024
* PROJECT         : 24-25 Production
* FILE NAME       : Scrap.cs
* DESCRIPTION     : Scriptable Object template for scrap
*                    
* REVISION HISTORY:
* Date 			Author    		        Comments
* ---------------------------------------------------------------------------
* 2024/09/22		Ben Jenkins    		 Created
* 2024/10/7         Ben Jenkins          Added selling methods
* 
*
/******************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "New Scrap", menuName = "New Scrap")]
public class Scrap : ScriptableObject
{
    //Fields for basic scrap
    
    public string Name;
    public string Description;

    public Sprite Artwork;

    public int Cost;
    public int PlayerAmount;
    public int ShopAmount;
    public bool IsLargeScrap;
    [Tooltip("True if picking up this item should contribute to pickup quests. Consider setting to false for quest specific items.")]
    public bool CountByPickup = true;
    public bool IsQuestItem = false;

    public List<ShopPrefereneceList> ShopPrefereneceList;

    public GameObject SpriteRenderedObject;

    public void SoldToShop()
    {
        if (PlayerAmount > 0)
        {
            ShopAmount++;
            PlayerAmount--;
        }
        else
            Debug.LogError("Player does not have the item to sell");

    }

    public void BoughtByPlayer()
    {
        if(ShopAmount>0)
        {
            ShopAmount--;
            PlayerAmount++;
        }
        else
            Debug.LogError("The shop does not have the item to sell");
    }


    private void Awake()
    {
        //Debug.Log("scrap awake");
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += ModeChanged;
#endif
    }

#if UNITY_EDITOR
    void ModeChanged(PlayModeStateChange playModeState)
    {
        if (playModeState == PlayModeStateChange.ExitingPlayMode)
        {
            //Debug.Log($"Clearing '{Name}' amounts");
            
            PlayerAmount = 0;
            ShopAmount = 0;
        }

    }
#endif
}
public enum ShopPreference
{
    Nobody,
    Manager,
    Otto,
    Adanna,
    Adder,
    MirAi,
    Scotoplanes
}

public enum ShopAmountLiked
{
    Normal,
    Likes,
    ReallyLikes
}

[Serializable]
public class ShopPrefereneceList
{
    public ShopPreference pref;
    public ShopAmountLiked AmountLiked;
}


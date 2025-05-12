/*******************************************************************
* COPYRIGHT       : 2023
* PROJECT         : 24-25 Production
* FILE NAME       : PlayerInventorySlot.cs
* DESCRIPTION     : Player inventory slot
*                    
* REVISION HISTORY:
* Date 			Author    		        Comments
* ---------------------------------------------------------------------------
* 2024/10/3		Ben Jenkins    		 Created
* 2024/10/7     Ben Jenkins          Fixed the display issue
* 
*
/******************************************************************/

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class PlayerInventorySlot : MonoBehaviour
{
    public Scrap Scrap;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI Cost;
    public TextMeshProUGUI PlayerAmount;
    public Image Sprite;
    public GameObject Tooltip;
    private InventoryManager InventoryManager;
    public GameObject AnimatedGO;
    public Animator SpriteAnimator;
    public SpriteRenderer AnimatedSprite;
    public GameObject Quest;


    // Start is called before the first frame update
    void Start()
    {
        if(Scrap==null)
        {
            Sprite.enabled = false;
            PlayerAmount.enabled = false;
        }
        else
        {
            Name.text = Scrap.Name;
            Description.text = Scrap.Description;
            Cost.text = "$" + Scrap.Cost.ToString();
            PlayerAmount.text = Scrap.PlayerAmount.ToString();
            if(AnimatedGO!=null && Scrap.SpriteRenderedObject != null)
            {
                Debug.Log("Start assign");
                AnimatedSprite.sprite = Scrap.SpriteRenderedObject.GetComponent<SpriteRenderer>().sprite;
                SpriteAnimator.runtimeAnimatorController = Scrap.SpriteRenderedObject.GetComponent<Animator>().runtimeAnimatorController;
                AnimatedGO.GetComponent<SpriteRenderer>().sprite = AnimatedSprite.sprite;
                AnimatedGO.GetComponent<Animator>().runtimeAnimatorController = SpriteAnimator.runtimeAnimatorController;

            }
            Sprite.sprite = Scrap.Artwork;
            Quest.SetActive(Scrap.IsQuestItem);
        }
            InventoryManager = GameObject.Find("DisplayBoard").GetComponent<InventoryManager>();
    }

    public void Update()
    {
        if(PlayerAmount.isActiveAndEnabled == true)
        {
            PlayerAmount.text=Scrap.PlayerAmount.ToString();
        }
        AnimatedGO.transform.LookAt(new Vector3(0,0,-1f));
    }

    public void UpdateScrap(Scrap scrap)
    {
        Scrap = scrap;
        Sprite.enabled = true;
        PlayerAmount.enabled=true;
        Name.text = Scrap.Name;
        Description.text = Scrap.Description;
        Cost.text = "$" + Scrap.Cost.ToString();
        PlayerAmount.text = Scrap.PlayerAmount.ToString();
        Sprite.sprite = Scrap.Artwork;
        Quest.SetActive(Scrap.IsQuestItem);
        //Debug.Log(AnimatedGO);
        if (AnimatedGO != null && scrap.SpriteRenderedObject!=null)
        {
            //Debug.Log("UpdateScrap assign");
            AnimatedSprite.sprite = Scrap.SpriteRenderedObject.GetComponent<SpriteRenderer>().sprite;
            SpriteAnimator.runtimeAnimatorController = Scrap.SpriteRenderedObject.GetComponent<Animator>().runtimeAnimatorController;
            AnimatedGO.GetComponent<SpriteRenderer>().sprite = AnimatedSprite.sprite;
            AnimatedGO.GetComponent<Animator>().runtimeAnimatorController = SpriteAnimator.runtimeAnimatorController;
        }
    }

    public void DisplayScrap()
    {
        InventoryManager.DisplayItem(Scrap);
    }
}

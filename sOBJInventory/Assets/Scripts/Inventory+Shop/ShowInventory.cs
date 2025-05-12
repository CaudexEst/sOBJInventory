/*******************************************************************
* COPYRIGHT       : 2024
* PROJECT         : 24-25 Production
* FILE NAME       : ShowInvetory.cs
* DESCRIPTION     : Controls if the inventory is on screen
*                    
* REVISION HISTORY:
* Date 			Author    		        Comments
* ---------------------------------------------------------------------------
* 2024/10/02 | Ben Jenkins | Created the script.
*
/******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowInventory : MonoBehaviour
{
    public GameObject inventory;
    public bool isActive;
    public GameObject pI;
    public Button inv;
    public Button qst;
    public Button Upgrade;
    public GameObject plyQst;
    public GameObject plyUpg;

    private void Start()
    {
        inventory.SetActive(isActive);
    }

    private void Awake()
    {
        Debug.Log("Locating Scene");
    }
    public void OpenUpgrades()
    {
        Upgrade.interactable = false;
        inv.interactable = true;
        qst.interactable = true;
        pI.SetActive(false);
        plyQst.SetActive(false);
        plyUpg.SetActive(true);
    }
    public void OnInventory()
    {
        Debug.Log($"{inventory.activeSelf}");
        if (inventory.activeSelf)
        {
            //if (ShipMovement.s_Instance != null) ShipMovement.s_Instance.gameObject.SetActive(true);
            OpenInv();
            inventory.SetActive(false);
            pI.SetActive(false);
            OpenInv();
        }
        else
        {
            //if (ShipMovement.s_Instance != null) ShipMovement.s_Instance.gameObject.SetActive(false);
            inventory.SetActive(true);
            pI.SetActive(true);
            OpenInv();
        }
    }

    public void OpenInv()
    {
        if(inventory.activeSelf)
        {
            inv.interactable = false;
            qst.interactable = true;
            Upgrade.interactable = true;
            pI.SetActive(true);
            plyQst.SetActive(false);
            plyUpg.SetActive(false);
        }
    }
    public void OpenQst()
    {
        Upgrade.interactable = true;
        inv.interactable = true;
        qst.interactable = false;
        pI.SetActive(false);
        plyQst.SetActive(true);
        plyUpg.SetActive(false);
    }
 }

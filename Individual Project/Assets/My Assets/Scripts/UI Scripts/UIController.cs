using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    [SerializeField] private ShopKeeperDisplay _shopKeeperDisplay;

    private void Awake()
    {
        _shopKeeperDisplay.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ShopKeeper.OnShopWindowRequested += DisplayShowWindow;
    }

    private void OnDisable()
    {
        ShopKeeper.OnShopWindowRequested -= DisplayShowWindow;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            _shopKeeperDisplay.gameObject.SetActive(false);
        }
    }

    private void DisplayShowWindow(ShopSystem shopSystem, PlayerInventoryHolder playerInventory)
    {
        _shopKeeperDisplay.gameObject.SetActive(true);
        _shopKeeperDisplay.DisplayShopWindow(shopSystem, playerInventory);
    }
}

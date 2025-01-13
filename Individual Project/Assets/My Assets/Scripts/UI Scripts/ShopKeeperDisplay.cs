using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopKeeperDisplay : MonoBehaviour
{
    [SerializeField] private ShopSlotUI _shopSlotPrefab;
    [SerializeField] private ShoppingCartItem_UI _shoppingCartItemPrefab;


    [SerializeField] private Button _buyTab;
    [SerializeField] private Button _sellTab;

    [Header("Shopping Cart")]
    [SerializeField] private TextMeshProUGUI _basketTotalText;
    [SerializeField] private TextMeshProUGUI _playerGoldText;
    [SerializeField] private TextMeshProUGUI _shopGoldText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private TextMeshProUGUI _buyButtonText;

    [Header("Item Preview Section")]
    [SerializeField] private Image _itemPreviewSprite;
    [SerializeField] private TextMeshProUGUI _itemPreviewName;
    [SerializeField] private TextMeshProUGUI _itemPreviewDescription;

    [SerializeField] private GameObject _itemListContentPanel;
    [SerializeField] private GameObject _shoppingCartContentPanel;

    private int _basketTotal;

    private bool _isSelling;

    private ShopSystem _shopSystem;
    private PlayerInventoryHolder _playerInventoryHolder;

    private Dictionary<InventoryItemData, int> _shoppingCart = new Dictionary<InventoryItemData, int>();
    private Dictionary<InventoryItemData, ShoppingCartItem_UI> _shoppingCartUI = new Dictionary<InventoryItemData, ShoppingCartItem_UI>();

    public void DisplayShopWindow(ShopSystem shopSystem, PlayerInventoryHolder playerInventoryHolder)
    {
        _shopSystem = shopSystem;
        _playerInventoryHolder = playerInventoryHolder;

        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if(_buyButton != null)
        {
            _buyButtonText.text = _isSelling ? "Sell Items" : "Buy Items";
            _buyButton.onClick.RemoveAllListeners();
            if (_isSelling)
            {
                _buyButton.onClick.AddListener(SellItems);
            }
            else
            {
                _buyButton.onClick.AddListener(BuyItems);
            }
        }

        ClearSlots();
        ClearItemPreview();

        _basketTotalText.enabled = false;
        _buyButton.gameObject.SetActive(false);
        _basketTotal = 0;
        _playerGoldText.text = $"Player Gold: {_playerInventoryHolder.PrimaryInventorySystem.Gold}";
        _shopGoldText.text = $"Shop Gold: {_shopSystem.AvailableGold}";

        if(_isSelling) DisplayPlayerInventory();
        else DisplayShopInventory();
    }

    private void BuyItems()
    {
        if (_playerInventoryHolder.PrimaryInventorySystem.Gold < _basketTotal) return;

        if (!_playerInventoryHolder.PrimaryInventorySystem.CheckInventoryRemaining(_shoppingCart)) return;

        foreach(var kvp in _shoppingCart)
        {
            _shopSystem.PurchaseItem(kvp.Key, kvp.Value);

            for(int i = 0; i < kvp.Value; i++)
            {
                _playerInventoryHolder.PrimaryInventorySystem.AddToInventory(kvp.Key, 1);
            }
        }

        _shopSystem.GainGold(_basketTotal);
        _playerInventoryHolder.PrimaryInventorySystem.SpendGold(_basketTotal);



        RefreshDisplay();
    }

    private void SellItems()
    {
        if(_shopSystem.AvailableGold <  _basketTotal) return;
        var itemsToSell = _shoppingCart.ToList();

        foreach(var kvp in itemsToSell)
        {
            var price = GetModifiedPrice(kvp.Key, kvp.Value, _shopSystem.SellMarkUp);

            _shopSystem.SellItem(kvp.Key, kvp.Value, price);

            _playerInventoryHolder.PrimaryInventorySystem.GainGold(price);
            _playerInventoryHolder.PrimaryInventorySystem.RemoveItemsFromInventory(kvp.Key, kvp.Value);

            _shoppingCart.Clear();

            RefreshDisplay();
        }
    }

    private void ClearSlots()
    {
        // Clear the shopping cart data
        _shoppingCart.Clear();

        // Clear shopping cart UI elements
        foreach (var uiItem in _shoppingCartUI.Values)
        {
            Destroy(uiItem.gameObject);
        }
        _shoppingCartUI.Clear();

        // Clear item list content panel
        foreach (Transform item in _itemListContentPanel.transform)
        {
            Destroy(item.gameObject);
        }

        // Reset UI elements for the cart
        _basketTotal = 0;
        _basketTotalText.text = string.Empty;
        _basketTotalText.enabled = false;
        _buyButton.gameObject.SetActive(false);
    }

    private void DisplayShopInventory()
    {
        foreach(var item in _shopSystem.ShopInventory)
        {
            if (item.ItemData == null) continue;

            var shopSlot = Instantiate(_shopSlotPrefab, _itemListContentPanel.transform);
            shopSlot.Init(item, _shopSystem.BuyMarkUp);
        }
    }

    private void DisplayPlayerInventory()
    {
        foreach(var item in _playerInventoryHolder.PrimaryInventorySystem.GetAllItemsHeld())
        {
            var tempSlot = new ShopSlot();
            tempSlot.AssignItem(item.Key, item.Value);

            var shopSlot = Instantiate(_shopSlotPrefab, _itemListContentPanel.transform);
            shopSlot.Init(tempSlot, _shopSystem.SellMarkUp);
        }
    }

    public void RemoveItemFromCart(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;
        var price = GetModifiedPrice(data, 1, shopSlotUI.MarkUp);

        if (_shoppingCart.ContainsKey(data))
        {
            _shoppingCart[data]--;
            var newString = $"{data.DisplayName} ({price}G) x{_shoppingCart[data]}";
            _shoppingCartUI[data].SetItemText(newString);

            if (_shoppingCart[data] <= 0)
            {
                _shoppingCart.Remove(data);
                var tempObj = _shoppingCartUI[data].gameObject;
                _shoppingCartUI.Remove(data);
                Destroy(tempObj);
            }
        }

        _basketTotal -= price;
        _basketTotalText.text = $"Total: {_basketTotal}G";

        if(_basketTotal <= 0 && _basketTotalText.IsActive())
        {
            _basketTotalText.enabled = false;
            _buyButton.gameObject.SetActive(false);
            ClearItemPreview();
            return;
        }

        CheckCartVsAvailableGold();
    }

    private void ClearItemPreview()
    {
        _itemPreviewSprite.sprite = null;
        _itemPreviewSprite.color = Color.clear;
        _itemPreviewName.text = "";
        _itemPreviewDescription.text = "";
    }

    public void AddItemToCart(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;

        UpdateItemPreview(shopSlotUI);

        var price = GetModifiedPrice(data, 1, shopSlotUI.MarkUp);

        if (_shoppingCart.ContainsKey(data))
        {
            _shoppingCart[data]++;
            var newString = $"{data.DisplayName} ({price}G) x{_shoppingCart[data]}";
            _shoppingCartUI[data].SetItemText(newString);
        }
        else
        {
            _shoppingCart.Add(data, 1);

            var shoppingCartTextObj = Instantiate(_shoppingCartItemPrefab, _shoppingCartContentPanel.transform);
            var newString = $"{data.DisplayName} ({price}G) x1";
            shoppingCartTextObj.SetItemText(newString);
            _shoppingCartUI.Add(data, shoppingCartTextObj);
        }

        _basketTotal += price;
        _basketTotalText.text = $"Total: {_basketTotal}G";

        if(_basketTotal > 0 && !_basketTotalText.IsActive())
        {
            _basketTotalText.enabled = true;
            _buyButton.gameObject.SetActive(true);
        }

        CheckCartVsAvailableGold();
    }

    private void CheckCartVsAvailableGold()
    {
        var goldToCheck = _isSelling ? _shopSystem.AvailableGold : _playerInventoryHolder.PrimaryInventorySystem.Gold;

        _basketTotalText.color = _basketTotal > goldToCheck ? Color.red : Color.white;

        if (_isSelling || _playerInventoryHolder.PrimaryInventorySystem.CheckInventoryRemaining(_shoppingCart)) return;

        _basketTotalText.text = "Not enough room in inventory";
        _basketTotalText.color = Color.red;
    }

    public static int GetModifiedPrice(InventoryItemData data, int amount, float markUp)
    {
        var baseValue = data.GoldValue * amount;

        return Mathf.FloorToInt(baseValue + baseValue * markUp);
    }

    private void UpdateItemPreview(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;

        _itemPreviewSprite.sprite = data.Icon;
        _itemPreviewSprite.color = Color.white;
        _itemPreviewName.text = data.DisplayName;
        _itemPreviewDescription.text = data.Description;
    }

    public void OnBuyTabPressed()
    {
        _isSelling = false;
        RefreshDisplay();
    }

    public void OnSellTabPressed()
    {
        _isSelling = true;
        RefreshDisplay();
    }
}

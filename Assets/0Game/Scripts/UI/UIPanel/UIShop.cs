using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIShop : BasePanel
{
    [SerializeField] private GameObject shopParent;
    [SerializeField] private ShopItem _shopItem;

    private List<ShopItem> _shopItems = new ();
    
    [SerializeField] private ScrollRect shopScroll;

    protected override void Awake()
    {
        base.Awake();
        LoadShopData();
    }

    private void LoadShopData()
    {
        var shopData = DataController.Instance.dataShop;
        foreach (var item in shopData.shopItems)
        {
            var itemIns = Instantiate(_shopItem, shopParent.transform);
            itemIns.Init(item);
            _shopItems.Add(itemIns);
        }
    }
    
    public void BuyItem()
    {
        Hide();
    }
    public override void Hide()
    {
        ShowAfterHide(UIController.Instance.uIGarage);
    }

    public void LocalizationUpdate()
    {
        foreach (var item in _shopItems)
        {
            item.LocalizationUpdate();
        }
    }
    
    private void OnDisable()
    {
        shopScroll.verticalNormalizedPosition = 1f;
    }

}

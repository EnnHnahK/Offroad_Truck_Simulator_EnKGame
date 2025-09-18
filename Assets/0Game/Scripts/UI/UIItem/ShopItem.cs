using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
	[SerializeField] private Image itemImage, typeImage;

	[SerializeField] private GameObject iconAds;

	[SerializeField] private Text coinText, itemName, priceText;

	[SerializeField] private Button btnBuy;

	private int _id, _coin;
	
	private DataShop.ShopType _itemType;

	private void Awake()
	{
		btnBuy.onClick.AddListener(Buy);
	}

	public void Init(DataShop.ShopItem item)
	{
		_id = item.id;
		_itemType = item.itemType;

		itemImage.sprite = item.itemImage;
		typeImage.sprite = item.itemTypeImage;
		_coin = item.coin;

		itemName.text = LocalizationManager.GetTranslation(Consts.iap_title + _id);
 		coinText.text = "+ " + Utils.FormatNumber(_coin);


		switch (_itemType)
		{
			case DataShop.ShopType.Reward:
				iconAds.SetActive(true);
				priceText.text = LocalizationManager.GetTranslation(Consts.free);
				break;
			case DataShop.ShopType.IAP:
				priceText.text = item.price + "$";
				break;
		}
	}

	private void IAPItem()
	{
		switch (_id)
		{
			case 1:
				//IAPManager.Instance.IAPBuy_Btn_Pressed(IAPManager.coin1);
				break;
			case 2:
				//IAPManager.Instance.IAPBuy_Btn_Pressed(IAPManager.coin2);
				break;
			case 3:
				//IAPManager.Instance.IAPBuy_Btn_Pressed(IAPManager.coin3);
				break;
		}
	}

	private void Buy()
	{
		//Sound 
		SoundManager.Instance.PlayShot(SoundManager.Instance.click);
		
		UIController.Instance.uIShop.BuyItem();

		#if LOCKED
			UIController.Instance.Notify(Consts.iap_fail);
			return;
		#endif
		
		#if UNITY_EDITOR || CHEAT
			UIController.Instance.uIGarage.IncreaseCoin(_coin);
			return;
		#endif

		switch (_itemType)
		{
			case DataShop.ShopType.Reward:
				//Reward Ads
				/*AdsAdapter.instance.ShowRewardedVideo(
					UIController.Instance.uIGarage.IncreaseCoin(_coin), 
					() => UIController.Instance.Notify(Consts.ads_fail), DataController.CurLevel, 
					AdsAdapter.@where.shop_reward);*/
				break;
			case DataShop.ShopType.IAP:
				//IAP
				IAPItem();
				break;
		}
	}
	
	public void LocalizationUpdate()
	{
		itemName.text = LocalizationManager.GetTranslation(Consts.iap_title + _id);

		if (_itemType is DataShop.ShopType.Reward)
		{
			priceText.text = LocalizationManager.GetTranslation(Consts.free);
		}
	}
}

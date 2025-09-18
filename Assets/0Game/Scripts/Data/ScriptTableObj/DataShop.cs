using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop_Data", menuName = "Data/Shop_Data")]
public class DataShop : ScriptableObject
{
	[System.Serializable]
	public struct ShopItem
	{
		public string name;
		public int id;
		public int coin;
		public ShopType itemType;
		public Sprite itemImage;
		public Sprite itemTypeImage;
		public float price;
	}

	public enum ShopType
	{
		IAP,
		Reward,
		Native
	}

	public List<ShopItem> shopItems = new ();
}

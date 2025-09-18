using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vehicle_Data", menuName = "Data/Vehicle_Data")]
public class DataVehicle : ScriptableObject
{
	[System.Serializable]
	public struct Vehicle
	{
#if UNITY_EDITOR		
		public string name;
#endif
		public int id;
		public VehicleType vehicleType;
		public Sprite vehicleImage;
		public Sprite bgImage;
		public int[] iapDes;
		public float priceIAP;
		public int[] iapExchange; //Index: 1 - Rim || 2 - Wheel || 3 - Engine
		public float BaseGrip;
#if UNITY_EDITOR
		public GameObject vehiclePrefab;
#endif
	}
	
	public enum VehicleType
	{
		IAP,
		Locked,
		NativeAds,
		Exchange,
	}
	
	public List<Vehicle> vehicles = new ();
}

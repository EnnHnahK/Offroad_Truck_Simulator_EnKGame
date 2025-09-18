using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChooseVehicle : BasePanel
{
	[SerializeField] private GameObject vehicleParent;
	[SerializeField] private VehicleItem vehicleItem;

	public List<VehicleItem> vehicleItems = new ();

	private int _indexVehicle, _indexTry;
	
	[SerializeField] private ScrollRect vehicleScroll;
	[SerializeField] private Text[] txtFragments;

	private bool _tryVehicle;

	//[SerializeField] public VehiclesMenu _vehiclesMenu;
	
	private void Start()
	{
		LoadVehicle();
	}

	private void LoadVehicle()
	{
		if(vehicleItems.Count != 0) return;
		
		_indexVehicle = DataController.CurVehicle;
		var dataVehicle = DataController.Instance.dataVehicle.vehicles;
		for (int i = 0; i < dataVehicle.Count; i++)
		{
			var data = dataVehicle[i];
			var vehicle = Instantiate(vehicleItem, vehicleParent.transform);
			vehicle.Init(data);
			vehicleItems.Add(vehicle);
			
			if (data.vehicleType is DataVehicle.VehicleType.IAP or DataVehicle.VehicleType.NativeAds or DataVehicle.VehicleType.Exchange )
			{
				vehicle.transform.SetSiblingIndex(0);
			}
		}
		
	}

	public override void Show()
	{
		base.Show();
		for (int i = 0; i < txtFragments.Length; i++)
		{
			txtFragments[i].text = $"{DataController.Instance.GetCurFragment(i)}";
		}
	}

	public void ChooseVehicle(int id, bool isHide= true)
	{
		if (isHide)
		{
			Hide();
		}

		if (vehicleItems.Count != 0)
		{
			if (_indexVehicle != id)
			{
				vehicleItems[_indexVehicle].Deselected();
			}
		}

		_tryVehicle = false;
		_indexVehicle = id;
		PrefData.VehicleInUse = id;
		VehicleManager.Instance.SpawnTruck(DataController.CurVehicle);
	}

	public void UnlockVehicle(int index)
	{
		LoadVehicle();
		
		vehicleItems[index].Unlocked();
	}

	public void TryVehicle(int id)
	{
		_tryVehicle = true;
		_indexTry = id;
		Hide();
		VehicleManager.Instance.SpawnTruck(DataController.CurVehicle);
	}

	public void EndTry()
	{
		if (!_tryVehicle) return;
		vehicleItems[_indexTry].EndTry();	
		_tryVehicle = false;
		ChooseVehicle(_indexVehicle, false);
	}
	
	public override void Hide()
	{
		ShowAfterHide(UIController.Instance.uIGarage);
	}

	private void OnDisable()
	{
		vehicleScroll.verticalNormalizedPosition = 1f;
	}

}

using System;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
	public static VehicleManager Instance { get; private set; }

	private static Dictionary<int, GameObject> _vehiclesDictionary = new ();
	private static GameObject _chosenTruck;
	private int _vehicleIndex = 0;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		_vehicleIndex = DataController.CurVehicle;
	}

	public void SpawnTruck(int vehicleIndex = -1)
	{
		if (vehicleIndex == -1)
		{
			_vehicleIndex = DataController.CurVehicle;
		}
		else
		{
			_vehicleIndex = vehicleIndex;
		}
		
		if (_chosenTruck)
		{
			Destroy(_chosenTruck);
		}
		if (!_vehiclesDictionary.ContainsKey(_vehicleIndex))
		{
			_vehiclesDictionary.Add(_vehicleIndex, _MainLevelLoader.Instance.LoadTruck(_vehicleIndex));
		}
		//LevelController.Instance.posCar[_vehicleIndex]
		_chosenTruck = Instantiate(_vehiclesDictionary[_vehicleIndex], LevelController.Instance.posSpawn.position, LevelController.Instance.posSpawn.rotation);
		//_chosenTruck.transform.SetPositionAndRotation(LevelController.Instance.posSpawn.position, LevelController.Instance.posSpawn.rotation);
		_chosenTruck.GetComponent<_TruckManager>().SetTruckManagerInstance();
		LoadCompleteVehicle();
	}

	void LoadCompleteVehicle()
	{
		CinemachineManager.Instance.SetupCam();
		UIController.Instance.uIGamePlay.SetEventLose();
		RoadManager.Instance.CalculatorDisRoad();
	}

}
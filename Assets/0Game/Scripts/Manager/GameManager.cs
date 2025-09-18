using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public static int CountPlay;

	[SerializeField] private GameObject tutorial;


	private void Awake()
	{
		Instance = this;

		UtilDev.DebugLogUnity("Application Set Frame Rate = 60 in " + name);
		UtilDev.DebugLogUnity("Tip: Unpack Prefab trước khi build để tránh bị nặng");
#if UNITY_EDITOR
		//Application.targetFrameRate = 60;
#elif !UNITY_EDITOR
		Application.targetFrameRate = 60;
#endif
		

		if (PrefData.FirstOpenGame)
		{
#if CHEAT
			Cheat();
			return;
#endif

			FirstOpenGame();
		}
	}

	private void Start()
	{
		if (DataController.IncompleteTutorial)
		{
			tutorial.SetActive(true);
		}
		CheckEndDay();
	}

	private void FirstOpenGame()
	{
		PrefData.FirstOpenGame = false;
		PrefData.PlayerCoin = 4000;
			
		//Vehicle id 4 is First Vehicle;
		PrefData.SetVehicleOwned(0);
		PrefData.VehicleInUse = 0;
		PrefData.SetTimeReward(DateTime.Now);
	}
	
	private void Cheat()
	{
		PrefData.MapLevel = 9;
		PrefData.FirstOpenGame = false;
		PrefData.PlayerCoin = 4000;
		
		PrefData.SetVehicleOwned(0);
		PrefData.VehicleInUse = 0;
		PrefData.SetTimeReward(DateTime.Now);
	}

	private void CheckEndDay()
	{
		int dayInReal = DateTime.Now.Day;
		if (dayInReal != PrefData.DayInReal)
		{
			PrefData.DayInReal = dayInReal;
			DailyMissionManager.Instance.GenerateDailyMission();
		}
		else
		{
			DailyMissionManager.Instance.GetGeneratedDailyMission();	
		}
	}
}

using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataController : MonoBehaviour
{
	public static DataController Instance;
	
	public static bool Music;
	public static bool Sound;
	public static bool Quality;
	public static bool Vibrate;
	public static bool FirstOpenGame;
	public static bool IncompleteTutorial;

	public static int LevelBonus;
	public static int MapLevel;
	public static int PlayerCoin;
	public static int CurLevel;
	public static int RewardVehicle;
	public static int CurVehicle;
	public static int StepTutorial;
	
	//Size
	public static int CountLevel;

	public DataVehicle dataVehicle;
	public DataDailyMission dataDailyMission;
	public DataLevel dataLevel;
	public DataShop dataShop;
	
	private int[] _curFragments = {3};
	
	private void Awake()
	{
		Instance = this;
		Music = PrefData.Music;
		Sound = PrefData.Sound;
		Vibrate = PrefData.Vibrate;
		FirstOpenGame = PrefData.FirstOpenGame;
		Quality = PrefData.Quality;
		
		LevelBonus = PrefData.LevelBonus;
		MapLevel = PrefData.MapLevel;
		CurLevel = PrefData.CurLevel;
		PlayerCoin = PrefData.PlayerCoin;
		CurVehicle = PrefData.VehicleInUse;
		IncompleteTutorial = PrefData.IncompleteTutorial;
		StepTutorial = PrefData.StepTutorial;
		RewardVehicle = PrefData.RewardVehicleIndex;
		
		CountLevel = dataLevel.maxLv;

		_curFragments = GetTotalFragments();
		
	}
	
	public float GetCurLevelBaseFuel()
	{
		return dataLevel.levels[CurLevel].baseFuel;
	}

	#region mission
	
	public int[] GetRewardLevel()
	{
		return dataLevel.levels[CurLevel].fragmentsReward;
	}
	
	private static int[] GetTotalFragments()
	{
		var array = new int[3];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = PrefData.GetFragments(i);
		}
		return array;
	}

	public int GetCurFragment(int index)
	{
		return _curFragments[index];
	}
	public void SetCurFragments(int index, int value)
	{
		_curFragments[index] = value;
	}

	
	#endregion

}

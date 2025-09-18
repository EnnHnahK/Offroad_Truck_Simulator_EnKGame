using System;
using UnityEngine;

public static class PrefData 
{
	#region Begin
	
	public static bool FirstOpenGame
	{
		get => PlayerPrefs.GetInt("first_open_game", 1) == 1;
		set
		{
			DataController.FirstOpenGame = value;
			PlayerPrefs.SetInt("first_open_game", value ? 1 : 0);
		}
	}	
	
	#endregion
	
	#region Tutorial
	
	public static bool IncompleteTutorial
	{
		get
		{
			return PlayerPrefs.GetInt("incomplete_tutorial", 1) == 1;
		}
		set
		{
			DataController.IncompleteTutorial = value;
			PlayerPrefs.SetInt("incomplete_tutorial", value ? 1 : 0);
		}
	}

	public static int StepTutorial
	{
		get
		{
			return PlayerPrefs.GetInt("step_tutorial", 0);
		}
		set
		{
			DataController.StepTutorial = value;
			PlayerPrefs.SetInt("step_tutorial", value);
		}
	}


	#endregion	
	
	#region Setting

	public static bool Quality
	{
		get => PlayerPrefs.GetInt("quality", 1) == 1;
		set
		{
			PlayerPrefs.SetInt("quality", value ? 1 : 0);
			DataController.Quality = value;
		}
	}
	
	public static bool Vibrate
	{
		get => PlayerPrefs.GetInt("vibrate", 1) == 1;
		set
		{
			PlayerPrefs.SetInt("vibrate", value ? 1 : 0);
			DataController.Vibrate = value;
		}
	}
	
	
	public static bool Music
	{
		get => PlayerPrefs.GetInt("music", 1) == 1;
		set
		{
			PlayerPrefs.SetInt("music", value ? 1 : 0);
			DataController.Music = value;
		}
	}
	
	public static bool Sound
	{
		get => PlayerPrefs.GetInt("sound", 1) == 1;
		set
		{
			PlayerPrefs.SetInt("sound", value ? 1 : 0);
			DataController.Sound = value;
		}
	}
	
	public static int LanguageIndex
	{
		get => PlayerPrefs.GetInt("language", -1);
		set => PlayerPrefs.SetInt("language", value);
	}
	
	public static bool RateShowed
	{
		get => PlayerPrefs.GetInt("rate_showed", 0) == 1;
		set => PlayerPrefs.SetInt("rate_showed", value ? 1 : 0);
	}

	public static bool AllIAPOwned
	{
		get => PlayerPrefs.GetInt("all_iap_owned", 0) == 1;
		set => PlayerPrefs.SetInt("all_iap_owned", value ? 1 : 0);
	}


	#endregion
	
	#region Vehicle

	public static int VehicleInUse
	{
		get => PlayerPrefs.GetInt("CurrentTruck_", 0);
		set
		{
			DataController.CurVehicle = value;
			PlayerPrefs.SetInt("CurrentTruck_", value);
		}
	}

	public static bool GetVehicleOwned(int index)
	{
		return PlayerPrefs.GetInt("vehicle_" + index + "_owned", 0) != 0;
	}

	public static void SetVehicleOwned(int index)
	{
		PlayerPrefs.SetInt("vehicle_" + index + "_owned", 1);
	}

    #endregion
	
	#region Gameplay

	public static int PlayerCoin
	{
		get => PlayerPrefs.GetInt("Money", 2000);
		set
		{
			DataController.PlayerCoin = value;
			PlayerPrefs.SetInt("Money", value);
		}
	}

	public static int MapLevel
	{
		get => PlayerPrefs.GetInt("map_level", 0);
		set
		{
			DataController.MapLevel = value;
			PlayerPrefs.SetInt("map_level", value);
		}
	}

	public static int CurLevel
	{
		get => PlayerPrefs.GetInt("CurrentLevel_", 0);
		set
		{
			DataController.CurLevel = value;
			PlayerPrefs.SetInt("CurrentLevel_", value);
		}
	}

	public static int RewardVehicleIndex
	{
		get => PlayerPrefs.GetInt("reward_vehicle", 1);
		set
		{
			DataController.RewardVehicle = value;
			PlayerPrefs.SetInt("reward_vehicle", value);
		}
	}
	
	public static int GetLevelFuel(int raceID)
	{
		return PlayerPrefs.GetInt($"level_{raceID}_fuel", 1);
	}
	
	public static void SetLevelFuel(int raceID, int value)
	{
		PlayerPrefs.SetInt($"level_{raceID}_fuel", value);
	}
	
	public static int GetLevelGrip(int raceID)
	{
		return PlayerPrefs.GetInt($"level_{raceID}_grip", 1);
	}

	public static void SetLevelGrip(int raceID, int value)
	{
		PlayerPrefs.SetInt($"level_{raceID}_grip", value);
	}

	public static int LevelBonus
	{
		get => PlayerPrefs.GetInt("level_bonus", 1);
		set
		{
			DataController.LevelBonus = value;
			PlayerPrefs.SetInt("level_bonus", value);
		}
	}

	#endregion
	
	public static int DayInReal
	{
		get => PlayerPrefs.GetInt("day_in_real");
		set => PlayerPrefs.SetInt("day_in_real", value);
	}
	
	public static void SetTimeReward(DateTime dateTime)
	{
		PlayerPrefs.SetString("reward_time", dateTime.Ticks.ToString());
	}
	
	public static string GetTimeReward()
	{
		return PlayerPrefs.GetString("reward_time", "");
	}

	
	#region Daily Mission
	
	public static bool GetFirstLv(int idLevel)
	{
		return PlayerPrefs.GetInt("first_lv_" + idLevel, 1) == 1;
	}
	
	public static void SetFirstLv(int id, bool value)
	{
		PlayerPrefs.SetInt("first_lv_" + id, value ? 1 : 0);
	}

	public static int GetFragments(int id)
	{
		return PlayerPrefs.GetInt("fragment_" + id, 0);
	}
	
	public static void SetFragment(int id, int value)
	{
		PlayerPrefs.SetInt("fragment_" + id, value);
		DataController.Instance.SetCurFragments(id, value);
	}
	
	public static int GetProgressMission(int id)
	{
		return PlayerPrefs.GetInt("progress_mis_" + id, 0);
	}
	
	
	/// <param name="value">-1 la da nhan roi, >= 0 chua nhan</param>
	public static void SetProgressMission(int id, int value)
	{
		PlayerPrefs.SetInt("progress_mis_" + id, value);
	}
	
	
	public static string SaveDailyMission
	{
		get => PlayerPrefs.GetString("mis_list_index_", "");
		set => PlayerPrefs.SetString("mis_list_index_", value);
	}
	
	#endregion
	
	#region RemoteConfig


	#endregion
}

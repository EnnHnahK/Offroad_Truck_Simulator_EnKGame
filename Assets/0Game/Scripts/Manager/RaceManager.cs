using System;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
	public static RaceManager Instance;
	public enum RaceStatus
	{
		Running,
		Finish,
		End
	}
	
	public RaceStatus raceStatus = RaceStatus.End;

	private void Awake()
	{
		Instance = this;
	}

	public void StartRace()
	{
		RoadManager.Instance.FirstDraw();
		raceStatus = RaceStatus.Running;
	}
	
	public void EndRace()
    {
    	raceStatus = RaceStatus.End;
    }

	private void ResetRace()
    {
    	
    }

    public void RaceFinish()
    {
    	raceStatus = RaceStatus.Finish;

        if (DataController.CurLevel is 0 && !PrefData.GetVehicleOwned(1))
        {
	        UIController.Instance.uIGarage.RewardedVehicle(true);
        }
        
        if (DataController.CurLevel is 2 && !PrefData.GetVehicleOwned(2))
        {
	        UIController.Instance.uIGarage.RewardedVehicle(true);
        }
        
        _MainLevelLoader.Instance.LoadNextLevel();
    }

    public bool RaceRunning()
    {
	    return raceStatus == RaceStatus.Running;
    }
}

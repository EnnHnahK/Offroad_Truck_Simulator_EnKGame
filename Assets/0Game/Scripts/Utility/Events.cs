using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Events
{
    public delegate void OnOneMinutePassEvent();
    public static OnOneMinutePassEvent OnOneMinutePass;


    public delegate void OnEnableDailyMissionPanelEvent();
    public static OnEnableDailyMissionPanelEvent OnEnableDailyMissionPanel;

    public delegate void OnCompleteDailyMissionEvent(int MissionID);
    public static OnCompleteDailyMissionEvent OnCompleteDailyMission;

    public delegate void OnClaimDailyMissionEvent(int MissionID);
    public static OnClaimDailyMissionEvent OnClaimDailyMission;

    public delegate void OnCompleteLevelEvent(bool complete);
    public static OnCompleteLevelEvent OnCompleteLevel;

    public delegate void OnCompleteIncomeMissionEvent(TransportType transportType);
    public static OnCompleteIncomeMissionEvent OnCompleteIncomeMission;

    public delegate void OnReachMeterEvent(int value);
    public static OnReachMeterEvent OnReachMeter;

    public delegate void OnEarnCoinEvent(int value);
    public static OnEarnCoinEvent OnEarnCoin;

    public delegate void OnCollectObjectEvent(string objectTag, int value);
    public static OnCollectObjectEvent OnCollectObject;

    public delegate void OnUpgradeVehicleEvent(int upgrade_type, int value);
    public static OnUpgradeVehicleEvent OnUpgradeVehicle;

}

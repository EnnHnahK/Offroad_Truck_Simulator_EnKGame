using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMReachorEarnHandle : DailyMissionHandleBase
{
    public enum EarnOrReachType
    {
        ReachMeter, EarnCoin, Upgrade, CollectObject
    }

    [SerializeField] EarnOrReachType type;
    [SerializeField] int UpgradeVehicleID;
    [SerializeField] string ObjectTag;

    protected override void OnDisableHandle()
    {
    }

    protected override void OnEnableHandle()
    {
    }

    protected override void OnInitHandle()
    {
        switch (type)
        {
            case EarnOrReachType.ReachMeter:
                Events.OnReachMeter += UpdateMissionProgress;
                break;
            case EarnOrReachType.EarnCoin:
                Events.OnEarnCoin += UpdateMissionProgress;
                break;
            case EarnOrReachType.Upgrade:
                Events.OnUpgradeVehicle += UpdateMissionProgress;
                break;
            case EarnOrReachType.CollectObject:
                Events.OnCollectObject += UpdateMissionProgress;
                break;
        }
    }

    protected override void OnTerminateHandle()
    {
        switch (type)
        {
            case EarnOrReachType.ReachMeter:
                Events.OnReachMeter -= UpdateMissionProgress;
                break;
            case EarnOrReachType.EarnCoin:
                Events.OnEarnCoin -= UpdateMissionProgress;
                break;
            case EarnOrReachType.Upgrade:
                Events.OnUpgradeVehicle -= UpdateMissionProgress;
                break;
            case EarnOrReachType.CollectObject:
                Events.OnCollectObject -= UpdateMissionProgress;
                break;
        }
    }

    private void UpdateMissionProgress(int value)
    {
        dataItem.dataDaily.AddCurProgress(value);
    }
    private void UpdateMissionProgress(int upgrade_type, int value)
    {
        if(upgrade_type == UpgradeVehicleID)
            dataItem.dataDaily.AddCurProgress(value);
    }
    private void UpdateMissionProgress(string obj_tag, int value)
    {
        if(ObjectTag == obj_tag)
            dataItem.dataDaily.AddCurProgress(value);
    }

    
}

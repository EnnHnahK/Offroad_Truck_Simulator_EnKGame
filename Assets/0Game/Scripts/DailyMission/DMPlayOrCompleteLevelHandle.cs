using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMPlayOrCompleteLevelHandle : DailyMissionHandleBase
{
    [SerializeField] int requiredMeter = 500;
    [SerializeField] int VehicleIDRequired = -1;

    protected override void OnDisableHandle()
    {
    }

    protected override void OnEnableHandle()
    {
    }

    protected override void OnInitHandle()
    {
        Events.OnCompleteLevel += UpdateMissionProgess;
    }

    protected override void OnTerminateHandle()
    {
        Events.OnCompleteLevel -= UpdateMissionProgess;
    }

    private void UpdateMissionProgess(bool complete)
    {
        if (dataItem.dataDaily.Complete())
            return;

        if (!complete && requiredMeter > 0)
        {
            var _roadProcess = RoadManager.Instance.GetRoadProgress();
            var _fullRoad = RoadManager.Instance.GetRoadLength();
            var _distance = (int)_fullRoad * _roadProcess;
            if (_distance > requiredMeter)
                dataItem.dataDaily.AddCurProgress(1);
        }
        else if (complete)
        {
            if(VehicleIDRequired > 0)
            {
                if(DataController.CurVehicle == VehicleIDRequired)
                    dataItem.dataDaily.AddCurProgress(1);
            }
            else 
                dataItem.dataDaily.AddCurProgress(1);
        }
    }
}

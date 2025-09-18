using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMTotalCompleteHandle : DailyMissionHandleBase
{
    protected override void OnDisableHandle()
    {
    }

    protected override void OnEnableHandle()
    {
    }

    protected override void OnInitHandle()
    {
        Events.OnEnableDailyMissionPanel += UpdateTotalMissionComplete;
    }

    protected override void OnTerminateHandle()
    {
        Events.OnEnableDailyMissionPanel -= UpdateTotalMissionComplete;
    }

    private void UpdateTotalMissionComplete()
    {
        if (dataItem.dataDaily.Complete())
            return;

        int count = 0;
        foreach (var item in DailyMissionManager.Instance.generatedDailyMission)
        {
            if (item.Complete())
                count++;
        }
        dataItem.dataDaily.SetCurProgress(Mathf.Min(count,dataItem.dataDaily.max_progress));
    }
}

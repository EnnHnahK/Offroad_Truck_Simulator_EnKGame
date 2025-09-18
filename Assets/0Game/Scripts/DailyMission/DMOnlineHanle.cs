using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMOnlineHanle : DailyMissionHandleBase
{

    protected override void OnInitHandle()
    {
        Events.OnOneMinutePass += UpdateMissionOnl;
    }

    protected override void OnTerminateHandle()
    {
        Events.OnOneMinutePass -= UpdateMissionOnl;
    }

    protected override void OnEnableHandle()
    {
        Events.OnOneMinutePass += dataItem.UpdateDailyMissionProgress;
    }
    protected override void OnDisableHandle()
    {
        Events.OnOneMinutePass -= dataItem.UpdateDailyMissionProgress;
    }

    private void UpdateMissionOnl()
    {
        dataItem.dataDaily.AddCurProgress(1);
    }

}

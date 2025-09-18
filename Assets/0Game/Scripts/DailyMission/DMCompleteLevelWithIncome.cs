using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMCompleteLevelWithIncome : DailyMissionHandleBase
{
    [SerializeField] bool NeedRequiredMission = false;
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

        if (!MissionManager.Instance)
            return;

        if ((complete && !NeedRequiredMission && MissionManager.Instance.MissionStatus == MissionStatus.None) 
            || (complete && NeedRequiredMission && MissionManager.Instance.MissionStatus == MissionStatus.Finished))
            dataItem.dataDaily.AddCurProgress(1);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMCompleteIncomeHandle : DailyMissionHandleBase
{
    [SerializeField] bool NeedRequiredMission = false;
    [SerializeField] TransportType transportTypeRequired;

    protected override void OnDisableHandle()
    {
    }

    protected override void OnEnableHandle()
    {
    }

    protected override void OnInitHandle()
    {
        Events.OnCompleteIncomeMission += UpdateProgressMission;
    }

    protected override void OnTerminateHandle()
    {
        Events.OnCompleteIncomeMission -= UpdateProgressMission;
    }

    private void UpdateProgressMission(TransportType transportType)
    {
        if (!NeedRequiredMission)
            dataItem.dataDaily.AddCurProgress(1);

        else if (transportTypeRequired == transportType)
            dataItem.dataDaily.AddCurProgress(1);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DailyMissionHandleBase : MonoBehaviour
{
    protected DailyMissionItem dataItem;

    protected abstract void OnInitHandle();
    protected abstract void OnTerminateHandle();

    protected abstract void OnEnableHandle();
    protected abstract void OnDisableHandle();

    private void OnEnable()
    {
        UpdateUIProgress();
        OnEnableHandle();
    }

    private void OnDisable()
    {
        OnDisableHandle();
    }

    protected void UpdateUIProgress()
    {
        dataItem.UpdateDailyMissionProgress();
    }

    public void Init(DailyMissionItem data)
    {
        this.dataItem = data;
        OnInitHandle();
        Events.OnCompleteDailyMission += BindingToTerminate;
    }

    private void BindingToTerminate(int ID)
    {
        if(ID == dataItem.dataDaily.ID)
            OnTerminateHandle();
    }

    public void Terminate()
    {
        Events.OnCompleteDailyMission -= BindingToTerminate;
        gameObject.SetActive(false);
    }

}

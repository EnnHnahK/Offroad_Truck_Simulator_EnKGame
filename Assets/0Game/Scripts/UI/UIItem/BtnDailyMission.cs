using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnDailyMission : MonoBehaviour
{
    [SerializeField] GameObject noti;
    private void OnEnable()
    {
        CheckNotify();
        Events.OnCompleteDailyMission += CheckNotify;
        Events.OnClaimDailyMission += CheckNotify;
    }

    private void OnDisable()
    {
        Events.OnCompleteDailyMission -= CheckNotify;
        Events.OnClaimDailyMission -= CheckNotify;
    }

    private void CheckNotify(int ID = 0)
    {
        var list = UIController.Instance.uIDailyMission.dailyMissionItems;
        foreach (var item in list)
        {
            if (item.dataDaily.Complete())
            {
                noti.SetActive(true);
                return;
            }
        }
        noti.SetActive(false);
    }
}

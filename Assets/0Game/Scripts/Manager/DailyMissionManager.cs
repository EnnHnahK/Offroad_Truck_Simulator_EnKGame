using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyMissionManager : MonoBehaviour
{
    public static DailyMissionManager Instance;

    public List<DataDailyMission.DailyMissionTask> generatedDailyMission;

    private void Awake()
    {
        Instance = this;
        StartCoroutine(IE_Timer());
    }

    private IEnumerator IE_Timer()
    {
        int min = 61;
        while (min > 0)
        {
            yield return new WaitForSecondsRealtime(60);
            min--;
            Events.OnOneMinutePass?.Invoke();
        }

    }

    public void GenerateDailyMission()
    {
        //Generate
        var rotationTasks = new List<DataDailyMission.DailyMissionTask>();
        foreach (var item in DataController.Instance.dataDailyMission.DailyTasks)
        {
            if (item.typeDailyMission == TypeDailyMission.Rotation)
                rotationTasks.Add(item);
            else
                generatedDailyMission.Add(item);
        }
        Utils.Shuffle(rotationTasks);

        var sizedRotationTask = rotationTasks.GetRange(0, Mathf.Min(rotationTasks.Count, 5));

#if UNITY_EDITOR || CHEAT
        sizedRotationTask = rotationTasks;
#endif
        generatedDailyMission.AddRange(sizedRotationTask);

        // save for today
        string listID = "";
        for (int i = 0; i < generatedDailyMission.Count; i++)
        {
            var generated = generatedDailyMission[i];
            generated.Reset();
            if (i == generatedDailyMission.Count - 1)
            {
                listID += generatedDailyMission.IndexOf(generated);
            }
            else
            {
                listID += generatedDailyMission.IndexOf(generated) + "_";
            }
        }

        DataController.Instance.dataDailyMission.todayDailyMission = listID;
        UIController.Instance.uIDailyMission.LoadDailyMission();
    }

    public void GetGeneratedDailyMission()
    {
        //Get from dataItem
        var listDailyMission = DataController.Instance.dataDailyMission.todayDailyMission.Split('_');
        for (int i = 0; i < listDailyMission.Length; i++)
        {
            if (int.TryParse(listDailyMission[i], out int index))
            {
                var item = DataController.Instance.dataDailyMission.DailyTasks[index];
                generatedDailyMission.Add(item);
            }
        }
        UIController.Instance.uIDailyMission.LoadDailyMission();

    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeDailyMission
{
    Online,
    Permanent,
    Rotation,
}


[CreateAssetMenu(fileName = "Data_DailyMission", menuName = "Data/Data_DailyMission")]
public class DataDailyMission : ScriptableObject
{
    [System.Serializable]
    public class DailyMissionTask
    {
        public int ID;
        public TypeDailyMission typeDailyMission;
        public string TaskName;
        public int max_progress = 3;
        public int[] fragmentsReward = new int[3];
        public DailyMissionHandleBase DailyMissionHanldePrefab;

        public int GetCurProgress()
        {
            return PrefData.GetProgressMission(ID);
        }

        public void AddCurProgress(int value)
        {
            if (Received())
            {
                return;
            }
            var total = GetCurProgress() + value;
            if (total <= max_progress)
            {
                PrefData.SetProgressMission(ID, total);
                if(total == max_progress)
                    Events.OnCompleteDailyMission?.Invoke(ID);
            }
        }
        public void SetCurProgress(int value)
        {
            if (Received())
            {
                return;
            }
            if (value <= max_progress && value != GetCurProgress())
            {
                PrefData.SetProgressMission(ID, value);
                if (value == max_progress)
                    Events.OnCompleteDailyMission?.Invoke(ID);
            }
        }

        public bool Complete()
        {
            return PrefData.GetProgressMission(ID) >= max_progress;
        }

        public bool Received()
        {
            return PrefData.GetProgressMission(ID) < 0;
        }
        
        public void Claim()
        {
            PrefData.SetProgressMission(ID, -1);
        }

        public void Reset()
        {
            PrefData.SetProgressMission(ID,0);
        }
    }

    

    public List<DailyMissionTask> DailyTasks;

    public string todayDailyMission
    {
        get => PrefData.SaveDailyMission; 
        set => PrefData.SaveDailyMission = value;
    }


#if UNITY_EDITOR

    [SerializeField] private bool init;
    private void OnValidate()
    {
        if (init)
        {
            init = false;
            int i = 0;
            foreach (var task in DailyTasks)
            {
                task.ID = i;
                if (task.max_progress == 0)
                    task.max_progress = 1;
                i++;
            }
        }
    }
#endif
}


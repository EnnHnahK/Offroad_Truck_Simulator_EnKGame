using UnityEngine;
using UnityEngine.UI;

public class DailyMissionItem : MonoBehaviour
{
    [SerializeField] Text MissionTitle;
    [SerializeField] Image star;
    [SerializeField] GameObject ClaimLable;
    [SerializeField] GameObject GoLable;

    [SerializeField] Text[] FragmentRewards;

    public DataDailyMission.DailyMissionTask dataDaily;
    private DailyMissionHandleBase handle;

    private void Awake()
    {
        GetComponent<ButtonEffectLogic>().onClick.AddListener(OnClick);
    }

    public void Init(DataDailyMission.DailyMissionTask data)
    {
        dataDaily = data;
        MissionTitle.text = $"{data.TaskName} ({data.GetCurProgress()}/{data.max_progress})";
        for (int i = 0; i < FragmentRewards.Length; i++)
        {
            FragmentRewards[i].transform.parent.gameObject.SetActive(data.fragmentsReward[i] != 0);
            FragmentRewards[i].text = "x" + data.fragmentsReward[i];
        }
        //Generate prefab
        handle = Instantiate(data.DailyMissionHanldePrefab, transform);
        handle.Init(this);
        //Update Visualize
        UpdateDailyMissionProgress();
    }

    [Button]
    public void UpdateDailyMissionProgress()
    {
        if (gameObject.activeInHierarchy)
        {
            MissionTitle.text = $"{dataDaily.TaskName} ({dataDaily.GetCurProgress()}/{dataDaily.max_progress})";
            ClaimLable.SetActive(dataDaily.Complete());
            GoLable.SetActive(!dataDaily.Complete());
            if (dataDaily.Complete() && !star.color.Equals(Color.white))
            {
                UIController.Instance.uIDailyMission.EffectComplete(star);
            }
        }
    }

    private void OnClick()
    {
        if (dataDaily.GetCurProgress() == dataDaily.max_progress)
        {
            //Claming Reward
            dataDaily.Claim();
            Events.OnClaimDailyMission?.Invoke(dataDaily.ID);
            for (int i = 0; i < dataDaily.fragmentsReward.Length; i++)
            {
                PrefData.SetFragment(i, DataController.Instance.GetCurFragment(i) + dataDaily.fragmentsReward[i]);
            }
            UIController.Instance.uIDailyMission.ClaimDailyMission(this);
            handle.Terminate();
        }
        else
        {
            //Back To Menu
            UIController.Instance.uIDailyMission.ShowAfterHide(UIController.Instance.uIGarage);
        }
    }

    public TypeDailyMission GetTypeDailyMission()
    {
        return dataDaily.typeDailyMission;
    }
}
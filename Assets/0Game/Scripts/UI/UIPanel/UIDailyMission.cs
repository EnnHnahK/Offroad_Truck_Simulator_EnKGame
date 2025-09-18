using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyMission : BasePanel
{
    public bool CanInteract => dailyMissionParent.enabled;
    public List<DailyMissionItem> dailyMissionItems = new();
    
    [SerializeField] private ScrollRect dailyMissionScroll;
    [SerializeField] private VerticalLayoutGroup dailyMissionParent;
    [SerializeField] private DailyMissionItem dailyMissionItem;

    [SerializeField] private Transform posStart;
    [SerializeField] private Transform[] iconStars;
    [SerializeField] private GameObject[] parStars;
    private int amountEffect;


    private List<DataDailyMission.DailyMissionTask> _generatedDailyMission => DailyMissionManager.Instance.generatedDailyMission;

    private Queue<int> queueIndexPar = new Queue<int>(5);

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < iconStars.Length; i++)
        {
            iconStars[i].position = posStart.position;
            queueIndexPar.Enqueue(i);
        }
    }

    private void OnEnable()
    {
        Events.OnEnableDailyMissionPanel?.Invoke();
    }


    public void LoadDailyMission()
    {
        if (dailyMissionItems.Count != 0) return;
        
        foreach (var daily in _generatedDailyMission)
        {
            if (daily.Received())
                continue;
            var dailyTask = Instantiate(dailyMissionItem, dailyMissionParent.transform);
            dailyTask.Init(daily);            
            dailyMissionItems.Add(dailyTask);
        }
    }

    public void EffectComplete(Image star)
    {
        if (queueIndexPar.Count <= 0)
        {
            return;
        }

        DOVirtual.DelayedCall(showEffects[0].timeShow + amountEffect / 2f, () =>
        {
            amountEffect--;
            int i = queueIndexPar.Dequeue();
            Transform iconStar = iconStars[i];
            iconStar.DORotate(Vector3.forward * 720, .5f).SetRelative(true).SetEase(Ease.InOutCubic);
            Vector3[] path = {CentroidTriangle(star.transform.position), star.transform.position};
            iconStar.DOPath(path, .5f, PathType.CatmullRom).SetEase(Ease.InOutCubic).OnComplete(() =>
            {
                var parStar = parStars[i];
                star.color = Color.white;
                parStar.transform.position = star.transform.position;
                parStar.SetActive(true);
                iconStar.position = posStart.position;
                DOVirtual.DelayedCall(1, () =>
                {
                    queueIndexPar.Enqueue(i);
                    parStar.SetActive(false);
                });
            });
        });
        amountEffect++;
    }

    // Minh hoa: lấy trong tam
    //     ----------------------posStart
    //    |
    //    |
    //   pos
    Vector3 CentroidTriangle(Vector2 pos)
    {
        Vector2 posTop = new Vector2(pos.x, posStart.position.y);
        return new Vector2((pos.x + posTop.x + posStart.position.x) / 3, (pos.y + posTop.y + posStart.position.y) / 3);
    }

    public override void Hide()
    {
        ShowAfterHide(UIController.Instance.uIGarage);
    }

    private void OnDisable()
    {
        dailyMissionScroll.verticalNormalizedPosition = 1f;
	}

    public void ClaimDailyMission(DailyMissionItem missionItem)
    {
        dailyMissionParent.enabled = false;
        int index = dailyMissionItems.IndexOf(missionItem);

        var remainList = dailyMissionItems.GetRange(index, dailyMissionItems.Count - index);

        var distance = remainList[0].rect().rect.width + dailyMissionParent.spacing + dailyMissionParent.padding.right + dailyMissionParent.padding.left;
        remainList[0].rect().DOAnchorPosX(-distance, .5f).SetRelative(true).OnComplete(() =>
        {
            for (int i = 1; i < remainList.Count; i++)
            {
                var remain = remainList[i];
                var childDistance = remain.rect().rect.height + dailyMissionParent.spacing;
                remain.rect().DOAnchorPosY(childDistance, 0.5f).SetRelative(true);
            }
            StartCoroutine(IE_ClaimAnimation(missionItem));
        });
    }
    IEnumerator IE_ClaimAnimation(DailyMissionItem missionItem)
    {
        yield return Yielder.GetWaitForSeconds(.6f);
        missionItem.gameObject.SetActive(false);
        dailyMissionParent.enabled = true;
        dailyMissionItems.Remove(missionItem);
    }
}

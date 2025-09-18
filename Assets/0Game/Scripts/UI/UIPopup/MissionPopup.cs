using System.Collections;
using DG.Tweening;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class MissionPopup : BasePopup
{
    [SerializeField] private ButtonEffectLogic btnGiveup, btnAccept;

    [SerializeField] private Text numItemText, earnText, timeText, notiMissonText, desMissionText;
    
    [SerializeField] private GameObject notiMission;
    [SerializeField] private RectTransform missionContent;

    [SerializeField] private Image iconMission, iconNoti;

    private int _time;

    private bool _missionAccepted;

    public int coinEarn = 1000;

    [SerializeField] private Sprite[] missionSprite;

    private Vector2 _sizeIcon, _sizeNoti;

    private int _typeVehicle = -1;

    protected override void Awake()
    {
        base.Awake();
        btnGiveup.onClick.AddListener(CancelMission);
        btnAccept.onClick.AddListener(AcceptMission);

        _sizeIcon = iconMission.rect().sizeDelta;
        _sizeNoti = iconNoti.rect().sizeDelta;
    }
    

    private void OnEnable()
    {
        Init();
        Time.timeScale = 0.3f;
    }

    public override void Show()
    {
        base.Show();
        DOVirtual.DelayedCall(.35f, () =>
        {
            missionContent.DOAnchorPosY(missionContent.anchoredPosition.y, .7f).From(new Vector2(missionContent.anchoredPosition.x, -111f)).SetUpdate(true).OnStart(() =>
            {
                missionContent.gameObject.SetActive(true);
            });
        });
    }

    private void Init()
    {
        missionContent.gameObject.SetActive(false);
        
        _time = 7;
        _missionAccepted = false;
        notiMissonText.text = coinEarn.ToString();

        var type = _TruckManager.Instance.GetTransportType();
        if (_typeVehicle != type)
        {
            // //Init Icon Item
            // var type = _TruckManager.Instance.GetTransportType();

            var itemCount = Random.Range(1, 4) * 100;
            
            
            desMissionText.text = LocalizationManager.GetTranslation(Consts.mission_des);
            
            if (type is 2 or 9)
            {
                itemCount = 3;
            }
            else if (type == 3)
            {
                itemCount = 10;
                desMissionText.text = LocalizationManager.GetTranslation(Consts.passenger_mission);
            }
            else if (type == 5)
            {
                itemCount = 2;
                desMissionText.text = LocalizationManager.GetTranslation(Consts.firefighting_mission);
            }
            else if (type is 6 or 7 or 8)
            {
                itemCount = 1;
            }
            else if (type is 10)
            {
                itemCount = 5;
            }
            
            numItemText.text = itemCount.ToString();
        
            iconMission.sprite = missionSprite[type];
            Utils.SetNativeImage(iconMission, _sizeIcon);

            iconNoti.sprite = missionSprite[type];
            Utils.SetNativeImage(iconNoti, _sizeIcon);
        }

        StartCoroutine(IETimeMission());
        MissionManager.Instance.ShowDelivery();
    }

    private IEnumerator IETimeMission()
    {
        while (_time > 0)
        {
            _time--;
            timeText.text = _time.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        if(!_missionAccepted) AcceptMission();
    }

    private void CancelMission()
    {
        Hide();
        MissionManager.Instance.MissionCanceled();
    }

    private void AcceptMission()
    {
        _missionAccepted = true;
        MissionManager.Instance.AcceptMission();
        Hide();
    }
    

    public void NotiMission()
    {
        notiMission.SetActive(true);
        DOVirtual.DelayedCall(1.5f, () => notiMission.SetActive(false));
    }

    public void MissionWorking()
    {
        
    }
    
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}

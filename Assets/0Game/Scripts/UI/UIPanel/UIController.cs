using System;
using DG.Tweening;
using NWH.VehiclePhysics2.Input;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public static UIController Instance;

	public UIGarage uIGarage;
	public UIGamePlay uIGamePlay;
	public UISetting uISetting;
	public UIGameCompleted uIGameCompleted;
	public UIChoseLevel uIChoseLevel;
	public UIChooseVehicle uIChooseVehicle;
	public UIDailyMission uIDailyMission;
	public UIShop uIShop;
	public UINotify uINotify;
	public UITutorial uITutorial;
	
	public LoadingScreen loadingScreen;
	
	[Header("Popup")]
	public MissionPopup missionPopup;
	public NoInternetPopup internetPopup;
	public NoAdsPopup noAdsPopup;
	public RatePopup ratePopup;
	public IAPSuggestPopup IAPSuggestPopup;
	public static bool PopShowed;
	
	private bool _rateShowed, _allIAPOwned, _noAdsShowed;
	private int countConstraintEvent = 5;
	
	[Header("Waiting Panel")]
	[SerializeField] private GameObject waitPanel;
	[SerializeField] private Image waitPanelBg;
	[SerializeField] private Transform waitingTextTransform;
	
	[Header("Input")]
	public MobileVehicleInputProvider mobileVehicleInputProvider;
	private void Awake()
	{
		Instance = this;

		_rateShowed = PrefData.RateShowed;
		_allIAPOwned = PrefData.AllIAPOwned;
	}

	public void Start()
	{
		//uIGarage.Show();
		if(DataController.StepTutorial >= 1) LoadingScreen(5f, false, () => uIGarage.Show());
		else
		{
			loadingScreen.SetTimeLoading(5f, false, null);
		}
	}

	private void Update()
	{
		if (internetPopup.isShow) return;
        
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			internetPopup.Show();
		}
	}
	
	public void LoadingScreen(float time, bool effect, Action action)
	{
		loadingScreen.SetTimeLoading(time, effect, action);
	}

	public void CoinFly()
	{
		uINotify.CoinFly(10);
		uIGameCompleted.CollectCoin(10);
	}

	public void Notify(string s)
	{
		uINotify.Notify(s);
	}

	public void NotiFinishRace(int disTraveled)
	{
		uINotify.NotiFinishRace(disTraveled);
	}
	
	public void LocalizationUpdate()
	{
		uIGarage.LocalizationUpdate();
		uIShop.LocalizationUpdate();
	}
	
	public void SetSteeringWheel(NWH.VehiclePhysics2.VehicleGUI.SteeringWheel steeringWheel)
	{
		mobileVehicleInputProvider.steeringWheel = steeringWheel;
	}

	public void SetWorkingInInput(bool isWorking)
	{
		mobileVehicleInputProvider.SetWorking(isWorking);
	}

	public void WaitingPanel(float time = 3f)
	{
		if (!waitPanel.activeSelf)
		{
			var color = waitPanelBg.color;
			waitPanel.SetActive(true);
			waitingTextTransform.DOLocalMoveX(0f, .7f).From(-800f);

			DOVirtual.DelayedCall(time - 0.7f, () =>
			{
				waitingTextTransform.DOLocalMoveX(800f, .7f).From(0);
			});
			waitPanelBg.DOFade(.1f, time).SetEase(Ease.InExpo).OnComplete(() =>
			{
				waitPanel.SetActive(false);
				var pos = waitingTextTransform.localPosition;
				pos.x = 0;
				waitingTextTransform.localPosition = pos;
				color.a = .7f;
				waitPanelBg.color = color;
			});
		}
	}

	public void CallCompletedRaceEvent()
	{
		CallShowRate();
		CallShowIAPSuggest();
		CallShowNoAds();
	}

	private void CallShowRate()
	{
		if(_rateShowed) return;
		PrefData.RateShowed = true;
		_rateShowed = true;
		ratePopup.Show();
		PopShowed = true;
	}
	
	private void CallShowIAPSuggest()
	{
		if(_allIAPOwned) return;
		if (GameManager.CountPlay > countConstraintEvent)
		{
			countConstraintEvent += 10;
			IAPSuggestPopup.Show();
			PopShowed = true;
		}
	}

	private void CallShowNoAds()
	{
		if(_noAdsShowed || GameManager.CountPlay != 3) return;
		_noAdsShowed = true;
		noAdsPopup.Show();
		PopShowed = true;
	}

	public void EventClosePop()
	{
		PopShowed = false;
		DOVirtual.DelayedCall(.5f, uIGarage.Show);
	}

	public void AllIAPOwned()
	{
		_allIAPOwned = true;
	}

	private void OnDisable()
	{
		waitPanelBg.DOKill();
		waitingTextTransform.DOKill();
	}

}

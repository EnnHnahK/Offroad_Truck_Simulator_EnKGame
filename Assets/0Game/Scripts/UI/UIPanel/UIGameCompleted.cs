using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class UIGameCompleted : BasePanel
{
	[SerializeField] private Button btnCoin, btnCollectBonus,  btnNextBonus, btnContinue;

	[SerializeField] private GameObject bonusRotate, racePointer, particleWin, bonusWheel,bonusResult;
	
	[SerializeField] private Text coinBarText, raceDistanceText, rewardText, bonusText,
		highScoreText, coinBonusText, titleText, totalCoinText, bonusResultText;


	[SerializeField] private Image raceFill;

	[SerializeField] private Transform startTransform, endTransform;

	[SerializeField] private ButtonEffect bonusEffect;

	[SerializeField] private ShowEffect coinButton, continueButton;
	
	[SerializeField] private Transform iconFinish;
	[SerializeField] private Transform centerScreen;
	[SerializeField] private GameObject[] fragmentObjects;
	[SerializeField] private Text[] fragmentTexts;
	[SerializeField] private Transform[] iconFragment = new Transform[3];
	
	private Tween _tweenBonus;

	private bool _rewarded;

	private int _coinEarn, _coinBonus, _coinReward, _coinMission;
	
	private readonly float[] _values = { 125, 75, 25, -25, -75, -125 };
	
	private int _bonusIndex = -1;
	
	private Sequence _rotationSequence;

	private float _roadProcess, _fullRoad; 

	private void OnEnable()
	{
		Init();
	}
	
	protected override void Awake()
	{
		base.Awake();
		btnCoin.onClick.AddListener(() =>
		{
			if(!canTarget) return;
			
			//Inter Available

			UIController.Instance.WaitingPanel(3f);
			DOVirtual.DelayedCall(3f, () =>
			{
				coinButton.gameObject.SetActive(false);
				continueButton.Effect();
			});
			
			return;
			
			Continue();
			UIController.Instance.uIGarage.IncreaseCoin(_coinEarn);
		});
		btnCollectBonus.onClick.AddListener(() =>
		{
			if(!canTarget) return;
			CollectBonus();
		});
		btnNextBonus.onClick.AddListener(() =>
		{
			if(!canTarget) return;
			NextBonus();
		});
		
		btnContinue.onClick.AddListener(() =>
		{
			if(!canTarget) return;
			
			Continue();
			UIController.Instance.uIGarage.IncreaseCoin(_coinEarn);
		});
	}
	
	private void Init()
	{
		canTarget = true;
		_rewarded = false;
		
		coinButton.gameObject.SetActive(false);
		continueButton.gameObject.SetActive(false);
		
		bonusWheel.SetActive(true);
		bonusResult.SetActive(false);
		
		_bonusIndex = -1;
		coinBarText.text = Utils.FormatNumber(DataController.PlayerCoin);

		_roadProcess = RoadManager.Instance.GetRoadProgress();
		_fullRoad = RoadManager.Instance.GetRoadLength();

		RaceDisplay();
		BonusWheel();
		
		if (_roadProcess >= 1 && DataController.CurLevel == DataController.MapLevel)
		{
			WinFirstLevel();
		}
		else
		{
			foreach (var item in fragmentObjects)
			{
				item.SetActive(false);
			}
		}

		//Delay Next Button
		DOVirtual.DelayedCall(1f, () =>
		{
			if (!_rewarded) coinButton.Effect();
		});
	}

	private int ProcessingVehicleEarn(int disTraveled)
	{
		switch (DataController.CurVehicle)
		{
			//IAP Vehicle x2 Coin
			case 8:
				disTraveled *= 2;
				break;
			//IAP Vehicle x1.5 Coin
			case 9:
				disTraveled = (int)(disTraveled * 1.5f);
				break;
		}

		return disTraveled + 100;
	}
	
	private void RaceDisplay()
	{
		var disTraveled = (int)(_fullRoad * _roadProcess);
		Events.OnReachMeter?.Invoke(disTraveled);

		_coinEarn += ProcessingVehicleEarn(disTraveled);
		

		if (_roadProcess >= 1)
		{
			titleText.text = LocalizationManager.GetTranslation(Consts.mission_completed);
			particleWin.SetActive(true);
		}
		else
		{
			titleText.text = LocalizationManager.GetTranslation(Consts.try_again);
			particleWin.SetActive(false);
		}
		
		
		//Race Fill
		DOTween.To(() => 0, x => raceDistanceText.text = x + "m", (int)disTraveled, 2f).SetEase(Ease.InOutQuad).OnComplete(() =>
		{
			raceDistanceText.text = Utils.FormatNumber(disTraveled) + "m";
		} );
		PointerEffect(disTraveled / _fullRoad);
		raceFill.DOFillAmount(disTraveled/_fullRoad, 2f).From(0).SetEase(Ease.InOutQuad);
		
		//Set Text
		highScoreText.text = LocalizationManager.GetTranslation(Consts.highScore).Replace("{0}", Utils.FormatNumber((int)disTraveled));
		rewardText.text = "+" + Utils.FormatNumber(_coinEarn);
		
		//Bonus min Lv1 = 23%. Per level + 2%. Bonus depends on distance Traveled
		_coinBonus = (int)(disTraveled * (23 + DataController.LevelBonus * 2)/100) + _coinMission;
		bonusText.text = "+" + Utils.FormatNumber(_coinBonus);	
		
		_coinEarn += _coinBonus;

		totalCoinText.text = Utils.FormatNumber(_coinEarn);
	}

	public void SetCoinMission(int coin)
	{
		_coinMission = coin;
	}

	public void CollectCoin(int coin)
	{
		_coinEarn += coin;
	}

	private void BonusWheel()
	{
		_rotationSequence = DOTween.Sequence();

		_rotationSequence.AppendCallback(() =>
		{
			bonusEffect.Shake();
		}).AppendInterval(2f).AppendCallback(() =>
		{
			bonusEffect.Shake();
		});
		
		_rotationSequence.SetLoops(-1, LoopType.Yoyo);

		StartCoroutine(IECheckRotation());
	}

    private IEnumerator IECheckRotation()
    {
	    while (!_rewarded)
	    {
		    float currentRotation =  bonusRotate.transform.rotation.eulerAngles.z;
		    if (currentRotation > 180) currentRotation -= 360;
		    
		    for (int i = 0; i < _values.Length - 1; i++)
		    {
			    if (currentRotation <= _values[i] && currentRotation >= _values[i + 1] && _bonusIndex != i)
			    {
				    _bonusIndex = i;
				    
				    _coinReward = _coinEarn * (_bonusIndex + 2);
				    coinBonusText.text = Utils.FormatNumber(_coinReward);

				    break;
			    }
		    }
		    yield return Yielder.GetWaitForSeconds(.1f);
	    }
    }
    
    private void Continue()
    {
	    canTarget = false;
	    ShowAfterHide(UIController.Instance.uIGarage, false, 3f);
	    
	    if (DataController.StepTutorial == 2)
	    {
		    UITutorial.Instance.NextStep();
	    }

	    if (!DataController.IncompleteTutorial)
	    {
		    UIController.Instance.CallCompletedRaceEvent();
	    }
	    
		if (_roadProcess >= 1)
		{
			RaceManager.Instance.RaceFinish();
		}
		else
		{
			_MainLevelLoader.Instance.LoadLevel(DataController.CurLevel);
		}
		
		
		
		//Check Try Vehicle 
		UIController.Instance.uIChooseVehicle.EndTry();
		_TruckManager.Instance.GetTruckItemCount().ToggleItem(false);
		//SoundManager.Instance.PlayBg(MusicType.Menu);
    }

    
    private void PointerEffect(float ratio)
    {
	    Vector3 targetPos = Vector3.Lerp(startTransform.localPosition, endTransform.localPosition, ratio);
	    
	    racePointer.transform.DOLocalMoveX(targetPos.x, 2f).From(startTransform.localPosition.x).SetEase(Ease.InOutQuad).OnComplete(() =>
	    {
		    racePointer.transform.DOScale(1.1f, 0.7f).From(0.9f).SetLoops(-1, LoopType.Yoyo);
	    });
    }


	private void CollectBonus()
	{
		
		#if LOCKED
			UIController.Instance.Notify("ADS FAILED");
			return;
		#endif
		
		#if UNITY_EDITOR || CHEAT
			_rewarded = true;
			//UIController.Instance.uIGarage.IncreaseCoin(_coinReward);
			btnContinue.gameObject.SetActive(false);
			bonusResult.SetActive(true);
			bonusWheel.SetActive(false);
			coinButton.gameObject.SetActive(false);
			bonusResultText.text = Utils.FormatNumber(_coinReward);
			return;
		#endif
		
		
		/*AdsAdapter.instance.ShowRewardedVideo(
			UIController.Instance.uIGarage.IncreaseCoin(_coinReward);, 
		() => UIController.Instance.Notify(Consts.ads_fail), DataController.CurLevel, 
		AdsAdapter.@where.bonus_wheel);*/
	}

	private void NextBonus()
	{
		Continue();
		UIController.Instance.uIGarage.IncreaseCoin(_coinReward);
	}

	void WinFirstLevel()
	{
		var dataRw = DataController.Instance.GetRewardLevel();
		for (int i = 0; i < fragmentTexts.Length; i++)
		{
			if (dataRw[i] == 0)
			{
				fragmentObjects[i].SetActive(false);
			}
			else
			{
				fragmentObjects[i].SetActive(true);
				fragmentTexts[i].text = "+0";
			}
		}
		EffectFragments(dataRw);
		CollectFragments(dataRw);
	}

	private void EffectFragments(int[] data)
	{
		var timeDelay = .3f;
		for (int i = 0; i < fragmentTexts.Length; i++)
		{
			if (data[i] != 0)
			{
				StartCoroutine(WaitEffectFragment(i, data[i], timeDelay));
				timeDelay += .7f;
			}
		}
	}

	IEnumerator WaitEffectFragment(int index, int amountRw, float timeDelay = 0)
	{
		if (timeDelay != 0)
		{
			yield return Yielder.GetWaitForSeconds(timeDelay);
		}

		Transform icon = iconFragment[index];
		Transform fragment = fragmentObjects[index].transform;
		icon.gameObject.SetActive(true);
		icon.DOScale(Vector3.one, .3f).From(Vector3.zero);
		icon.DOMove(centerScreen.position, .3f).From(iconFinish.position);
		yield return Yielder.GetWaitForSeconds(.8f);
		icon.DOScale(Vector3.zero, .2f);
		icon.DOMove(fragmentObjects[index].transform.position, .2f);
		yield return Yielder.GetWaitForSeconds(.2f);
		icon.gameObject.SetActive(false);
		fragment.DOScale(Vector3.one * 1.25f, .05f);
		yield return Yielder.GetWaitForSeconds(.05f);
		fragmentTexts[index].text = "+" + amountRw;
		fragment.DOScale(Vector3.one, .05f);
	}
	
	private void CollectFragments(int[] data)
	{
		for (int i = 0; i < fragmentTexts.Length; i++)
		{
			PrefData.SetFragment(i, DataController.Instance.GetCurFragment(i) + data[i]);
		}
	}

	private void OnDisable()
	{
		_rotationSequence.Kill();
		racePointer.transform.DOKill();

		_coinEarn = 0;
		//bonus[_bonusIndex].SetActive(false);
	}
}

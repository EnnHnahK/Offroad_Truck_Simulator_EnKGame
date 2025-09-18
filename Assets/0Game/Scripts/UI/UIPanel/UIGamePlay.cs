using System.Collections;
using System.Text;
using DG.Tweening;
using NWH.Common.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIGamePlay : BasePanel
{
	[SerializeField] private ButtonEffectLogic btnReverse, btnSkip, /*btnBrake,*/ btnCam, btnNitro, btnWiper, btnWiperAuto;
	
	[SerializeField] private GameObject upOn, downOn, reverse, steeringWheel, adsIcon, wiperAutoOn,
		wiperAutoOff, leftButtonGroup, nitroTutorial, tutorialWheel, breakActive, breakInactive;
	[SerializeField] private Image speedFill, fuelFill, raceFill, nitroFill;
	[SerializeField] private Text speedText, fuelText, nitroText, wiperAutoText;

	[SerializeField] private GameObject[] dirtiesWindow;
	[SerializeField] private Animator wiperAnimator, fuelAnimator;

	public SteeringWheelInput throttleInput, brakeInput;

	private int _camIndex = 0, _camLength, _nitroCount, _timeAutoWiper = 4; //Min
	private float _maxSpeed, _maxFuel, _totalDistance, _baseFuel, _fuel, _fuelUse = 1f, timeDirty = 10f, _angle;

	private bool _isReversing, _usingFuel, _useNitro, _useWiper, _autoWiper, _tutNitro, _tutWiper, _mission, _warningFuel, _tutorialSteering;

	private Vector3 _preReversePos;
	
	[SerializeField] private ReverseButtonAnimUI reverseButtonAnimUI;

	private Coroutine _coroutineFuel;

	private WaitForSeconds _waitWiper;
	
	
	[Header("Vehicle Controller")]
	[SerializeField] private MobileInputButton btnBrake;
	[SerializeField] private NWH.VehiclePhysics2.VehicleGUI.SteeringWheel throttle, brakes;

	protected override void Awake()
	{
		base.Awake();
		btnReverse.onEnter.AddListener(() =>
		{
			if (!canTarget || CheckThrottle()) return;
			
			if (DataController.StepTutorial == 7)
			{
				PrefData.StepTutorial++;
				UIController.Instance.uITutorial.HideFinger();
			}
			Reverse();
		});
		_preReversePos = reverse.transform.position;
		btnSkip.onClick.AddListener(() =>
		{
			if (RaceManager.Instance.RaceRunning())
			{
				CompletedRace(false);
			}
		});
		btnBrake.onDown.AddListener(() =>
		{
			if (DataController.StepTutorial == 9)
			{
				PrefData.StepTutorial++;
				UIController.Instance.uITutorial.HideFinger();
			}
			
			Brake(true);
		});
		btnBrake.onUp.AddListener(() =>
		{
			Brake(false);
		});
		btnCam.onClick.AddListener(ChangeCamera);
		btnNitro.onClick.AddListener(NitroBoost);
		btnWiper.onClick.AddListener(ManualWiper);
		btnWiperAuto.onClick.AddListener(AutoWiper);
		
		//Temp Load Cur Level Fuel
		_baseFuel = DataController.Instance.GetCurLevelBaseFuel();

		_waitWiper = new WaitForSeconds(timeDirty);
	}


	private void OnEnable()
	{
		ResetReverse();
		Init();
		StartCoroutine(IEUpdateDisplay());
		//SoundManager.Instance.PlayBg(MusicType.Gameplay);
	}

	private void Init()
	{
		//Tutorial Steering
		_tutorialSteering = true;
		tutorialWheel.SetActive(true);
		steeringWheel.SetActive(false);
		
		//Set Angle Max Steering
		var wheelbase = _TruckManager.Instance.GetValueWheelBase();
		throttle.SetMaxSteeringAngle(wheelbase);
		brakes.SetMaxSteeringAngle(wheelbase);
		
		//Cam
		_camIndex = 0;
		_camLength = _TruckManager.Instance.GetCameraSwitch().GetLengthCam();

		//Fuel
		_maxFuel = ProcessingVehicleInit();

#if UNITY_EDITOR
		_maxFuel = 1000;
#endif
		_fuel = _maxFuel;
	
		//Update Text && Fill Bar
		fuelText.text = Consts.Percent;
		fuelFill.fillAmount = 1f;
		
		//Grip
		if (DataController.StepTutorial > 2)
		{
			var baseGrip = DataController.Instance.dataVehicle.vehicles[DataController.CurVehicle].BaseGrip;
			_TruckManager.Instance.ApplyGripUpgrade(baseGrip + UIController.Instance.uIGarage.GetLevelStats(1) * 0.05f);
		}
		else
		{
			_TruckManager.Instance.ApplyGripUpgrade(1.5f);
		}
		
		
		//Speed
		_maxSpeed = 25f;//UIController.Instance.inputSystem.GetVehicleMaxSpeed();
		
		if(_coroutineFuel != null) StopCoroutine(_coroutineFuel);
		_coroutineFuel =  StartCoroutine(IEFuelUsing());
		
		leftButtonGroup.SetActive(!DataController.IncompleteTutorial);
		
		//Mission
		_mission = false;
		UIController.Instance.uIGameCompleted.SetCoinMission(0);

		_warningFuel = false;
		CinemachineManager.Instance.SetRunRotate(true);
	}

	private float ProcessingVehicleInit()
	{
		var scale = 1f;
		switch (DataController.CurVehicle)
		{
			//IAP Vehicle x2 Fuel
			case 8:
				scale = 2;
				break;
			//IAP Vehicle x1.5 Fuel
			case 9:
				scale = 1.5f;
				break;
		}
		return _baseFuel + UIController.Instance.uIGarage.GetLevelStats(0) * DataController.Instance.dataLevel.levels[DataController.CurLevel].basePerLevel * scale;
	}
	
	protected void Start()
	{
		//Check Tutorial Nitro + Wiper
		_tutNitro = DataController.StepTutorial < 8;
		_tutWiper = DataController.StepTutorial < 10;
		
		if(_tutNitro) btnNitro.gameObject.SetActive(false);
		if (_tutWiper)
		{
			btnWiper.gameObject.SetActive(false);
			btnWiperAuto.gameObject.SetActive(false);
		}
	}

	public void TutorialSteering()
	{
		if(!_tutorialSteering) return;
		
		_tutorialSteering = false;
		tutorialWheel.SetActive(false);
		steeringWheel.SetActive(true);

		//Tutorial
		if (DataController.StepTutorial == 1)
		{
			UITutorial.Instance.NextStep();
		}
	}
	
	
	#region events assignment

	public void SetEventLose()
	{
		_TruckManager.Instance.GetTruckOnCollider().OnTruckLose += OnColLose;
	}
	
	private void OnColLose()
	{
		if (RaceManager.Instance.RaceRunning())
		{
			CompletedRace(false);
		}
	}
	#endregion
	
	
	IEnumerator IEUpdateDisplay()
	{
	    //yield return new WaitUntil(() => VehicleManager.Instance.vehicleSelected != null);
		while (RaceManager.Instance.RaceRunning())
		{
			//Speed
			if (!_TruckManager.Instance.GetVehicleControl())
			{
				yield break;
			}
			var speed = _TruckManager.Instance.GetVehicleControl().Speed * 4;
			speedFill.fillAmount = speed / _maxSpeed * 0.75f;
			speedText.text = ((int)speed).ToString();
			
			raceFill.fillAmount = RoadManager.Instance.GetRoadProgress();

			yield return Yielder.GetWaitForSeconds(0.2f);
		}
		//yield return null;
	}
	
	
	public void CompletedRace(bool finish = true)
	{
		canTarget = false;
		
		fuelAnimator.Play("Idle");
		
		if(_isReversing) Reverse();
		
		//End Race && Stop Vehicle
		RaceManager.Instance.EndRace();
		EventSystem.current.SetSelectedGameObject(null);
		
		//Truck10
		if(_TruckManager.Instance.GetTransportType() == 5) _TruckManager.Instance.GetFireCamMission().ToggleFireFighting(false);

		Events.OnCompleteLevel?.Invoke(finish);
		
		//Mission Check
		UIController.Instance.uINotify.WorkingNotify(false);
		UIController.Instance.SetWorkingInInput(false);
		
		brakeInput.StopSteeringWheel();
		throttleInput.StopSteeringWheel();

		UIController.Instance.NotiFinishRace((int)(RoadManager.Instance.GetRoadLength() * RoadManager.Instance.GetRoadProgress()));

		DOTween.Sequence().AppendInterval(.8f)
			.AppendCallback(() => _TruckManager.Instance.Stop())
			.AppendInterval(.7f).AppendCallback(() =>
			{
				ShowAfterHide(UIController.Instance.uIGameCompleted);
			});
	}
	

	#region Vehicle Control

	private void ResetReverse()
	{
		_isReversing = false;
		upOn.SetActive(true);
		downOn.SetActive(false);
		reverse.transform.DOMoveY(_preReversePos.y, 0.5f);
	}

	public void SetReverse(bool isReversing)
	{
		brakeInput.StopSteeringWheel();
		throttleInput.StopSteeringWheel();

		if (isReversing != _isReversing)
		{
			Reverse();
		}
	}

	private void Reverse()
	{
		//UIController.Instance.inputSystem.ToggleReversing();
		reverseButtonAnimUI.ToggleReversingAnim();
		_isReversing = !_isReversing;
		SoundManager.Instance.PlayShot(SoundManager.Instance.gearShift);
		
		_TruckManager.Instance.GetVehicleFunction().ReverseLightIntensity(_isReversing);
		
		
		//Mission
		if (_TruckManager.Instance.GetTransportType() == 6)
		{
			_TruckManager.Instance.GetTrailerMission().ToggleTrailerCam(_isReversing);
		}
		
		if (_isReversing)
		{
			//Refresh Wheel
			UIController.Instance.SetSteeringWheel(brakes);
			brakes.gameObject.SetActive(true);
			throttle.gameObject.SetActive(false);
			
			throttle.SetAngleValue();
			
			downOn.SetActive(true);
			upOn.SetActive(false);
		}
		else
		{
			//Refresh Wheel
			UIController.Instance.SetSteeringWheel(throttle);
			brakes.gameObject.SetActive(false);
			throttle.gameObject.SetActive(true);
		
			brakes.SetAngleValue();
			
			upOn.SetActive(true);
			downOn.SetActive(false);
		}
	}
	
	#endregion

	private void ChangeCamera()
	{
		if (_camIndex < _camLength - 1) _camIndex++;
		else _camIndex = 0;
		
		steeringWheel.SetActive(_camIndex != _camLength - 1);
		
		_TruckManager.Instance.GetCameraSwitch().SelectCamera(_camIndex);
		
	}
	
	private void Brake(bool isBrake)
	{
		if (isBrake)
		{
			breakActive.SetActive(true);
			breakInactive.SetActive(false);
		}
		else
		{
			breakActive.SetActive(false);
			breakInactive.SetActive(true);
		}
		_TruckManager.Instance.GetVehicleFunction().BrakeLightIntensity(isBrake);
		
	}
	#region  Mission
	public void Mission()
	{
		if (_mission) return;
		_mission = true;
		UIController.Instance.missionPopup.Show();
	}

	public bool GetMissionPopShowed()
	{
		return _mission; 
	}
	
	#endregion

	#region Vehicle Nitro System

	private void NitroBoost()
	{
		#if UNITY_EDITOR || CHEAT
		if(_useNitro) return;
	
		if (_nitroCount == 0)
		{
			//Reward Ads
			/*AdsAdapter.instance.ShowRewardedVideo(
						_nitroCount = 2;
						ApplyNitro();,
				() => UIController.Instance.Notify(Consts.ads_fail), DataController.CurLevel, 
				AdsAdapter.@where.shop_reward);*/
			_nitroCount = 2;
			adsIcon.gameObject.SetActive(false);
			ApplyNitro();
		}
		else
		{
			ApplyNitro();
		}
		#endif
		
		#if UNITY_EDITOR || DEBUGLOG
		Debug.Log("NitroBoost Apply");
		#endif
	}

	private void ApplyNitro()
	{
		if (DataController.StepTutorial == 7)
		{
			nitroTutorial.SetActive(false);
			UIController.Instance.uITutorial.HideFinger();
			PrefData.StepTutorial++;
		}
		
		_useNitro = true;
		_TruckManager.Instance?.SetNitroPar(true);
		
		_nitroCount--;
		
		//Update Text
		if (_nitroCount == 0) nitroText.text = ""; 
		else nitroText.text = "" + _nitroCount; 
		
		_TruckManager.Instance?.ApplyNitro(true);
		
		nitroFill.DOFillAmount(0f, 30f).From(1f).SetEase(Ease.Linear).OnComplete(() =>
		{
			_useNitro = false;

			_TruckManager.Instance?.SetNitroPar(false);
			_TruckManager.Instance?.ApplyNitro(false);
			if (_nitroCount == 0)
			{
				nitroText.text = "2";
				adsIcon.SetActive(true);
			}
		});
	}
	
	public void NitroTutorial()
	{
		if (!_tutNitro) return;
		_tutNitro = false;
		UIController.Instance.uITutorial.SetFingerPos(btnNitro.transform.localPosition, leftButtonGroup.transform);
		nitroTutorial.SetActive(true);
		btnNitro.gameObject.SetActive(true);
		btnNitro.transform.DOMoveX(btnNitro.transform.position.x, 1f).From(btnNitro.transform.position.x - 50f);
		adsIcon.SetActive(false);
		_nitroCount = 1;
		nitroText.text = "1";
	}


	#endregion
	
	#region Vehicle Wiper System
	
	public void WiperTutorial()
	{
		if(!_tutWiper) return;
		_tutWiper = false;
		UIController.Instance.uITutorial.SetFingerPos(btnWiper.transform.localPosition, leftButtonGroup.transform);
		btnWiper.gameObject.SetActive(true);
		btnWiper.transform.DOMoveX(btnNitro.transform.position.x, 1f).From(btnNitro.transform.position.x - 50f);
		btnWiperAuto.gameObject.SetActive(false);
	}

	private void ManualWiper()
	{
		if (_useWiper) return;
		_useWiper = true;
		
		if (DataController.StepTutorial == 9)
		{
			PrefData.StepTutorial++;
			UIController.Instance.uITutorial.HideFinger();
			btnWiperAuto.gameObject.SetActive(true);
		}
		
		wiperAnimator.Play("Wiper");
		DOVirtual.DelayedCall(2f, () =>
		{
			if (dirtiesWindow[1].activeSelf)
			{
				DOVirtual.DelayedCall(timeDirty,  () =>
				{
					dirtiesWindow[1].SetActive(true);

					if (_autoWiper)
					{
						DOVirtual.DelayedCall(0.5f, ManualWiper);
					}
				});
				dirtiesWindow[1].SetActive(false);
			}
			_useWiper = false;
		});
	}

	private void AutoWiper()
	{
		if(_autoWiper) return;
		_autoWiper = true;
		
		wiperAutoOn.SetActive(true);
		wiperAutoOff.SetActive(false);
		ManualWiper();
		StartCoroutine(IEAutoWiper());
	}

	IEnumerator IEAutoWiper()
	{
		StringBuilder sb = new StringBuilder();
		var sec = 59;
		while (_autoWiper)
		{
			sb.Clear();
			sb.AppendFormat("{0:D2}:{1:D2}", _timeAutoWiper, sec);
			wiperAutoText.text = sb.ToString();
			sec -= 10;
			if (sec <= 0)
			{
				_timeAutoWiper--;
				sec = 59;
			}

			if (_timeAutoWiper <= 0)
			{
				_autoWiper = false;
				wiperAutoOn.SetActive(false);
				wiperAutoOff.SetActive(true);
				yield break;
			}
			yield return _waitWiper;
		}
	}


	#endregion
	
	public void SetBaseFuel(float baseFuel)
	{
		_baseFuel = baseFuel;
	}

	public float GetCurFuel()
	{
		return _fuel;
	}

	public void UsingFuel(bool isUsing)
	{
		_usingFuel = isUsing;
	}
	
	IEnumerator IEFuelUsing()
	{
		while (true)
		{
			if (_usingFuel)
			{
				if (_fuel > 0)
				{
					_fuel -= _fuelUse;
					
					//Fuel
					var fuelRatio = _fuel / _maxFuel ;
					fuelFill.fillAmount = fuelRatio * 0.71f;
					fuelText.text = (int)(Mathf.Max(0 , fuelRatio * 100)) + "%";

					if (_fuel <= 5 && !_warningFuel)
					{
						_warningFuel = true;
						fuelAnimator.Play("Fuel");
					}
					
				}
				else
				{
					if (RaceManager.Instance.RaceRunning())
					{
						CompletedRace(false);
					}
					yield break;
				}
			}
			yield return Consts.OneSec;
		}
	}

	public void SetDirtyWindow(TypeWeather type, bool isActive = true)
	{
		return;
		//if(dirtiesWindow[0] == null) return;
		
		switch (type)
		{
			case TypeWeather.None:
			case TypeWeather.SunShine:
				foreach (var g in dirtiesWindow)
				{
					if (g != null)
					{
						g.SetActive(false);
					}
				}
				return;
		}

		if (type == TypeWeather.Rain && _tutWiper)
		{
			WiperTutorial();
		}
		
		dirtiesWindow[(int)type - 1]?.SetActive(isActive);
	}

	private void OnDisable()
	{
		UsingFuel(false);
		if (_useNitro)
		{
			if (_TruckManager.Instance)
			{
				_TruckManager.Instance.SetNitroPar(false);
			}
			_useNitro = false;
			nitroFill.DOKill();
			nitroFill.fillAmount = 1;
			
			if (_nitroCount == 0)
			{
				nitroText.text = "2";
				adsIcon.SetActive(true);
			}
		}

		if (_useWiper)
		{
			_useWiper = false;

		}
		_autoWiper = false;
		_tutorialSteering = false;
		CinemachineManager.Instance.SetRunRotate(false);
	}

	public Transform GetReverse()
	{
		return btnReverse.transform;
	}

	public Transform GetBreak()
	{
		return btnBrake.transform;
	}

	private bool CheckThrottle()
	{
		return throttleInput.GetThrottleStatus() || brakeInput.GetThrottleStatus();
	}
	
	public float GetAngleClamped(){
		if (throttle.gameObject.activeSelf) return throttle.GetClampedValue();
		else return brakes.GetClampedValue();
	}
}

public enum TypeWeather
{
    None = 0, 
    SunShine = 1, 
    Rain = 2, 
    Snow = 3,
}


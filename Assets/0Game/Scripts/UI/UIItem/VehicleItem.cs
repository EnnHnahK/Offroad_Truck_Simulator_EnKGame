using System;
using DG.Tweening;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class VehicleItem : MonoBehaviour
{
	private int _id;
	private DataVehicle.VehicleType _vehicleType;
	private bool _isTry, _isOwned, _canExchange;
	
	[Header("Vehicle Basic")]
	[SerializeField] private Button btnChooseVehicle;
	[SerializeField] private Image vehicleImage, bgImage;
	
	[Header("Try Vehicle"), Tooltip("Use for vehicles not yet owned")]
	[SerializeField] private GameObject rewardAds;
	[SerializeField] private GameObject locked, rewardButton, goButton;

	[Header("Owned Vehicle"), Tooltip("Use for owned vehicles")]
	[SerializeField] private GameObject owned;
	[SerializeField] private GameObject selectButton, notiNew;
	
	[Header("IAP Vehicle"), Tooltip("Use for vehicles that can be purchased in-app")]
	[SerializeField] private GameObject[] iapDesObjects;
	[SerializeField] private GameObject IAP, IAPDes, buyButton;
	[SerializeField] private Text priceIAP, IAPCoinText, IAPFuelText;
	[SerializeField] private Animator particleAnimator;

	[Header("Native Vehicle"), Tooltip("Use for native ads")]
	[SerializeField] private GameObject nativeAds;
	[SerializeField] private GameObject nativeButton;

	[Header("Exchange Vehicle"), Tooltip("Used for vehicles that can be exchanged for Fragments")]
	[SerializeField] private Text[] fragmentsRequest;
	[SerializeField] private GameObject exchange, exchangeButton;
	[SerializeField] private Outline[] fragmentsOutlines;

	private int[] _fragmentsNeeded, _fragmentTemp;
	
	[Header("In Use Vehicle"), Tooltip("Use for vehicles in use")]
	[SerializeField] private GameObject inUse;
	
	public void Init(DataVehicle.Vehicle vehicle)
	{
		//Basic Vehicle
		_id = vehicle.id;
		vehicleImage.sprite = vehicle.vehicleImage;
		bgImage.sprite = vehicle.bgImage;
		_vehicleType = vehicle.vehicleType;
		
		Utils.SetNativeImage(vehicleImage, vehicle.vehicleImage.rect.size, 1.2f);

		//Owned Vehicle
		if (PrefData.GetVehicleOwned(_id))
		{
			if (_id == DataController.CurVehicle)
			{
				inUse.SetActive(true);
				return;
			}
			_isOwned = true;
			
			owned.SetActive(true);
			selectButton.SetActive(true);
			return;
		}
		
		//Not yet Owned Vehicle
		locked.SetActive(true);

		switch (_vehicleType)
		{
			case DataVehicle.VehicleType.NativeAds: //Native Vehicle
				nativeAds.SetActive(true);
				nativeButton.SetActive(true);
				return;
			case DataVehicle.VehicleType.IAP: //IAP Vehicle
				//Set Price Text IAP
				particleAnimator.enabled = true;
				
				IAP.SetActive(true);
				priceIAP.text = "$" + vehicle.priceIAP;
			
				//Load Des IAP
				IAPDes.SetActive(true);
				buyButton.SetActive(true);
				
				
				for (int i = 0; i < vehicle.iapDes.Length; i++)
				{
					if (vehicle.iapDes[i] == 0)
					{
						iapDesObjects[i].SetActive(false);
					}else if(vehicle.iapDes[i] == 1)
					{
						iapDesObjects[i].SetActive(true);
					}else if(vehicle.iapDes[i] == 2)
					{
						iapDesObjects[i].SetActive(true);
						IAPCoinText.text = "+100%";
						IAPFuelText.text = "+100%";
					}
				}
				return;
			case DataVehicle.VehicleType.Exchange:
				_fragmentsNeeded = new int[3];
				_fragmentTemp = new []{0, 0, 0};
				
				exchange.SetActive(true);
				exchangeButton.SetActive(true);
				for (int i = 0; i < fragmentsRequest.Length; i++)
				{
					_fragmentsNeeded[i] = vehicle.iapExchange[i];
					FragmentsExchangeVehicle(true);
				}
				return;
		}
		
		//Try Vehicle
		rewardAds.SetActive(true);
		rewardButton.SetActive(true);
	}
	
	private void OnEnable()
	{
		DOVirtual.DelayedCall(.2f, VehicleShow);
		
		//Vehicle non yet owned processing
		if(_isOwned) return;
		
		if(_vehicleType == DataVehicle.VehicleType.Exchange) FragmentsExchangeVehicle();
	}

	private void Awake()
	{
		btnChooseVehicle.onClick.AddListener(() =>
		{
			if (UIController.Instance.uIChooseVehicle.canTarget)
			{
				ChooseVehicle();
			}
		});
	}
	
	private void VehicleShow()
	{
		vehicleImage.transform.DOLocalMoveX(0, 0.5f).From(200);
		vehicleImage.transform.DOScale(1f, 0.5f).From(0.5f).OnComplete(() =>
		{
			vehicleImage.transform.DOShakePosition(20,new Vector3(0.5f, 1, 0), 1, 90f, false, false)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo);
		});
	}

	private void FragmentsExchangeVehicle(bool isInit = false)
	{
		_canExchange = true;
		for (int i = 0; i < fragmentsRequest.Length; i++)
		{
			var cur = DataController.Instance.GetCurFragment(i);

			if (cur < _fragmentsNeeded[i]) _canExchange = false;
			
			if(_fragmentTemp[i] == cur && !isInit) continue;
			_fragmentTemp[i] = cur;
			
			if (cur >= _fragmentsNeeded[i])
			{
				fragmentsRequest[i].color = Color.green;
				fragmentsOutlines[i].effectColor = Color.green;
			}
			else
			{
				fragmentsRequest[i].color = Color.red;
				fragmentsOutlines[i].effectColor = Color.red;
			}
			
			fragmentsRequest[i].text = cur +"/"+ _fragmentsNeeded[i];
		}
	}

	private void IAPVehicle()
	{
		switch (_id)
		{
			case 0:
				//IAPManager.Instance.IAPBuy_Btn_Pressed(IAPManager.vehicle1);
				break;
			case 1:
				//IAPManager.Instance.IAPBuy_Btn_Pressed(IAPManager.vehicle1);
				break;
			case 2:
				//IAPManager.Instance.IAPBuy_Btn_Pressed(IAPManager.vehicle1);
				break;
		}
	}

	private void BuySuccess(DataVehicle.VehicleType type)
	{
		locked.SetActive(false);
		_isOwned = true;
		
		switch (type)
		{
			case DataVehicle.VehicleType.IAP:
				IAP.SetActive(false);
				IAPDes.SetActive(false);
				buyButton.SetActive(false);
				return;
			case DataVehicle.VehicleType.NativeAds:
				nativeAds.SetActive(false);
				nativeButton.SetActive(false);
				return;
			case DataVehicle.VehicleType.Exchange:
				exchange.SetActive(false);
				exchangeButton.SetActive(false);
				return;
		}
		
		
		rewardAds.SetActive(false);
		rewardButton.SetActive(false);
	}

	public void Deselected()
	{
		inUse.SetActive(false);
		owned.SetActive(true);
		selectButton.SetActive(true);
	}
	
	private void ChooseVehicle()
	{
		//Sound 
		//SoundManager.Instance.PlayShot(SoundManager.Instance.click);
		
		#if (UNITY_EDITOR || CHEAT) && !LOCKED
			
			//Temp
			if (_vehicleType == DataVehicle.VehicleType.Exchange)
			{
				if (_canExchange)
				{
					PrefData.SetVehicleOwned(_id);
					BuySuccess(_vehicleType);
					
					if(selectButton.activeSelf) selectButton.SetActive(false);
					UIController.Instance.uIChooseVehicle.ChooseVehicle(_id);
			
					inUse.SetActive(true);
					return;
				}
		            
				UIController.Instance.Notify(LocalizationManager.GetTranslation(Consts.not_enough_frag));
				return;
			}
			if (!PrefData.GetVehicleOwned(_id))
			{
				PrefData.SetVehicleOwned(_id);
				BuySuccess(_vehicleType);
			}
			
			if(selectButton.activeSelf) selectButton.SetActive(false);
			UIController.Instance.uIChooseVehicle.ChooseVehicle(_id);
			
			inUse.SetActive(true);
			
			if(notiNew.activeSelf) notiNew.SetActive(false);
			return;
		#endif
		
		if (PrefData.GetVehicleOwned(_id))
		{
			if (selectButton.activeSelf) selectButton.SetActive(false);
			UIController.Instance.uIChooseVehicle.ChooseVehicle(_id);
			
			inUse.SetActive(true);
			return;
		}
		
		switch (_vehicleType)
		{
			case DataVehicle.VehicleType.NativeAds:
#if LOCKED
				UIController.Instance.Notify(Consts.install_fail);
				UIController.Instance.uIChooseVehicle.ChooseVehicle(DataController.CurVehicle);
				return;
#endif				
				break;
			
			case DataVehicle.VehicleType.IAP:
#if LOCKED
					UIController.Instance.Notify(Consts.iap_fail);
					UIController.Instance.uIChooseVehicle.ChooseVehicle(DataController.CurVehicle);
					return;
#endif
				IAPVehicle();
				return;
			case DataVehicle.VehicleType.Locked:
#if LOCKED
				UIController.Instance.Notify(Consts.ads_fail);
				UIController.Instance.uIChooseVehicle.ChooseVehicle(DataController.CurVehicle);
				return;
#endif
				/*AdsAdapter.instance.ShowRewardedVideo(
					TryVehicle();
				() => UIController.Instance.Notify(Consts.ads_fail), DataController.CurLevel, 
				AdsAdapter.@where.vehicle_reward);*/
				return;
			case DataVehicle.VehicleType.Exchange:
				if (_canExchange)
				{
					PrefData.SetVehicleOwned(_id);
					BuySuccess(_vehicleType);
					return;
				}
				
				UIController.Instance.Notify(LocalizationManager.GetTranslation(Consts.not_enough_frag));
				return;
		}
	}
	
	public void Unlocked()
	{
		rewardAds.SetActive(false);
		owned.SetActive(true);
		rewardButton.SetActive(false);
		
		notiNew.SetActive(true);
	}

	#region Try Vehicle

	
	private void TryVehicle()
	{
		UIController.Instance.uIChooseVehicle.TryVehicle(_id);
		if (_isTry) return;
		_isTry = true;
		locked.SetActive(false);
		rewardAds.SetActive(false);
		rewardButton.SetActive(false);
		goButton.SetActive(true);
	}

	public void EndTry()
	{
		_isTry = false;
		locked.SetActive(true);
		rewardAds.SetActive(true);
		rewardButton.SetActive(true);
		goButton.SetActive(false);
	}

	#endregion


	private void OnDisable()
	{
		vehicleImage.transform.DOKill();
	}
}


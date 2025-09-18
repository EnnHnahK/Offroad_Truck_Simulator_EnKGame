using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class IAPSuggestPopup : BasePopup
{
	[SerializeField] private ButtonEffectLogic btnBuy;

	[SerializeField] private GameObject[] iapDes;
	[SerializeField] private Text[] iapText;
	[SerializeField] private Transform lightRay;

	[SerializeField] private Image vehicleIcon, fillLine;

	[SerializeField] private Text salePrice, defaultPrice, timeSale;

	[SerializeField] private Animator buyAnimator;

	private bool _initiated;
	
	private List<int> _iapIndex = new List<int>{8, 9, 10};
	private int _indexSuggest;

	protected override void Awake()
	{
		base.Awake();
		btnBuy.onClick.AddListener(BuyIAP);
		
#if UNITY_EDITOR
		var checkStatus = true;
		for (int i = 0; i < _iapIndex.Count; i++)
		{
			var type = DataController.Instance.dataVehicle.vehicles[_iapIndex[i]].vehicleType;
			if (type != DataVehicle.VehicleType.IAP)
			{
				Debug.LogError("Index value does not match data. Index = " + _iapIndex[i] + ". Type "+ type + ". Type matched: IAP");
				checkStatus = false;
				break;
			}
		}
		
		if(checkStatus) Debug.Log("Index value matches data");
#endif
	}

	private void OnEnable()
	{
		if(!_initiated) Init();
		
		DOVirtual.DelayedCall(.3f, PopEffect);
	}

	private void Init()
	{
		_initiated = true;
		
		for (int i = _iapIndex.Count - 1; i >= 0; i--)
		{
			if (PrefData.GetVehicleOwned(_iapIndex[i]))
			{
				_iapIndex.RemoveAt(i);
			}
		}

		if (_iapIndex.Count == 0)
		{
			PrefData.AllIAPOwned = true;
			UIController.Instance.AllIAPOwned();
			return;
		}

		_indexSuggest = _iapIndex[Random.Range(0, _iapIndex.Count)];
		var data = DataController.Instance.dataVehicle.vehicles[_indexSuggest] ;
		vehicleIcon.sprite = data.vehicleImage;
		
		Utils.SetNativeImage(vehicleIcon, vehicleIcon.rectTransform.sizeDelta);

		for (int i = 0; i < data.iapDes.Length; i++)
		{
			switch (data.iapDes[i])
			{
				case 0:
					iapDes[i].SetActive(false);
					break;
				case 1:
					iapDes[i].SetActive(true);
					break;
				case 2:
					iapDes[i].SetActive(true);
					iapText[0].text = "+100%";
					iapText[1].text = "+100%";
					break;
			}
		}

		defaultPrice.text ="$" + data.priceIAP;
		salePrice.text = "$" + (data.priceIAP - 5f);
	}

	public override void Hide()
	{
		base.Hide();
		UIController.Instance.EventClosePop();
	}
	/*
	private IEnumerator IERemainingTime()
	{
		StringBuilder sb = new StringBuilder();
		while (true)
		{
			TimeSpan remainingTime = _targetTime - DateTime.Now;

			if (remainingTime <= TimeSpan.Zero)
			{
    				#if UNITY_EDITOR
				Debug.Log("Reward Vehicle Available");
    				#endif

				_rewardAvailable = true;

				notifyReward.SetActive(true);

				desCountDownText.text = LocalizationManager.GetTranslation(Consts.vehicle_unlocked);

				countdownText.text = "";

				yield break;
			}

			sb.Clear();
			sb.AppendFormat("{0:D2}:{1:D2}:{2:D2}", (int)remainingTime.TotalHours, remainingTime.Minutes, remainingTime.Seconds);
			countdownText.text = sb.ToString();

			yield return Yielder.GetWaitForSeconds(1);
		}
	}*/

	private void PopEffect()
	{
		vehicleIcon.transform.DOShakePosition(20,new Vector3(0.5f, 1, 0), 1, 90f, false, false)
			.SetEase(Ease.InOutSine)
			.SetLoops(-1, LoopType.Yoyo);

		defaultPrice.transform.DOLocalMoveY(defaultPrice.transform.localPosition.y, 1.5f).From(0);
		defaultPrice.transform.DOScale(0.5f, 1.5f).From(0.7f);
		defaultPrice.DOColor(defaultPrice.color, 1.5f).From(Color.white).OnComplete(() =>
		{
			buyAnimator.enabled = true;
			salePrice.gameObject.SetActive(true);
			salePrice.DOFade(1f, .5f).From(0);
			fillLine.DOFillAmount(1f, .5f).From(0);
		});
		
		lightRay.DORotate(new Vector3(0, 0, -360), 5f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
	}

	private void BuyIAP()
	{
		_initiated = false;
		Hide();
		PrefData.SetVehicleOwned(_indexSuggest);
		UIController.Instance.uIChooseVehicle.ChooseVehicle(_indexSuggest, false);
	}
	
	private void OnDisable()
	{
		fillLine.DOKill();
		fillLine.fillAmount = 0;
		defaultPrice.DOKill();
		salePrice.DOKill();
		
		salePrice.gameObject.SetActive(false);
		buyAnimator.enabled = false;
		
	}
}

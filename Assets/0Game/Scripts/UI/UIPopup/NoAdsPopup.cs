using UnityEngine;
public class NoAdsPopup : BasePopup
{
	[SerializeField] private ButtonEffectLogic btnOk;

	protected override void Awake()
	{
		base.Awake();
		btnOk.onClick.AddListener(BuyRemoveAds);
	}

	private void BuyRemoveAds()
	{
		
	}
	
	public override void Hide()
	{
		base.Hide();
		UIController.Instance.EventClosePop();
	}
}	

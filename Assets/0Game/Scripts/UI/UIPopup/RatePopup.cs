using UnityEngine;

public class RatePopup : BasePopup
{
    [SerializeField] private ButtonEffectLogic btnOk;
    
    protected override void Awake()
    {
        base.Awake();
        btnOk.onClick.AddListener(RateUs);
    }
    
    private void RateUs()
    {
        Hide();
        Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}");
    }

    public override void Hide()
    {
        base.Hide();
        UIController.Instance.EventClosePop();
    }
}

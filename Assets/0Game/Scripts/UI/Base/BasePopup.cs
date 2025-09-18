using DG.Tweening;
using UnityEngine;

public class BasePopup : MonoBehaviour
{
    [SerializeField] protected ShowEffect main;

    [SerializeField] private ButtonEffectLogic btnClose;

    public bool isShow;
    
    protected virtual void Awake()
    {
        if(btnClose != null) btnClose.onClick.AddListener(Hide);
    }

    public virtual void Show()
    {
        isShow = true;
        gameObject.SetActive(true);
        main.Effect();
    }

    public virtual void Hide()
    {
        main.Hide();
        DOVirtual.DelayedCall(0.34f, () => gameObject.SetActive(false));
        isShow = false;
    }
}

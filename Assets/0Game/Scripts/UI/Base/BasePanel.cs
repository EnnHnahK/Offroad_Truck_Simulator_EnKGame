using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
	[SerializeField] private Button btnClose;

	[SerializeField] protected ShowEffect[] showEffects;

	public bool canTarget;

	protected virtual void Awake()
	{
		if(btnClose != null) btnClose.onClick.AddListener(Hide);
	}

	public virtual void Show()
	{
		canTarget = true;
		gameObject.SetActive(true);
		
		//Sound
		SoundManager.Instance.PlayShot(SoundManager.Instance.panelShow);

		//Apply Effect
		foreach (var show in showEffects)
		{
			show.Effect();
		}
	}

	public virtual void Hide()
	{
		canTarget = false;
		foreach (var show in showEffects)
		{
			show.Hide();
		}
		DOVirtual.DelayedCall(0.35f,  () => gameObject.SetActive(false));
	}

	public virtual void ShowAfterHide(BasePanel nextPanel, bool loading = false, float time = 5f)
	{
		canTarget = false;
		foreach (var show in showEffects)
		{
			show.Hide();
		}
		
		DOVirtual.DelayedCall(0.34f,  () =>
		{
			gameObject.SetActive(false);
			if(loading) UIController.Instance.LoadingScreen(time, true, nextPanel.Show); 
			else nextPanel.Show();
		});
	}
}

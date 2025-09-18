using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlingParticle : MonoBehaviour
{
	[SerializeField] private Image[] imageParicle;

	private bool _initiated;

	private Sequence _bling;
	
	private void OnEnable()
	{
		if (!_initiated)
		{
			Init(); 
			return;
		}

		_bling.Play().SetLoops(-1);
	}

	private void Init()
	{
		_bling = DOTween.Sequence();

		foreach (var image in imageParicle)
		{
			_bling.AppendCallback(() =>
			{
				image.transform.DOScale(1f, 1f).From(.5f);
				image.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360);
				image.DOFade(1f, 1f).From(.4f);
			}).AppendInterval(.7f).AppendCallback(() => image.DOFade(.35f, .35f).From(1f));
		}
		_bling.Play().SetLoops(-1);
	}

	private void OnDisable()
	{
		foreach (var image in imageParicle)
		{
			image.DOKill();
		}
	}
}

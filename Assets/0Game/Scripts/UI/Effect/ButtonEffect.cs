using DG.Tweening;
using UnityEngine;

public class ButtonEffect : MonoBehaviour
{
	[SerializeField] private RectTransform _rectTransform;

	private float strength = 4;

	private Vector3 _strengthVector;
	private void Awake()
	{
		_strengthVector = new Vector3(0, 0, strength);
	}

	private void OnEnable()
	{
		_rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
	}

	public void Shake()
	{
		if (gameObject.activeInHierarchy)
		{
			_rectTransform.DOShakeRotation(0.5f, _strengthVector, 25, 3).SetEase(Ease.InQuad).OnComplete(() => {
				_rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
			});
			_rectTransform.DOScale(1.05f, 0.25f).OnComplete(() =>
			{
				_rectTransform.DOScale(1f, .25f);
			});
		}
	}

	private void OnDisable()
	{
		_rectTransform.DOKill();
	}


}

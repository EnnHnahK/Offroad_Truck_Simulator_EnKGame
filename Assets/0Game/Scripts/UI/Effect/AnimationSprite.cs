using DG.Tweening;
using UnityEngine;

public class AnimationSprite : MonoBehaviour
{
	public enum AnimationType
	{
		Scale,
		Face,
	}

	[SerializeField] private AnimationType _animationType;

	private void Start()
	{
		switch (_animationType)
		{
			case AnimationType.Scale:
			{
				ScaleAnimation();
				break;
			}
		}
	}

	private void ScaleAnimation()
	{
		transform.DOScale(transform.localScale * 1.25f, 1.5f).SetLoops(-1, LoopType.Yoyo); 
	}
}

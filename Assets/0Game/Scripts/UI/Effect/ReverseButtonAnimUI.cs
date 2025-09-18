using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseButtonAnimUI : MonoBehaviour
{
    private const string ANIMATION_REV_D = "ReverseD";
    private const string ANIMATION_REV_R = "ReverseR";

    [SerializeField] private Animator _revButtonAnimator;

	private bool _isForward = true;
	
	public void ToggleReversingAnim()
	{
		_isForward = !_isForward;

		if (_isForward)
		{
			_revButtonAnimator.Play(ANIMATION_REV_D, 0, 0);
		}
		else
		{
			_revButtonAnimator.Play(ANIMATION_REV_R, 0, 0);
		}
	}
}

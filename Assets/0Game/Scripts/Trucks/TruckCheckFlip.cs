using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckCheckFlip : MonoBehaviour
{
	public event Action OnTruckFlipped;

    [SerializeField] private float flipCheckDelay = 0.25f;
    [SerializeField] private float minimumYThreshold = 0.1f;

	private bool _isCheckingFlip = false;

	private void Update()
	{
		if (!_isCheckingFlip)
		{
			_isCheckingFlip = true;

			StartCoroutine(CheckIfFlip());
		}
	}

	private IEnumerator CheckIfFlip()
	{
		if (gameObject.transform.up.y < minimumYThreshold)
		{
			OnTruckFlipped?.Invoke();
		}

		yield return Yielder.GetWaitForSeconds(flipCheckDelay);

		_isCheckingFlip = false;
	}
}

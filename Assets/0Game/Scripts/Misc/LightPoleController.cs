using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class LightPoleController : MonoBehaviour
{
	[SerializeField] private bool _isActive;
	[SerializeField] private Light _light;
    [SerializeField] private float _chanceLightTurningOff = 0.1f;
    [SerializeField] private float _chanceLightStuttering = 0.4f;
    [SerializeField] private Vector2 _lightStutterOffTimeFrame = new Vector2(0.1f, 0.2f);
    [SerializeField] private Vector2 _lightStutterOnTimeFrame = new Vector2(0.05f, 0.6f);
    [SerializeField] private Vector2 _lightStutterCounts = new Vector2(2, 6);
	[SerializeField] private float _lightTurningOffTimeExtend = 1f;

    private bool _isLightOn = true;
    private int _currentLightStutterCount = 0;
	private float _currentTimeDelay = 0f;
	private bool _isLightStuttering = false;
	private bool _isLightTurningOff = false;

	private void Start()
	{
		if (!_isActive)
		{
			Destroy(this);
			return;
		}

		_light.enabled = true;
		_isLightOn = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && _isLightOn)
		{
			float oddCalculation = Random.Range(0f, 1f);
			//Debug.Log("oddCalculation: " + oddCalculation);

			_isLightStuttering = oddCalculation <= _chanceLightStuttering;
			_isLightTurningOff = oddCalculation <= _chanceLightTurningOff;

			if (_isLightStuttering)
			{
				StartCoroutine(LightStutterCoroutine());
			}
		}
	}

	private IEnumerator LightStutterCoroutine()
	{
		//Debug.Log("Stuttering");

		_currentLightStutterCount = Random.Range((int)_lightStutterCounts.x, (int)_lightStutterCounts.y);

		while (_currentLightStutterCount-- > 0)
		{
			_currentTimeDelay = (float)Math.Round(Random.Range(_lightStutterOffTimeFrame.x, _lightStutterOffTimeFrame.y), 1);

			_light.enabled = false;

			yield return Yielder.GetWaitForSeconds(_currentTimeDelay);

			_currentTimeDelay = (float)Math.Round(Random.Range(_lightStutterOnTimeFrame.x, _lightStutterOnTimeFrame.y), 1);
			if (_currentLightStutterCount == 0 && _isLightTurningOff)
			{
				_currentTimeDelay += _lightTurningOffTimeExtend;
			}
			_light.enabled = true;
			yield return Yielder.GetWaitForSeconds(_currentTimeDelay);
		}

		if (_isLightTurningOff)
		{
			_light.enabled = false;
			_isLightOn = false;
		}
	}
}

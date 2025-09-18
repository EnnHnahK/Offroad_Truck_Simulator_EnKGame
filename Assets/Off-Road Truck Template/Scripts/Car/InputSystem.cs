//--------------------------------------------------------------
//
//                    Truck Parking kit
//        
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class InputSystem : MonoBehaviour
{
	// Accelerometer controlling
	//[Header("Accelerometer")]
	// public float accelSensibility = 10f;
	// public float accelSmooth = 0.5f;
	// public GameObject sWheel;
	[Header("Components")]
	public SteeringWheel steeringWheel;
	private float motorInputForwardRatio = 1f;
	private float motorInputBackwardRatio = 1f;
	[SerializeField] private GameObject breakActive;
	[SerializeField] private GameObject breakInactive;

	//private VehicleController2017 controller;
	private float motorInput, steerInput;
	private bool handBrake, reversing;
	//private Vector3 curAc;
	//private float GetAxisH = 0f;
	//private bool accelInput;
	private bool _isLevelUpdated = false;
	private bool _isVehicleSpawned = false;

	public void SetInsLv(bool complete)
	{
		_isLevelUpdated = complete;
	}

	public void SetInsVehicle(bool complete)
	{
		_isVehicleSpawned = complete;
		if (_isVehicleSpawned)
		{
			SetInforStatus();
			steeringWheel.ResetAngle();
		}
	}

	private void SetInforStatus()
	{
		motorInput = 0f;
		steerInput = 0f;
		handBrake = false;
		//controller = _TruckManager.Instance.GetVehicleController();
	}
	
	void Update()
	{
		if (!_isLevelUpdated || !_isVehicleSpawned)
		{
			return;
		}

		// if (accelInput)
		// {
		// 	curAc = Vector3.Lerp(curAc, Input.acceleration - Vector3.zero, Time.deltaTime / accelSmooth);
		// 	GetAxisH = Mathf.Clamp(curAc.x * accelSensibility, -1, 1);
		// 	steerInput = GetAxisH;
		// }

		steerInput = steeringWheel.GetClampedValue();

		if (RaceManager.Instance.RaceRunning())
		{
			Debug.LogError("dang tat controller vao sua");
			// controller.Move(motorInput, steerInput, handBrake);
			// controller.PlayParSwamp(motorInput != 0);
		}
		
	}

	public void Throttle()
	{
		if (!reversing)
			motorInput = motorInputForwardRatio;
		else
			motorInput = -motorInputBackwardRatio;
	}

	public void ThrottleRelease()
	{
		motorInput = 0;
	}

	public void Steer(bool state)
	{
		steerInput = Mathf.Lerp(steerInput, state ? 1f : - 1, Time.deltaTime * 34);
	}

	public void SteerRelease()
	{
		steerInput = 0;
	}

	public void Brake(bool state)
	{
		handBrake = state;

		if (state)
		{
			breakActive.SetActive(true);
			breakInactive.SetActive(false);
		}
		else
		{
			breakActive.SetActive(false);
			breakInactive.SetActive(true);
		}
	}

	public void ToggleReversing()
	{
		reversing = !reversing;
	}
}
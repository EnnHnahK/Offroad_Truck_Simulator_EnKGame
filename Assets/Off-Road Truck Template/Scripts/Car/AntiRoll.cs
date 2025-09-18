using UnityEngine;
using System.Collections;

public class AntiRoll : MonoBehaviour {

	[SerializeField] private WheelCollider _wheelL;
	[SerializeField] private WheelCollider _wheelR;
	[SerializeField] private float _antiRollValue = 3000;

	private Rigidbody _rigidbody;

	private void Start()
	{
		_rigidbody = _TruckManager.Instance.GetRigidbody();
	}

	private void FixedUpdate ()
	{
		WheelHit hit;
		float travelL = 1.0F;
		float travelR = 1.0F;

		bool groundedL = _wheelL.GetGroundHit(out hit);
		if (groundedL) 
		{
			travelL = (-_wheelL.transform.InverseTransformPoint(hit.point).y - _wheelL.radius) / _wheelL.suspensionDistance;
		}
		bool groundedR = _wheelR.GetGroundHit(out hit);
		if (groundedR) 
		{
			travelR = (-_wheelR.transform.InverseTransformPoint(hit.point).y - _wheelR.radius) / _wheelR.suspensionDistance;
		}

		float antiRollForce = (travelL - travelR) * _antiRollValue;

		if (groundedL)
		{
			_rigidbody.AddForceAtPosition(_wheelL.transform.up * -antiRollForce, _wheelL.transform.position);
		}
		if (groundedR)
		{
			_rigidbody.AddForceAtPosition(_wheelR.transform.up * -antiRollForce, _wheelR.transform.position);
		}
	}
}


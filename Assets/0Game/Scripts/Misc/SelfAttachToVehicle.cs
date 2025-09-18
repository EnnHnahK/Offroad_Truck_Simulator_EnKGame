using System;
using System.Collections;
using UnityEngine;

public class SelfAttachToVehicle : MonoBehaviour
{
	IEnumerator Start()
	{
		yield return new WaitUntil(() => _TruckManager.Instance);
		yield return new WaitUntil(() => _TruckManager.Instance.GetVehicleControl());
		SelfAttachVehicle();
	}


	private void SelfAttachVehicle()
	{
		if (_TruckManager.Instance.GetVehicleControl())
		{
			transform.SetParent(_TruckManager.Instance.GetVehicleControl().transform);
		}
		else
		{
			Destroy(this);
		}
		transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		transform.localScale = Vector3.one;

		_TruckManager.Instance.GetTruckOnCollider().OnTruckWin += () =>
		{
			transform.SetParent(LevelController.Instance.transform);
			transform.position = Vector3.zero;
		};
	}
}
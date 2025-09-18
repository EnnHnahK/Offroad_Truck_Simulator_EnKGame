using UnityEngine;
using System.Collections;

public class LightManager : MonoBehaviour {

	public Transform target;

	public LightShaft[] lighShaft;

	void Start ( ) {
		target = GameObject.Find("Main Camera").transform;
	}

	public void SetTarget(Transform val)
	{
		for (int a = 0; a < lighShaft.Length; a++)
			lighShaft [a].target = val;
	}

}

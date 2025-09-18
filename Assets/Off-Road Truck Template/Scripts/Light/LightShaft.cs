using UnityEngine;
using System.Collections;

public class LightShaft : MonoBehaviour {

	public Transform target;
	public LightManager manager;

	IEnumerator Start ( ) {
		
		mr = GetComponent<MeshRenderer> ();

		yield return new WaitForEndOfFrame ();

	//	target = GameObject.FindGameObjectWithTag ("Player").transform;
		target = Camera.main.transform;
	}
	
	public float startDistance = 100f,fadeSpeed = 30f;

	public float distance = 0;

	public MeshRenderer mr;

	void Update ( ) 
	{
		if (!target)
			return;
		
		if (canCompute) {

			if(Camera.main)
				target = Camera.main.transform;

			distance = Vector3.Distance (transform.position, target.position);

			if (distance <= startDistance) {

				if (mr.material.GetColor ("_TintColor").a > 0)
					mr.material.SetColor ("_TintColor", new Color (1f, 1f, 1f, mr.material.GetColor ("_TintColor").a - (fadeSpeed * Time.deltaTime))) ;
			} else {

				if (mr.material.GetColor("_TintColor").a < 1f)
					mr.material.SetColor("_TintColor", new Color (1f, 1f, 1f, mr.material.GetColor("_TintColor").a + (fadeSpeed * Time.deltaTime)));
				
			}
		}
	}

	public bool canCompute;

	void OnBecameVisible() {
		canCompute = true;
	}
	void OnBecameInvisible() {
		canCompute = false;
	}
}

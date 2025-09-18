using UnityEngine;
using System.Collections;

public class RainLights : MonoBehaviour {

	// Use this for initialization
	public float minTime,maxTime;
	public Light rLight;

	public float maxIntensity = 1f;

	IEnumerator Start () {
	

		while (true) {

			yield return new WaitForSeconds (maxTime);

			rLight.intensity = maxIntensity;

			yield return new WaitForSeconds (minTime);

			rLight.intensity = 0;

			yield return new WaitForSeconds (minTime);

			rLight.intensity = maxIntensity;

			yield return new WaitForSeconds (minTime);

			rLight.intensity = 0;

		}
	}

}

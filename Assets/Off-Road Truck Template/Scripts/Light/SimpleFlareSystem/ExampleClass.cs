using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour {

	// Draw a yellow sphere in the scene view at the position
	// on the near plane of the selected camera that is
	// 100 pixels from lower-left.



		void OnDrawGizmos() {
			Camera camera = GetComponent<Camera>();
			Vector3 p = camera.ScreenToWorldPoint(new Vector3(100, 100, camera.nearClipPlane));
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(p, 0.1f);
		}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LB_Wheel_Deformer : MonoBehaviour 
{
	bool canUpdate;
	public MeshRenderer WheelMesh;
	public float updateDelay = 0.1f;
	public float widthMultiplier ;
	// Higher is lowe   r   
	public float intensityDivide = 10f;
	public TerrainDeformer targetTerrain;
	public bool updateTexture;

	// provate variable s
	WheelHit hit;
	WheelCollider col;
	Rigidbody rigid;

	void Start () {
		targetTerrain = GameObject.FindObjectOfType<TerrainDeformer>();
		if (!targetTerrain)
			Destroy(GetComponent<LB_Wheel_Deformer>());

		canUpdate = true;
		col = GetComponent<WheelCollider> ();
		rigid = GetComponentInParent<Rigidbody> ();

	}

	// Update is called once per frame
	Quaternion q;
	Vector3 p;
	void Update () {
		if (canUpdate) {
			canUpdate = false;
			StartCoroutine (UpdateTerrain ());
			if (WheelMesh == null)
			{
#if UNITY_EDITOR
				Debug.LogError("wheel mesh dang null");
#endif
				return;
			}
			if (rigid.velocity.magnitude > 0.5f) {
				col.GetGroundHit (out hit);
				targetTerrain.DestroyTerrain (hit.point, WheelMesh.bounds.size.x + widthMultiplier, rigid.velocity.magnitude / intensityDivide, updateTexture);
			}
		}
	}

	/*
	void OnCollisionEnter(Collision other)
	{
		if (canUpdate) {
			canUpdate = false;
			StartCoroutine (doAgain ());
			if (other.collider.GetComponent<TerrainDeformer> () != null) {
				other.collider.GetComponent<TerrainDeformer> ().DestroyTerrain (other.contacts [0].point, transform.GetComponent<MeshRenderer> ().bounds.size.x);
			}
		}
	}
*/
	IEnumerator UpdateTerrain()
	{
		yield return new WaitForSeconds (updateDelay);
		canUpdate = true;
	}
}

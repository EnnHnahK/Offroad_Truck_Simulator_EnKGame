
using System;
using System.Collections;
using System.Collections.Generic;
using NWH.WheelController3D;
using Unity.VisualScripting;
using UnityEngine;

public class WheelDeformer : MonoBehaviour
{
	[SerializeField] private MeshRenderer wheelMesh;

	[SerializeField] private float widthMultiplier;
	[SerializeField] private bool updateTexture = true;
	private float updateDelay = 0.1f;
	private float intensityDivide = 10.0f;

	private TerrainDeformer targetTerrain;
	private bool canUpdate = true;
	private WheelController wheelController;
	private Rigidbody truckRigidbody;
	private static List<WheelDeformer> wheels = new List<WheelDeformer>();
	private static List<WheelDeformer> wheelsInMud = new List<WheelDeformer>();


    void Start()
	{
		wheels.Add(this);
        wheelsInMud.Clear();
        targetTerrain = LevelController.Instance.terrainDeform; //GameObject.FindObjectOfType<TerrainDeformer>();
		if (!targetTerrain)
		{
			Destroy(this);
		}

        //col = GetComponent<WheelCollider>();
        //truckRigidbody = _TruckManager.Instance.GetRigidbody();
        wheelController = GetComponent<WheelController>();
		if (wheelController == null)
		{
			Destroy(this);
			return;
		}
		truckRigidbody = transform.parent.GetComponent<Rigidbody>();
		//dragCounter = dragDelay;
	}

	//private float dragDelay = .5f;
	//private float dragCounter;
	void Update()
	{
		if (canUpdate && _TruckManager.Instance)
		{
			canUpdate = false;
			StartCoroutine(UpdateTerrain());
            if (truckRigidbody.velocity.magnitude > 0.05f)
            {
                //col.GetGroundHit(out hit);
                if (wheelsInMud.Count > 0)
                {
                    _TruckManager.Swamp = true;
                    var size = Mathf.Abs(wheelMesh.localBounds.size.x * wheelMesh.transform.lossyScale.x);
                    size = Mathf.Max(0.8f, size);
                    //var distance = Vector3.Distance(wheelController.HitPoint, wheelController.transform.position);
                    targetTerrain.DestroyTerrain(wheelController.HitPoint, size + widthMultiplier, truckRigidbody.velocity.magnitude / intensityDivide, updateTexture);
                    if (_TruckManager.Instance.GetVehicleFunction().DragType != DragType.Water)
                        _TruckManager.Instance.GetVehicleFunction().Slow(DragType.Mud);
                }
                else
                {
                    _TruckManager.Swamp = false; 
					if (_TruckManager.Instance.GetVehicleFunction().DragType != DragType.Water)
                        _TruckManager.Instance.GetVehicleFunction().Slow(DragType.None);
                }
            }
		}
	}
	
	/*void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(wheelController.HitPoint, Mathf.Abs(wheelMesh.localBounds.size.x * wheelMesh.transform.lossyScale.x));     

    }*/

    IEnumerator UpdateTerrain()
	{
		yield return Yielder.GetWaitForSeconds(updateDelay);
		canUpdate = true;
	}

	private void OnDestroy()
	{
		wheels.Clear();
		wheelsInMud.Clear();
	}

#if UNITY_EDITOR

	public bool meshInChild = true;

	[Button]
	private void SetUpDeformer()
	{
		var visual = GetComponent<WheelController>().WheelVisual;

		if (visual == null)
		{
			Debug.LogError("Unable to get Wheel Controller component");
			return;
		}

		Transform wheelTransform;

		wheelTransform = meshInChild ? visual.transform.GetChild(0) : visual.transform;

		if (wheelTransform == null)
		{
			Debug.LogError("Unable to get Wheel Object Transform component");
			return;
		}

		var mesh = wheelTransform.GetComponent<MeshRenderer>();

		if (mesh == null)
		{
			Debug.LogError("Unable to get Mesh Renderer component");
		}
		else
		{
			wheelMesh = mesh;
		}
	}

#endif

    private void OnTriggerEnter(Collider other)
    {
		if (other.name.Equals("Terrain") && !wheelsInMud.Contains(this) )
        {
            wheelsInMud.Add(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Equals("Terrain") && wheelsInMud.Contains(this) )
        {
            wheelsInMud.Remove(this);
        }
    }
}

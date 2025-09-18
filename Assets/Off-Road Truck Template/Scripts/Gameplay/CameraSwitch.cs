//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

// This script used for switc between cameras

using UnityEngine;
using System.Collections;
using System;

public class CameraSwitch : MonoBehaviour
{

	[SerializeField] private float _xIdleBodyOffset;
	[SerializeField] private Vector3 _bodyOffset = Vector3.zero;
	[SerializeField] private Vector3 _aimOffset = Vector3.zero;

	// MainCamera
	//public GameObject mainCamera;
	//public float cameraDistance = 21.6f;
	//public float cameraheight = 7.19f;

	public event Action<bool> OnCameraSwitch;

	[Header("The first slot should be empty")]
	[Header("Because is used for Main camera")]
	[Space(3)]

	[Header("Camera List:")]
	// List of the camera's gameObjects
	public GameObject[] cameras;

	// Hold curent active camera id
	//int currentCamera = 0;

	//FlareLookAt[] flares;
	private int _curId = 0;


	void Start()
	{
		/*GameObject[] temp = GameObject.FindGameObjectsWithTag ("Flare");

		flares = new FlareLookAt[temp.Length];


		for (int a = 0; a < temp.Length; a++)
			flares[a] = temp [a].GetComponent<FlareLookAt> ();
		*/

		cameras[0] = Camera.main.gameObject;//GameObject.FindWithTag("MainCamera");

		CinemachineManager.Instance.SetUpVirtualCamera(_xIdleBodyOffset, _bodyOffset, _aimOffset);
	}
	
	// Diactivate all cameras and activate current selected
	public void SelectCamera(int id)
	{
		
		CinemachineManager.Instance.SetBindingMode(id != 0);
		//for (int i = 0; i < cameras.Length; i++)
			cameras [_curId].SetActive (false);

		_curId = id;
		cameras [id].SetActive (true);

		OnCameraSwitch?.Invoke(id == cameras.Length - 1);

		/*for (int a = 0; a < flares.Length; a++)
			flares [a].cam = cameras [id].transform;*/
	}

	public int GetLengthCam()
	{
		return cameras.Length;
	}
}

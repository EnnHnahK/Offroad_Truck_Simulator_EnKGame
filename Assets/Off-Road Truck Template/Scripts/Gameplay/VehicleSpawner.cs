//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class VehicleSpawner : MonoBehaviour {

	public GameObject[] vehicles;
	public Transform spawnPoint;



	void Awake () {
		Instantiate (vehicles [PlayerPrefs.GetInt ("TruckID")], spawnPoint.position, spawnPoint.rotation);
	}

}
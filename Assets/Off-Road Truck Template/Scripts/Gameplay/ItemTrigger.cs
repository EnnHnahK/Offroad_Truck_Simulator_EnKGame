//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class ItemTrigger : MonoBehaviour {
	// Use this for initialization
	public ItemManager manager;

	void OnTriggerEnter (Collider col) {
		if (col.CompareTag("Player")) {
			manager.TotalFuel = 100000;
			manager.Alarm.volume = 1;
			//		Their game doesn't notify checkpoint
			//manager.Alarm.Play();
		}
	}
	
	// Update is called once per frame
	void Start () {
		if (!manager)
			manager = GameObject.FindObjectOfType<ItemManager> ();
	}
}

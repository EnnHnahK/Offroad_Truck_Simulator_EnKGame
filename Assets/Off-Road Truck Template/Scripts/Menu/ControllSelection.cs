//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class ControllSelection : MonoBehaviour {

	public void SelectControl (int id) 
	{

		PlayerPrefs.SetInt ("Controll", id);
	}
	public void SetFalse(GameObject target)
	{
		target.SetActive (false);
	}


}

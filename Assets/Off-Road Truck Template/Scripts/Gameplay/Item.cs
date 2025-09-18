//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	[SerializeField] private int value = 1;

	public int GetValue()
	{
		return value;
	}
}

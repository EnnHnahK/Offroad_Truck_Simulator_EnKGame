//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

// This script used for color picking system in game menu(garage)

using UnityEngine;

public class ColorPicker : MonoBehaviour {

	// List of the colors
	public Color[] Colors;


	// Public function for changing color buttons
	public void SetColor (int id)
	{
			PlayerPrefs.SetInt ("TruckColor" + PlayerPrefs.GetInt ("TruckID").ToString (), id);
		
 			GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<ColorLoader>().mat.color = Colors [id];

	}
}

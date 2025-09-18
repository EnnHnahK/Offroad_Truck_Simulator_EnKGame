using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerUp : MonoBehaviour {

	float  startTime ;
	string  textTime ;
	
	 float guiTime; 

	//The gui-Time is the difference between the actual time and the start time.
	float  minutes ;
	float  seconds ;

	//Create a reference for the textfield
	public Text textField ;


	void Start() {
		startTime = Time.time;
	}
	   



	void Update () {
		guiTime = Time.time - startTime;
		//The gui-Time is the difference between the actual time and the start time.
		minutes = Mathf.Floor ( guiTime / 60); //Divide the guiTime by sixty to get the minutes.
		seconds = Mathf.Floor (guiTime % 60);//Use the euclidean division for the seconds.

		textTime = string.Format ("{0:00}:{1:00}", minutes, seconds);
		//text.Time is the time that will be displayed.
		textField.text = textTime;
	}
}

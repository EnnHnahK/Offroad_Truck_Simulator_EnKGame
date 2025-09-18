//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour {


	public int TotalScore;

	public Text ScoreText,FinishMenuText;

	public Slider FuelSlider;

	public Text FuelText;




	public int TotalFuel = 100000;
	public float FualInterval = 0.3f;

	public AudioSource Alarm;

	public GameObject finishMenu;

	public Text timerText,bestTime,currentTime;


	private void Start () {
		startTime = Time.time;
		
		TotalScore = PlayerPrefs.GetInt ("Coins");
		ScoreText.text = TotalScore.ToString ();

		FuelSlider.value = TotalFuel;
	}

	// Update is called once per frame
	public void AddScore (int value) {
		TotalScore += value;
		ScoreText.text = TotalScore.ToString ();
	}

	void Update()
	{
		TimeUP ();
	}

	float  startTime ;
	string  textTime ;

	float guiTime; 

	//The gui-Time is the difference between the actual time and the start time.
	float  minutes ;
	float  seconds ;

	public void TimeUP ()
	{    
		guiTime = Time.time - startTime;
		//The gui-Time is the difference between the actual time and the start time.
		minutes = Mathf.Floor ( guiTime / 60); //Divide the guiTime by sixty to get the minutes.
		seconds = Mathf.Floor (guiTime % 60);//Use the euclidean division for the seconds.

		textTime = string.Format ("{0:00}:{1:00}", minutes, seconds);
		//text.Time is the time that will be displayed.
		timerText.text = textTime;
	}

	    
	public void SaveBestTime()
	{
		if (PlayerPrefs.GetInt ("First" + PlayerPrefs.GetInt ("LevelID").ToString ()) != 3) {
			PlayerPrefs.SetFloat ("Minutes" + PlayerPrefs.GetInt ("LevelID").ToString (), minutes);
			PlayerPrefs.SetFloat ("Seconds" + PlayerPrefs.GetInt ("LevelID").ToString (), seconds);
			PlayerPrefs.SetInt ("First" + PlayerPrefs.GetInt ("LevelID").ToString (), 3);
		} else {
			if (PlayerPrefs.GetFloat ("Minutes" + PlayerPrefs.GetInt ("LevelID").ToString ()) == minutes) {
				if (PlayerPrefs.GetFloat ("Seconds" + PlayerPrefs.GetInt ("LevelID").ToString ()) != seconds) {
					if (PlayerPrefs.GetFloat ("Seconds" + PlayerPrefs.GetInt ("LevelID").ToString ()) > seconds)
						PlayerPrefs.SetFloat ("Seconds" + PlayerPrefs.GetInt ("LevelID").ToString (), seconds);
				}
			}
			{
				if (PlayerPrefs.GetFloat ("Minutes" + PlayerPrefs.GetInt ("LevelID").ToString ()) > minutes) {
					PlayerPrefs.SetFloat ("Minutes" + PlayerPrefs.GetInt ("LevelID").ToString (), minutes);
					PlayerPrefs.SetFloat ("Seconds" + PlayerPrefs.GetInt ("LevelID").ToString (), seconds);
				}
			}
		}
	}

	public string ReadBestTime()
	{
		float min, secn;

		min	= PlayerPrefs.GetFloat ("Minutes" + PlayerPrefs.GetInt ("LevelID").ToString ());
		secn  = PlayerPrefs.GetFloat ("Seconds" + PlayerPrefs.GetInt ("LevelID").ToString ());

		string minS,secS;

		minS = min.ToString ();
		secS = Mathf.Floor(secn).ToString ();

		if (min < 10)
			minS = "0" + min.ToString ();

		if (secn < 10)
			secS = "0" + Mathf.Floor(secn).ToString ();
	
		return  minS + ":"+secS;
	}

	public string ReadCurrentTIme()
	{
		string min;
		string sec;

		if (minutes < 10)
			min = "0" + minutes.ToString ();
		else
			min = minutes.ToString ();

		if (seconds < 10)
			sec = "0" +( Mathf.FloorToInt(seconds)).ToString ();
		else
			sec = (Mathf.FloorToInt(seconds)).ToString ();

		return min + ":"+ sec;
	}
}
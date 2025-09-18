//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

// This script used for level selection and lock system in game menu

using UnityEngine;
using System.Collections;   
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{

	// Array of locks
	public GameObject[] Locks;
	    
	// Show total coins on top of te menu
	public Text TotalCoins;

	// Each level price to unlock
	public int[] levelValues;

	// Show this menu when coins are not enouph to but selected level
	public GameObject Shop;

	// Next menu for activat it
	public GameObject LevelSelectMenu, CarSelectMenu;

	// Night mode togle
	public Toggle nightMode;

	public Text[] bestTime;

	void Start ()
	{

		// Update coins text on start
		TotalCoins.text = PlayerPrefs.GetInt ("Coins").ToString (); 

		// Cheak levels locks on start
		for (int a = 0; a < Locks.Length; a++) {

			if (PlayerPrefs.GetInt ("Level" + a.ToString ()) == 3) // 3=>true - 0=>false 
				Locks [a].SetActive (false);

			float min = PlayerPrefs.GetFloat ("Minutes" + a.ToString ());
			float secn = PlayerPrefs.GetFloat ("Seconds" + a.ToString ());

			string minS, secS;

			minS = min.ToString ();
			secS = Mathf.Floor (secn).ToString ();

			if (min < 10)
				minS = "0" + min.ToString ();

			if (secn < 10)
				secS = "0" + Mathf.Floor (secn).ToString ();


			bestTime [a].text = "Best Time " +  (minS + ":" + secS)
				.ToString ();
		}

		// Load night mode cheak box value on start
		if (PlayerPrefs.GetInt ("NightMode") == 3) // 3=>true - 0=>false 
			nightMode.isOn = true;
		else
			nightMode.isOn = false;
	}

	// Set night mode on check box value changed by user (on or off)    
	public void SetNightMode()
	{
		StartCoroutine (SaveNightMode ());
	}

	// Save night mode cheak box value
	IEnumerator SaveNightMode()
	{

		yield return new WaitForEndOfFrame();
		if (nightMode.isOn)
			PlayerPrefs.SetInt ("NightMode", 3);  // 3 => true
		else
			PlayerPrefs.SetInt ("NightMode", 0);  // 0 => false
	}



	public void SelectLevel(int id)
	{

		if (PlayerPrefs.GetInt ("Level" + id.ToString ()) == 3) { // 3=>true - 0=>false 
			CarSelectMenu.SetActive (true);
			PlayerPrefs.SetInt ("LevelID", id);
			LevelSelectMenu.SetActive (false);
		} else {
			if (PlayerPrefs.GetInt ("Coins") >= levelValues [id]) {

				Locks [id].SetActive (false);
				PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") - levelValues [id]);
				TotalCoins.text = PlayerPrefs.GetInt ("Coins").ToString ();
				PlayerPrefs.SetInt ("Level" + id.ToString (),3 );
			} else
				Shop.SetActive (true);

		}
	}
}

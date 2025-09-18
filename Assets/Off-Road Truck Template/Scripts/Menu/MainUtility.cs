//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

// This script used for main utilities used in game menus

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainUtility : MonoBehaviour
{
	
	public int startingScore = 1400;
	void Awake()
	{
		
		// Is game first run?   3 => true    0 => false
		if (PlayerPrefs.GetInt("FirstRun") != 3)
		{

			// Open first level
			PlayerPrefs.SetInt("Level0", 3);

			// Set Sea active in settings true
			PlayerPrefs.SetInt("Sea", 3);

			// Open first car
			PlayerPrefs.SetInt("Truck0", 3);

			// Player starting first time coins
			PlayerPrefs.SetInt("Coins", startingScore);

			PlayerPrefs.SetInt("FirstRun", 3);

		}
}
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.H)) {
			PlayerPrefs.DeleteAll ();
			Debug.Log ("PlayerPrefs.DeleteAll ();");
		}
		if (Input.GetKeyDown (KeyCode.E))
			PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") + 14000);
	}

	public void Exit ()
	{
		Application.Quit ();
	}

	public void SetTrue (GameObject target)
	{
		target.SetActive (true);
	}

	public void SetFalse (GameObject target)
	{
		target.SetActive (false);
	}

	public void ToggleObject (GameObject target)
	{
		target.SetActive (!target.activeSelf);
	}

	public void LoadLevel (string name)
	{
		SceneManager.LoadScene (name);
	}

	public void OpenURL (string val)
	{
		Application.OpenURL (val);
	}
}

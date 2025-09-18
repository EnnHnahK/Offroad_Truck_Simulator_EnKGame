//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	public GameObject Loading;

	public void LoadLevel(string name)
	{
		Loading.SetActive (true);
		UnityEngine.SceneManagement.SceneManager.LoadScene (name);

	}
	public void Exit()
	{
		Application.Quit ();
	}
}

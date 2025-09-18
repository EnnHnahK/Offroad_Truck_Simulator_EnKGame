using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class LevelLoader : MonoBehaviour {

	public GameObject[] Levels;  

	void Start () {

		if (SceneManager.GetActiveScene ().name.Contains ("Garage") ||
			SceneManager.GetActiveScene ().name.Contains ("Menu"))
			return;

		int id = PlayerPrefs.GetInt("LevelID");
		Levels [id].SetActive (true);
		Levels [id].transform.parent = null;
	}
}

//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

// This script used for Vehicle selection system in game menu(garage)

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VehicleSelect : MonoBehaviour
{


	// Vehicle prefabs array
	public GameObject[] vehicles;

	// SpawnPoint
	public Transform point;

	// Ech Vehicle value
	public int[] Values;

	// Lock Icon,Shop window,Buy button
	public GameObject Lock, Shop, Buy;

	// Selected Vehicle ID
	int ID;

	//TotalScore text, Vehicle value text
	public Text TotalScore, VehicleValue;
	
	// SetActive(true) loading window before start loading level
	public GameObject Loading;

	// MainLevel name
	public string LevelNameDay = "MainLevel",LevelNameNight = "MainLevel";


	void Start ()
	{
		
		ID = PlayerPrefs.GetInt ("TruckID");

		// Instantiate last selected Vehicle by saved ID
		Instantiate (vehicles [ID], point.position, point.rotation);

		// Update total score text
		TotalScore.text = PlayerPrefs.GetInt ("Coins").ToString ();

		// Update current Vehicle value text
		VehicleValue.text = Values [ID].ToString ();


		// Update current Vehicle is locked or not
			if (PlayerPrefs.GetInt ("Truck" + ID.ToString ()) == 3) {
				Lock.SetActive (false);
				Buy.SetActive (false);
			} else {
				Lock.SetActive (true);
				Buy.SetActive (true);
			}

	}
	// Public function for NextVehicle select button in menu
	public void NextVehicle ()
	{
		if (ID < vehicles.Length - 1)
			ID++;


			PlayerPrefs.SetInt ("TruckID", ID);
		
		Destroy (GameObject.FindGameObjectWithTag ("Player"));
		Instantiate (vehicles [ID], point.position, point.rotation);


			if (PlayerPrefs.GetInt ("Truck" + ID.ToString ()) == 3) {
				Lock.SetActive (false);
				Buy.SetActive (false);
			} else {
				Lock.SetActive (true);
				Buy.SetActive (true);
			}



		VehicleValue.text = Values [ID].ToString ();

	}
	// Public function for PrevVehicle select button in menu
	public void PrevVehicle ()
	{

		if (ID > 0)
			ID--;

		PlayerPrefs.SetInt ("TruckID", ID);

		Destroy (GameObject.FindGameObjectWithTag ("Player"));
		Instantiate (vehicles [ID], point.position, point.rotation);

			if (PlayerPrefs.GetInt ("Truck" + ID.ToString ()) == 3) {
				Lock.SetActive (false);
				Buy.SetActive (false);
			} else {
				Lock.SetActive (true);
				Buy.SetActive (true);
			}


		VehicleValue.text = Values [ID].ToString ();

	}
	// Select current Vehicle
	public void SelectVehicle ()
	{
		// If selected Vehicle is open,then Start game
			if (PlayerPrefs.GetInt ("Truck" + ID.ToString ()) == 3) {

			// Set current selected Vehicle ID for instantiate in main level    
				PlayerPrefs.SetInt ("TruckID", ID);

				// Activate loading screen
				Loading.SetActive (true);

				if (PlayerPrefs.GetInt ("NightMode") == 3) { // 3=>true  , 0 =>false
					SceneManager.LoadScene (LevelNameNight);
				} else {
					SceneManager.LoadScene (LevelNameDay);
				}

			}
		
	}

	
	// Buy current selected Vehicle
	public void BuyVehicle ()
	{
		// If player have enough money, buy selected Vehicle
			if (Values [ID] <= PlayerPrefs.GetInt ("Coins")) {

				PlayerPrefs.SetInt ("Truck" + ID.ToString (), 3);

				PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") - Values [ID]);
				{
					Lock.SetActive (false);
					Buy.SetActive (false);
				}

				TotalScore.text = PlayerPrefs.GetInt ("Coins").ToString ();



			} else// If player did't have enough money, Show shop offer window   
				Shop.SetActive (true);
		
	}
}

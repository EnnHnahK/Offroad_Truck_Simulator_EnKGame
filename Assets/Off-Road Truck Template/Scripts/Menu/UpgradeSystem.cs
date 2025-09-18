using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpgradeSystem : MonoBehaviour {


	// Temp for upgrades
	int tEngine,tSpeed,tFuel;
	[Header("Vehicle Upgrade System")]
	[Space(3)]
	public int maxUpgradeLevel = 10;
	[Space(3)]
	public Text ScoreTXT,EngineTXT,SpeedTXT,FuelTXT;

	public int[] enginePrice,speedPrice,fuelPrice;

	public Text enginePriceText, speedPriceText, fuelPriceText;

	[Space(3)]
	public GameObject Shop;

	void Start () {

		// Load upgrades on start 
		LoadUpgrade ();
	}

	// Load upgrades
	public  void LoadUpgrade()
	{
		tEngine = PlayerPrefs.GetInt ("Engine" + PlayerPrefs.GetInt ("TruckID"));
		EngineTXT.text = tEngine.ToString ()+" /"+maxUpgradeLevel.ToString();
		tSpeed = PlayerPrefs.GetInt ("Speed" + PlayerPrefs.GetInt ("TruckID"));
		SpeedTXT.text = tSpeed.ToString ()+" /"+maxUpgradeLevel.ToString();
		tFuel = PlayerPrefs.GetInt ("Fuel" + PlayerPrefs.GetInt ("TruckID"));
		FuelTXT.text = tFuel.ToString ()+" /"+maxUpgradeLevel.ToString();
		ScoreTXT.text = PlayerPrefs.GetInt ("Coins").ToString();

		if(tEngine<10)
			enginePriceText .text = enginePrice [tEngine].ToString () + " $";
		else
			enginePriceText .text = "Completed";
		
		if(tSpeed<10)
			speedPriceText .text = speedPrice [tSpeed].ToString () + " $";
		else
			speedPriceText .text = "Completed";

		if(tFuel<10)
			fuelPriceText .text = fuelPrice [tFuel].ToString () + " $";
		else
			fuelPriceText .text = "Completed";
	}

	// Upgrade Engine
	public void EngineUpgrade()
	{
		if(tEngine<maxUpgradeLevel)
		{
			print (tEngine.ToString ());
			if(PlayerPrefs.GetInt ("Coins")>enginePrice[tEngine])
			{
				PlayerPrefs.SetInt ("Coins",PlayerPrefs.GetInt ("Coins")-enginePrice[tEngine]);
				tEngine++;
				PlayerPrefs.SetInt ("Engine"+PlayerPrefs.GetInt ("TruckID").ToString(),tEngine);
				ScoreTXT.text = PlayerPrefs.GetInt ("Coins").ToString();
				EngineTXT.text = tEngine.ToString ()+" /7";

				if(tEngine<10)
					enginePriceText .text = enginePrice [tEngine].ToString () + " $";
				else
					enginePriceText .text = "Completed";
			}
			else
				Shop.SetActive(true);
		}
	}

	// Upgrade Speed
	public void SpeedUpgrade()
	{
		if(tSpeed<maxUpgradeLevel)
		{print (tSpeed.ToString ());
			if(PlayerPrefs.GetInt ("Coins")>speedPrice[tSpeed])
			{
				PlayerPrefs.SetInt ("Coins",PlayerPrefs.GetInt ("Coins")-speedPrice[tSpeed]);
				tSpeed++;
				PlayerPrefs.SetInt ("Speed"+PlayerPrefs.GetInt ("TruckID").ToString(),tSpeed);
				ScoreTXT.text = PlayerPrefs.GetInt ("Coins").ToString();
				SpeedTXT.text = tSpeed.ToString ()+" /7";

				if(tSpeed<10)
					speedPriceText .text = speedPrice [tSpeed].ToString () + " $";
				else
					speedPriceText .text = "Completed";
			}
			else
				Shop.SetActive(true);
		}
	}

	// Upgrade Fuel
	public void FuelUpgrade()
	{
		if(tFuel<maxUpgradeLevel)
		{
			if(PlayerPrefs.GetInt ("Coins")>fuelPrice[tFuel])
			{
				PlayerPrefs.SetInt ("Coins",PlayerPrefs.GetInt ("Coins")-fuelPrice[tFuel]);
				tFuel++;
				PlayerPrefs.SetInt ("Fuel"+PlayerPrefs.GetInt ("TruckID").ToString(),tFuel);
				ScoreTXT.text = PlayerPrefs.GetInt ("Coins").ToString();
				FuelTXT.text = tFuel.ToString ()+" /7";	

				if(tFuel<10)
					fuelPriceText .text = fuelPrice [tFuel].ToString () + " $";
				else
					fuelPriceText .text = "Completed";
			}
			else
				Shop.SetActive(true);
		}
	}


	public void ToggleUpgradeMenu(GameObject target)
	{

		if (PlayerPrefs.GetInt ("Truck" + PlayerPrefs.GetInt ("TruckID").ToString ()) == 3) 
			target.SetActive (!target.activeSelf);
		
	}
}

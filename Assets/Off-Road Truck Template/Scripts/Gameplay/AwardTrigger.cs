using UnityEngine;
using System.Collections;

public enum AwardItem
{

	CoinAward,FualAward
}
public class AwardTrigger : MonoBehaviour {



	public AwardItem itemType;

	ItemManager manager;

	public int awardScore = 100;

	public bool playAudio;

	public AudioSource awardAudioSource;

	void Start () {
		manager = GameObject.FindObjectOfType<ItemManager> ();
	}
	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag ("Player")) 
		{
			if (itemType == AwardItem.CoinAward)
				manager.AddScore (awardScore);
			if (playAudio)
				awardAudioSource.Play ();
			Destroy (gameObject);
		}
		if (col.CompareTag ("Player")) 
		{
			if (itemType == AwardItem.FualAward)
				manager.TotalFuel = 100;
			if (playAudio)
				awardAudioSource.Play ();
			Destroy (gameObject);
		}
	}
}

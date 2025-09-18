using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private List<GameObject> winningConfetti;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && winningConfetti != null)
		{
			foreach (GameObject confettiObject in winningConfetti)
			{
				confettiObject.SetActive(true);
			}
		}
	}
}

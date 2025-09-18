using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class TruckOnCollider : MonoBehaviour
{
	public event Action OnTruckLose;
	public event Action OnTruckWin;
	
	private GameObject _lastCheckpoint;

	private void Start()
	{
		_TruckManager.Instance.GetTruckCheckFlip().OnTruckFlipped += TruckGotHit;
	}
	
	public void OnTriggerEnter(Collider other)
	{
		//Road Manager
		if (other.CompareTag("RoadPoint"))
		{
			other.enabled = false;
			RoadManager.Instance.OnPointPast(other.transform);
		}
		else if (other.CompareTag("Item"))
        {
            Events.OnCollectObject?.Invoke("Item", 1);
            SoundManager.Instance.PlayEmphasis(.2f, .2f);
			var obj = other.gameObject;
			other.isTrigger = false;
			var rig = other.AddComponent<Rigidbody>();
			rig.drag = 9999;
			UIController.Instance.CoinFly();
			obj.tag = "Untagged";
			DOVirtual.DelayedCall(1.5f, () => Destroy(obj));
		}
		else if (other.CompareTag("Coin"))
        {
            Events.OnCollectObject?.Invoke("Coin", 1);
            SoundManager.Instance.PlayEmphasis(.2f, .2f);
			SoundManager.Instance.PlayShot(SoundManager.Instance.coin);
			var tCoin = other.transform;
            other.enabled = false;
			tCoin.DORotate(Vector3.up * 360 * 3, .8f).SetRelative(true);
			tCoin.DOLocalMoveY(tCoin.localPosition.y + 5f, .8f).SetEase(Ease.OutQuad).OnComplete(() =>
			{
				UIController.Instance.CoinFly();
				var par = ObjectPool.Instance.Get(ObjectPool.Instance.parCoin);
				par.transform.position = tCoin.position;
				DOVirtual.DelayedCall(.5f, () => ObjectPool.Instance.Return(par));
				Destroy(tCoin.gameObject);
			});
		}
		//Dead Zone
		else if (other.CompareTag("Deadzone"))
		{
			TruckGotHit();
		}
		else if (other.CompareTag("TutorialNitro"))
		{
			UIController.Instance.uIGamePlay.NitroTutorial();
		}
		else if (other.CompareTag("Mission"))
		{
			other.gameObject.SetActive(false);
			UIController.Instance.uIGamePlay.Mission();
		}
		else if (other.CompareTag("MissionZone") && !MissionManager.Instance.GetMissionTrigger())
		{
			MissionManager.Instance.MissionTrigger(true);
		}
		//Trailer Zone
		else if (other.CompareTag("DeliveryZone"))
		{
			if (_TruckManager.Instance.GetTransportType() == 6)
			{
				_TruckManager.Instance.GetTrailerMission().TrailerDetach();
			}
		}
		//Win
		else if (other.CompareTag("FinishLine"))
		{
			if (RaceManager.Instance.RaceRunning() && RoadManager.Instance.CheckGotoCorrect())
			{
				Win();
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		//If it is item
		if (other.CompareTag("MissionZone") && MissionManager.Instance.GetMissionTrigger()) 
		{
			MissionManager.Instance.MissionTrigger(false);
		}
	}

	private void TruckGotHit()
	{
		OnTruckLose?.Invoke();
	}

	void Win()
	{
		RoadManager.Instance.SetFinish();
		UIController.Instance.uIGamePlay.CompletedRace();
		OnTruckWin?.Invoke();
	}
	
}
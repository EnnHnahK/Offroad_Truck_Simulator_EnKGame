using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FireCamMission : MonoBehaviour
{
	private List<ParticleSystem> _fires = new ();
	[SerializeField] private ParticleSystem smokeParticle;
	
	[Header("In Truck")]
	[SerializeField] private ParticleSystem waterLeakParticle;
	[SerializeField] private Transform camFire, camRotate;
	[SerializeField] private GameObject headLight,waterGun;
	
	//flag
	private bool _isFighting;
	private int _countFire;
	
	//Cache
	private Camera _camera;
	
	private void Awake()
	{
		_camera = Camera.main;
	}
	
	private void Update()
	{
		if (MissionManager.Instance != null && MissionManager.Instance.GetFireFightWorkingStatus())
		{
			RotateCamFire();
		}
	}
	
	void RotateCamFire()
	{
		float rotationSpeed = UIController.Instance.uIGamePlay.GetAngleClamped() * 100f;

		Vector3 currentRotation = camRotate.transform.eulerAngles;

		float newRotationY = currentRotation.y + rotationSpeed * Time.deltaTime;

		camRotate.transform.rotation = Quaternion.Euler(currentRotation.x, newRotationY, currentRotation.z);
	}
	
	public void RotateCamFire(Transform pos, bool isFighting)
	{
		if (isFighting)
		{
			MissionManager.Instance.FireFighting();
			
			camFire.gameObject.SetActive(true);
            
			Quaternion targetRotation = Quaternion.LookRotation(pos.position - camRotate.transform.position);
    
			camRotate.DORotate(targetRotation.eulerAngles, 1f).SetDelay(1.5f).OnComplete(() =>
			{
				waterLeakParticle.Play();
				SoundManager.Instance.PlayLoop(SoundManager.Instance.water, .15f);
	            
				CheckWaterGun();
			});
		}
		else
		{
			waterLeakParticle.Stop();
			SoundManager.Instance.StopLoop(SoundManager.Instance.water);
			camRotate.DOLocalRotate(Vector3.zero, 1f).OnComplete(() =>
			{
				camFire.gameObject.SetActive(false);
			});
		}
	}

	
	public void CheckWaterGun()
	{
		StartCoroutine(IECheckWaterGun());
	}
	
	private IEnumerator IECheckWaterGun()
	{
		while (_countFire > 0)
		{
			foreach (var fire in _fires)
			{
				Vector3 viewportPos = _camera.WorldToViewportPoint(fire.transform.position);
		
				float distanceFromCenterX = Mathf.Abs(viewportPos.x - 0.5f);
				float distanceFromCenterY = Mathf.Abs(viewportPos.y - 0.5f);

				float tolerance = 0.2f; 
		
				if (distanceFromCenterX < tolerance && distanceFromCenterY < tolerance)
				{
					FireFighting(fire);
					break;
				}

			}
			yield return Yielder.GetWaitForSeconds(.2f);
		}
	}

	private void FireFighting(ParticleSystem fire)
	{
		if(_isFighting) return;
		_isFighting = true;
		
		fire.transform.DOScale(0, 4f).OnComplete(() =>
		{
			fire.Stop();
			smokeParticle.transform.position = fire.transform.position;
			smokeParticle.Play();

			_countFire--;

			_fires.Remove(fire);
			_isFighting = false;

			if (_countFire == 0)
			{
				DOVirtual.DelayedCall(1f, () => MissionManager.Instance.FireFightingDone()) ;
			}
		});
	}

	public void SetFireMission(List<ParticleSystem> firePar)
	{
		_fires = new List<ParticleSystem>(firePar);
		_countFire = _fires.Count;
	}
	
	public void ToggleFireFighting(bool isActive)
	{
		if(waterGun == null) return;
		
		if (isActive)
		{
			SoundManager.Instance.PlayLoop(SoundManager.Instance.truckFire, .15f);
		}
		else
		{
			SoundManager.Instance.StopLoop(SoundManager.Instance.truckFire);
			SoundManager.Instance.StopLoop(SoundManager.Instance.water);
		}
		waterGun.SetActive(isActive);
		headLight.SetActive(isActive);
	}

	public void SetParticleBoxCollider(BoxCollider[] boxColliders)
	{
		foreach (var box in boxColliders)
		{
			waterLeakParticle.trigger.AddCollider(box);
		}
	}
}

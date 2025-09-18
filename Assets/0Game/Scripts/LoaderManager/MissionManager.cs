using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class MissionManager : MonoBehaviour
{
	public static MissionManager Instance;
	private MissionStatus _missionStatus;
	public MissionStatus MissionStatus => _missionStatus;
	//Flag
	private bool _missionTrigger, _missionAnimated, _ignoreTrigger;
	
	//Cache
	private Vector3 _deliPos, _triggerSize, _triggerMaxSize;
	private float _time;
	private int _typeMissionIndex;
	
	private bool _fireFightingWorking, _steeringWheelWorking;
	
	[Header("Mission")]
	private float _missionProcessTime = 6f;
	[SerializeField] private Transform missionEndPos, missionZone;
	[SerializeField] private GameObject pointerArrow, boxBlock;
	[SerializeField] private ParticleSystem missionZoneParticle;
	[SerializeField] private SpriteRenderer iconRenderer;
	[SerializeField] private GameObject[] typeMission;
	[SerializeField] private BoxCollider boxInTrigger;
	[SerializeField] private MeshRenderer rodMesh;

	[Header("Vehicle Transport"), Tooltip("Used by Vehicle Transport missions || Mission Type 2")]
	[SerializeField] private Transform[] vehicleItem;
	
	[Header("Passenger Transport"), Tooltip("Used by Passenger Transport missions || Mission Type 3")]
	[SerializeField] private Animator[] citizenAnimators;
	[SerializeField] private Transform busStopEnd;

	[Header("Crane"), Tooltip("Used for transporting wood, stone, vehicles || Mission Type 0 -> 2")]
	[SerializeField] private Transform craneAxis;
	[SerializeField] private Transform craneRod, craneHook,  craneEndPos, craneRodStart;
	[SerializeField] private GameObject[] craneItem, childItem;	
	[SerializeField] private GameObject crane;
	
	
	[Header("Firefighting"), Tooltip("Used by firefighting missions || Mission Type 5")]
	[SerializeField] private List<ParticleSystem> fireParticle = new();
	[SerializeField] private Transform[] fireEndPos;
	[SerializeField] private BoxCollider[] fireBoxColliders;
	private Vector3[] _scaleFire;

	[Header("Trailer Mission"), Tooltip("Used for transporting Trailer || Mission Type 6")]
	[SerializeField] private GameObject particleTrailer;
	[SerializeField] private GameObject pillarTrailer, trailerPack;
	[SerializeField] private Transform trailerEnd;

	[Header("Damaged Vehicle Mission"), Tooltip("Used for transporting Damaged Vehicle || Mission Type 7")]
	[SerializeField] private Transform vehicleDragEnd;

	[Header("Bulldozer Mission"), Tooltip("Used for Bulldozer Mission || Mission Type 8")]
	[SerializeField] private Transform bulldozerEnd;
	[SerializeField] private MeshRenderer tileRenderer;
	
	[Header("Sup Mission"), Tooltip("Used for transporting Sup || Mission Type 9")]
	[SerializeField] private Transform supEndPos;
	[SerializeField] private Transform[] supTransforms;

	[Header("Item Crane Mission"), Tooltip("Used for transporting item in Crane || Mission Type 10")]
	[SerializeField] private Transform[] craneBoxItems, craneBoxParent;
	[SerializeField] private Transform craneBoxItemsEnd;
	[SerializeField] private GameObject arrowItems;
	[SerializeField] private ParticleSystem particleItems;
	

	private void Awake()
	{
		Instance = this;
		
		//Mission
		if(missionZone != null) _deliPos = missionZone.transform.position;
		else
		{
			enabled = false;
			return;
		}

		_triggerSize = boxInTrigger.size;
		_triggerMaxSize = _triggerSize * 8;

		//FireFighting Mission
		_scaleFire = new Vector3[fireParticle.Count];
		for (int i = 0; i < fireParticle.Count; i++)
		{
			_scaleFire[i] = fireParticle[0].transform.localScale;
		}
	}
	
	private void Update()
	{
		if(UIController.Instance == null ||_TruckManager.Instance == null || !UIController.Instance.uIGamePlay.GetMissionPopShowed()) return;

		if (_missionTrigger && RaceManager.Instance.RaceRunning() || _ignoreTrigger)
		{
			
			UIController.Instance.uINotify.WorkingNotify(true);
			
			if (_time > .5f)
			{
				if (_missionStatus == MissionStatus.Accepted)
				{
					switch (_typeMissionIndex)
					{
						//Type 0 Wood = 0, Stone = 1, Vehicle = 2, People = 3, Liquid = 4, FireFighting = 5, Trailer = 6, Crane = 7
						case 2: ReceivingVehicle(true);
							break;
						case 3: ReceivingPeopleEffect(true);
							break;
						case 4: ReceivingLiquidEffect(true);
							break;
						case 5: RotateCamFire(true);
							break;
						case 6: AttachTrailer(true);
							break;
						case 7: DragVehiclesDamaged(true,true);
							break;
						case 8: ReceivingPile(true);
							break;
						case 9: ReceivingSup(true);
							break;
						case 10: DragItemToCrane(true);
							break;
						default: RotateCrane(true);
							break;
					}
				}
				else if (_missionStatus == MissionStatus.Started)
				{
					switch (_typeMissionIndex)
					{
						case 2: ReceivingVehicle(false);
							break;
						case 3: ReceivingPeopleEffect(false);
							break;
						case 4: ReceivingLiquidEffect(false);
							break;
						case 5: RotateCamFire(true);
							break;
						case 6: AttachTrailer(false);
							break;
						case 7: DragVehiclesDamaged(true,false);
							break;
						case 8: ReceivingPile(false);
							break;
						case 9: 
							ReceivingSup(false);
							break;
						case 10: DragItemToCrane(false);
							break;
						default: RotateCrane(false);
							break;
					}
				}
				_ignoreTrigger = false;
			}
			
			_TruckManager.Instance.ToggleMissionProcess(true);

			
			_time += Time.deltaTime;
			
			if (_typeMissionIndex != 5)
			{
				_TruckManager.Instance.FillProcess(_time/_missionProcessTime);
			}

			if (_time > _missionProcessTime)
			{
				_time = 0;
				UIController.Instance.uINotify.WorkingNotify(false);
				switch (_missionStatus)
				{
					case MissionStatus.Accepted: StartMission();
						break;
					case MissionStatus.Started: FinishMission();
						break;
				}
			}
		}
		else
		{
			_TruckManager.Instance.ToggleMissionProcess(false);
			_time = 0;
		}
		
		
	}
	
	#region  Transport Stone + Wood [Type 0 - Wood || Type 1 - Stone]

	[Button]
	private void RotateCrane(bool isReceiving)
	{
		if(_missionAnimated) return;
		_missionAnimated = true;
		
		var itemPos = _TruckManager.Instance.GetItemPos();
		
		Vector3 direction = craneAxis.position - itemPos;
		
		direction.y = 0;	
		
		Quaternion rotation = Quaternion.LookRotation(direction);

		var curRotation = craneAxis.rotation;
		Vector3 rotateCrane = new Vector3(curRotation.eulerAngles.x, rotation.eulerAngles.y + 90, curRotation.eulerAngles.z);

		var eulerAngles = craneHook.eulerAngles;
		Vector3 rotatePoint = new Vector3(eulerAngles.x, _TruckManager.Instance.GetItemPosVehicleEuler().y, eulerAngles.z);

		float curYHook = craneHook.position.y;
		
		Sequence sequence = DOTween.Sequence();

		var timePoint = 0f;
		if (isReceiving) timePoint = 1f;

		sequence.Append(craneAxis.DORotate(rotateCrane, 1f)).AppendCallback(() =>
		{
			craneRod.DOMove(GetPostRod(), 1f);
		}).AppendInterval(1f).Append(craneHook.DOMove(itemPos, 1f)).Append(craneHook.DORotate(rotatePoint, timePoint)).AppendCallback(() =>
		{
			var startFade = 0f; var endFade = 1f;

			if (isReceiving)
			{
				startFade = 1f;
				endFade = 0f;
			}
			else
			{
				craneItem[_typeMissionIndex].SetActive(true);
			}
				
			rodMesh.sharedMaterial.DOFade(endFade, 1.2f).From(startFade).OnComplete(() =>
			{
				LoadItemInCrane(isReceiving);
				rodMesh.sharedMaterial.DOFade(1f, 0f);
			});
			
			DOVirtual.DelayedCall(1.5f, () =>
			{
				craneHook.DOMoveY(curYHook, 1.5f).OnComplete(() =>
				{
					if (!isReceiving)
					{
						DOVirtual.DelayedCall(1f, () =>
						{
							_TruckManager.Instance.GetTruckItemCount().ToggleItem(false);
						});
					}
						
					
					craneAxis.DOLocalRotateQuaternion(curRotation, 1f).OnComplete(() =>
					{
						if (isReceiving)
						{
							DOVirtual.DelayedCall(5f, () =>
							{
								SetPosCrane(craneEndPos);
							});
							
						}
					});
				});
			});
		});
	}

	private void LoadItemInCrane(bool isReceiving)
	{
		if (isReceiving)
		{
			_TruckManager.Instance.GetTruckItemCount().ToggleItem(true);
			childItem[_typeMissionIndex].transform.SetParent(_TruckManager.Instance.GetTruckItemCount().GetMissionItemTransform());

			childItem[_typeMissionIndex].transform.DOLocalMove(Vector3.zero, 1f);
			childItem[_typeMissionIndex].transform.DOLocalRotateQuaternion(Quaternion.identity, 1f);
			childItem[_typeMissionIndex].transform.DOScale(Vector3.one, 1f);
			
			craneItem[_typeMissionIndex].SetActive(false);
		}
		else
		{
			childItem[_typeMissionIndex].SetActive(true);
			craneItem[_typeMissionIndex].SetActive(true);
			
			childItem[_typeMissionIndex].transform.SetParent(craneItem[_typeMissionIndex].transform);
			
			childItem[_typeMissionIndex].transform.DOLocalMove(Vector3.zero, 1f);
			childItem[_typeMissionIndex].transform.DOLocalRotateQuaternion(Quaternion.identity, 1f);
			childItem[_typeMissionIndex].transform.DOScale(Vector3.one, 1f);
			
			_TruckManager.Instance.GetTruckItemCount().TurnOffItemMission();
		}	
	}

	[Button]
	private Vector3 GetPostRod()
	{
		//Vector3 endRod = craneRod.position;
		Vector3 startRod = craneRodStart.position;

		Vector3 dirRod = craneRod.position - startRod;
		Vector3 dirItem = Vector3.up * 10;
		
		Vector3 crossDir = Vector3.Cross(dirRod, dirItem);
		
		if (Mathf.Approximately(crossDir.sqrMagnitude, 0))
		{
			return Vector3.zero;
		}
		
		float delta = Vector3.Dot(Vector3.Cross(_TruckManager.Instance.GetItemPos() - startRod, dirItem), crossDir) / crossDir.sqrMagnitude;
		
		var intersection = startRod + delta * dirRod;

		return intersection;
	}
	
	private void SetPosCrane(Transform point)
	{
		crane.transform.position = point.position;
		crane.transform.localRotation = point.localRotation;

		craneAxis.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
	}

	#endregion
	
	#region Transport Vehicle [Type 2]

	private void ReceivingVehicle(bool isReceiving)
	{
		if(_missionAnimated) return;
		_missionAnimated = true;
		
		var itemPos = _TruckManager.Instance.GetTruckItemCount().GetVehicleItemPos(0);

		Vector3 direction = craneAxis.position - itemPos;
		
		direction.y = 0;	
		
		Quaternion rotation = Quaternion.LookRotation(direction);

		var curRotation = craneAxis.rotation;
		Vector3 rotateCrane = new Vector3(curRotation.eulerAngles.x, rotation.eulerAngles.y + 90, curRotation.eulerAngles.z);
		
		Sequence sequence = DOTween.Sequence();
		
		var timePoint = 0f;
		if (isReceiving)
		{
			timePoint = 1f;
			vehicleItem[0].gameObject.SetActive(false);
		}

		sequence.Append(craneAxis.DORotate(rotateCrane, 1f));
		
		for (int i = 0; i < vehicleItem.Length; i++)
		{
			var temp = i;
			var pos = _TruckManager.Instance.GetTruckItemCount().GetVehicleItemPos(temp);
			
			var eulerAngles = craneHook.eulerAngles;
			Vector3 rotatePoint = new Vector3(eulerAngles.x, _TruckManager.Instance.GetTruckItemCount().GetVehicleItemEuler(temp).y, eulerAngles.z);

			sequence.AppendCallback(() =>
			{
				if(isReceiving) vehicleItem[temp].gameObject.SetActive(false);
				Vector3 newPosition = pos;
				newPosition.y = craneRod.position.y;
				craneRod.DOMove(newPosition, 1f);
			}).Append(craneHook.DOMove(pos, 1f)).Append(craneHook.DORotate(rotatePoint, timePoint)).AppendCallback(() =>
			{
				var startFade = 0f; var endFade = 1f;

				if (isReceiving)
				{
					startFade = 1f;
					endFade = 0f;
				}
				else
				{
					craneItem[_typeMissionIndex].SetActive(true);
					childItem[_typeMissionIndex].SetActive(false);
				}
				
				rodMesh.sharedMaterial.DOFade(endFade, 1.5f).From(startFade).OnComplete(() =>
				{
					rodMesh.sharedMaterial.DOFade(1f, 0f);
					
					if(!isReceiving)  childItem[_typeMissionIndex].SetActive(true);
				});

				DOVirtual.DelayedCall(1.5f, () =>
				{
					if (isReceiving)
					{
						_TruckManager.Instance.GetTruckItemCount().ReceivingVehicle(vehicleItem[temp], temp);
						craneItem[_typeMissionIndex].SetActive(false);
					}
					else
					{
						craneItem[_typeMissionIndex].SetActive(true);
						_TruckManager.Instance.GetTruckItemCount().ReturnVehicle(temp);
					}
				});
			}).AppendInterval(1.5f).Append(craneHook.DOMoveY(pos.y + 5f, 1f)).AppendCallback(() =>
			{
				if (isReceiving)
				{
					if(temp != vehicleItem.Length - 1) craneItem[_typeMissionIndex].SetActive(true);
				}
				else
				{
					craneItem[_typeMissionIndex].SetActive(false);
				}
			});
		}

		sequence.AppendCallback(() =>
		{
			DOVirtual.DelayedCall(1f, () =>
			{
				craneHook.DOMoveY(itemPos.y + 5f, 2f).OnComplete(() =>
				{
					if (isReceiving) SetPosCrane(craneEndPos);
				});
			});
		});
	}

	#endregion
	
	#region Transport People [Type 3]
	
	[Button]
	private void ReceivingPeopleEffect(bool isReceiving)
	{
		if(_missionAnimated) return;
		_missionAnimated = true;
		
		if(citizenAnimators.Length == 0) return;
		
		Sequence sequence = DOTween.Sequence();
		
		if (isReceiving)
		{
			
			for (int i = 0; i < citizenAnimators.Length; i++)
			{
				var temp = i;
				var tempPos = citizenAnimators[temp].transform.position;
				sequence.AppendCallback(() =>
				{
					citizenAnimators[temp].gameObject.SetActive(true);
					citizenAnimators[temp].Play("Walking", 0, 0);
					citizenAnimators[temp].transform.LookAt(_TruckManager.Instance.GetVehiclePos());
				}).AppendCallback(() => citizenAnimators[temp].transform.DOMove(_TruckManager.Instance.GetVehiclePos(), 1.75f).OnComplete(() =>
				{
					citizenAnimators[temp].transform.position = tempPos;
					citizenAnimators[temp].gameObject.SetActive(false);
				}).SetEase(Ease.Linear)).AppendInterval(1.75f);
			}
		}
		else
		{
			for (int i = 0; i < citizenAnimators.Length; i++)
			{
				var temp = i;
				sequence.AppendCallback(() =>
				{
					citizenAnimators[temp].gameObject.SetActive(true);
					citizenAnimators[temp].Play("Walking", 0, 0);
					citizenAnimators[temp].transform.LookAt(typeMission[_typeMissionIndex].transform.position);
				}).Append(citizenAnimators[temp].transform.DOMove(typeMission[_typeMissionIndex].transform.position, 1.75f).From(_TruckManager.Instance.GetVehiclePos()).OnComplete(() => citizenAnimators[temp].gameObject.SetActive(false)).SetEase(Ease.Linear));
			}
		}
		
		sequence.SetLoops(2).Play().OnComplete(() =>
		{
			if (isReceiving)
			{
				_TruckManager.Instance.GetTruckItemCount().ToggleItem(true, false);
				DOVirtual.DelayedCall(5f, () => SetMissionDecorPos(busStopEnd)) ;
			}
			else
			{
				_TruckManager.Instance.GetTruckItemCount().ToggleItem(false);
			}
			
		});

	}

	private void SetMissionDecorPos(Transform point)
	{
		typeMission[_typeMissionIndex].transform.position = point.position;
		typeMission[_typeMissionIndex].transform.localRotation = busStopEnd.localRotation;
	}
	

	#endregion
	
	#region Transport Liquid [Type 4]
	
	private void ReceivingLiquidEffect(bool isReceiving)
	{
		if(_missionAnimated) return;
		_missionAnimated = true;
		
		_TruckManager.Instance.GetTruckItemCount().AlphaContainer(isReceiving);

		DOVirtual.DelayedCall(_missionProcessTime + 5f, () => SetMissionDecorPos(craneEndPos));
	}
	#endregion

	#region Firefighting Mission [Type 5]

	[Button]
	private void RotateCamFire(bool isFighting)
	{
		if(fireParticle == null){
			#if UNITY_EDITOR || DEBUGLOG
				Debug.LogError("FirePos Transform is null");
			#endif
			return;
		}
		
		if(_missionAnimated) return;
		_missionAnimated = true;
		_TruckManager.Instance.GetFireCamMission().RotateCamFire(fireParticle[0].transform, isFighting);
	}

	public void FireFightingDone()
	{
		_fireFightingWorking = false;
		UIController.Instance.SetWorkingInInput(_fireFightingWorking);
		
		_time += _missionProcessTime;
		
		for (int i = 0; i < fireParticle.Count; i++)
		{
			if (fireParticle[i].transform.localPosition != fireEndPos[i].localPosition)
			{
				fireParticle[i].transform.localPosition = fireEndPos[i].localPosition;

				var temp = i;
				DOVirtual.DelayedCall(1f, () =>
				{
					fireParticle[temp].Play();
					fireParticle[temp].transform.localScale = _scaleFire[temp]; 
				});
			}
		}

		_missionAnimated = false;
		RotateCamFire(false);
		
		_TruckManager.Instance.GetFireCamMission().SetFireMission(fireParticle);
	}

	public void FireFighting()
	{
		_fireFightingWorking = true;
		UIController.Instance.SetWorkingInInput(_fireFightingWorking);
	}


	public bool GetFireFightWorkingStatus()
	{
		return _fireFightingWorking;
	}
	

	#endregion
	
	#region Transport Trailer [Type 6]
	
	
	private void AttachTrailer(bool isAttach)
	{
		if(_missionAnimated) return;
		_missionAnimated = true;
		
		if (DataController.StepTutorial == 6)
		{
			var reverseTransform = UIController.Instance.uIGamePlay.GetReverse();
			if(UITutorial.Instance) UITutorial.Instance.SetFingerPos(reverseTransform.localPosition, reverseTransform.parent);
			PrefData.StepTutorial++;
		}
		
		if (isAttach)
		{
			particleTrailer.SetActive(true);
			typeMission[_typeMissionIndex].SetActive(true);
			
			CinemachineManager.Instance.LookAt(particleTrailer.transform, true);
			_TruckManager.Instance.GetTrailerMission().SetTarget(particleTrailer.transform);
			return;
		}
		
		TrailerDetached();
	}

	public void TrailerAttached()
	{
		if(_missionStatus != MissionStatus.Accepted) return;
		
		pillarTrailer.SetActive(false);

		particleTrailer.transform.DOLocalMoveY(particleTrailer.transform.localPosition.y - 10f, 1f).OnComplete(() =>
		{
			particleTrailer.SetActive(false);
			particleTrailer.transform.position = trailerEnd.position;
			particleTrailer.transform.localRotation = trailerEnd.localRotation;
		});
		
		_TruckManager.Instance.GetTrailerMission().ToggleTrailerCam(false, true);
		
		RoadManager.Instance.DrawCurTruckPos();
		
		StartMission();
		
		UIController.Instance.uINotify.WorkingNotify(false);
		
		_TruckManager.Instance.GetTrailerMission().UnTarget();

		_time += _missionProcessTime;
	}

	public void TrailerDetached()
	{
		if(_missionStatus != MissionStatus.Started) return;	
		
		_TruckManager.Instance.GetTrailerMission().TrailerDetach();
		
		_time += _missionProcessTime;
		
		FinishMission();
			
		UIController.Instance.uINotify.WorkingNotify(false);
		
	
	}

    #endregion

	#region Transport Damaged Vehicles [Type 7]

	private void DragVehiclesDamaged(bool isDrag, bool isReceiving)
	{
		if(_missionAnimated) return;
		_missionAnimated = true;
		
		_steeringWheelWorking = isDrag;
		
		UIController.Instance.SetWorkingInInput(_steeringWheelWorking);

		if (isReceiving)
		{
			_TruckManager.Instance.GetCraneMission().ToggleMission(isDrag, true, typeMission[_typeMissionIndex].transform);
		}
		else
		{
			vehicleDragEnd.gameObject.SetActive(isDrag);
			_TruckManager.Instance.GetCraneMission().ToggleMission(isDrag, false, vehicleDragEnd);
		}
	}
	
	public void DragVehicleDone()
	{
		_steeringWheelWorking = false;
		
		_time += _missionProcessTime;
		_missionAnimated = false;
		
		DragVehiclesDamaged(false, true);
	}

	public bool GetWheelWorking()
	{
		return _steeringWheelWorking;
	}
	
	#endregion

	#region Bulldozer Mission [Type 8]

	private void ReceivingPile(bool isReceiving)
	{
		if(_missionAnimated) return;
		_missionAnimated = true;

		if (isReceiving)
		{
			typeMission[_typeMissionIndex].transform.DOScaleY(0, 3f).OnComplete(() =>
			{
				typeMission[_typeMissionIndex].transform.position = bulldozerEnd.position;
				typeMission[_typeMissionIndex].transform.rotation = bulldozerEnd.rotation;
			});
			_TruckManager.Instance.GetBulldozerMission().ScoopingPile(true);
		}
		else
		{
			_TruckManager.Instance.GetBulldozerMission().ScoopingPile(false);
			typeMission[_typeMissionIndex].transform.DOScaleY(1, 3f);
		}
	}
	
	#endregion

	#region Sup Mission [Type 9]

	private void ReceivingSup(bool isReceiving)
	{
		if(_missionAnimated) return;
		_missionAnimated = true;
		
		Sequence sequence = DOTween.Sequence();

		if (isReceiving)
		{
			_TruckManager.Instance.GetTruckItemCount().ToggleItem(true,false);
			for (int i = 0; i < supTransforms.Length; i++)
			{
				var temp = i;
				sequence.AppendCallback(() =>
				{
					
					supTransforms[temp].SetParent(_TruckManager.Instance.GetTruckItemCount().GetVehicleItemTransform(temp));
					supTransforms[temp].DOLocalJump(Vector3.zero, 3f, 1, 1.5f).SetEase(Ease.OutQuad);
					supTransforms[temp].DOLocalRotateQuaternion(Quaternion.identity, 1f);
				}).AppendInterval(1f);
			}

			sequence.AppendInterval(5f).AppendCallback(() =>
			{
				typeMission[_typeMissionIndex].transform.localRotation = supEndPos.localRotation;
				typeMission[_typeMissionIndex].transform.localPosition = supEndPos.localPosition;
			});
			
			return;
		}
		
		for (int i = 0; i < supTransforms.Length; i++)
		{
			var temp = i;
			sequence.AppendCallback(() =>
			{
				supTransforms[temp].SetParent(typeMission[_typeMissionIndex].transform);
				_TruckManager.Instance.GetTruckItemCount().ToggleItemTransform(temp);
				supTransforms[temp].DOLocalJump(new Vector3(0, temp * 0.5f,0) , 3f, 1, 1.5f);
				supTransforms[temp].DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(0,typeMission[_typeMissionIndex].transform.localRotation.y, 0 )), 1.5f);
			}).AppendInterval(1f);
		}

		sequence.AppendCallback(() => _TruckManager.Instance.GetTruckItemCount().ToggleItem(false, false));
	}

	

	 #endregion

	#region Transport Item Crane [Type 10]

	private void DragItemToCrane(bool isReceiving)
	{
		if(_missionAnimated) return;
		_missionAnimated = true;
		
		_TruckManager.Instance.GetCraneItemMission().ToggleMission(typeMission[_typeMissionIndex].transform, isReceiving);
	}


	public void DragItemDone(bool isReceiving)
	{
		_time += _missionProcessTime;
		DOVirtual.DelayedCall(5f, () =>
		{
			if (isReceiving)
			{
				typeMission[_typeMissionIndex].transform.localPosition = craneBoxItemsEnd.localPosition;
				typeMission[_typeMissionIndex].transform.localRotation = craneBoxItemsEnd.localRotation;
			}
			else
			{
				arrowItems.SetActive(false);
				particleItems.Stop();
			}
			
			_missionAnimated = false;
		});
	}
	public Transform GetItemCraneTransform(int index)
	{
		return craneBoxItems[index];
	}

	public Transform GetItemCraneParent(int index)
	{
		return craneBoxParent[index];
	}
	

    #endregion
	
	#region  Mission Event

	
	[Button]
	public void AcceptMission()
	{
		_missionAnimated = false;
		_ignoreTrigger = false;
		
		missionZoneParticle.Play();
		pointerArrow.SetActive(true);
		iconRenderer.gameObject.SetActive(true);
		
		_typeMissionIndex = _TruckManager.Instance.GetTransportType();
		
		//Enable Block Collider
		boxBlock.SetActive(true);

		//Check Crane need active
		crane.SetActive(_typeMissionIndex <= 2);

		//Check Mission Item in Map // Init missionTime Process
		for (int i = 0; i < typeMission.Length; i++)
		{
			if (_typeMissionIndex == i)
			{
				switch (i)
				{
					case 3:
					case 2: _missionProcessTime = 14f;
						break;
					case 5:
						_TruckManager.Instance.GetFireCamMission().ToggleFireFighting(true);
						_TruckManager.Instance.GetFireCamMission().SetFireMission(fireParticle);
						_missionProcessTime = 600f;
						break;
					case 6:
						var trailer = Instantiate(trailerPack);
						trailer.transform.SetParent( typeMission[_typeMissionIndex].transform);
						trailer.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						_missionProcessTime = 600f;
						continue;
					case 7: 
						_missionProcessTime = 600f;
						break;
					case 8: _TruckManager.Instance.GetBulldozerMission().SetMatWithEnvironment(tileRenderer.material);
						break;
					case 10: _missionProcessTime = 600f;
						break;
				}
				
				
				typeMission[i].SetActive(true);
				continue;
			}
			typeMission[i].SetActive(false);
		}

		
		//Check Crane Item 
		for (int i = 0; i < craneItem.Length; i++)
		{
			if (_typeMissionIndex == i)
			{
				craneItem[i].SetActive(true);
				continue;
			}
			craneItem[i].SetActive(false);
		}
		
		//FireFighting
		if (fireBoxColliders.Length != 0 && _typeMissionIndex == 5)
		{
			_TruckManager.Instance.GetFireCamMission().SetParticleBoxCollider(fireBoxColliders);
		}

		_missionStatus = MissionStatus.Accepted;
	}
	
	[Button]
	private void StartMission()
	{
		_missionTrigger = false;
		_missionStatus = MissionStatus.Started;
		SoundManager.Instance.UpgradeSound(1);
		HideMissionZone(true, -10f);

		//Reset flag animated
		_missionAnimated = false;
		
		//Disable Block Collider
		boxBlock.SetActive(false);

		boxInTrigger.size = _triggerSize;
		if(_typeMissionIndex == 6) boxInTrigger.gameObject.SetActive(true);
	}

	[Button]
	private void FinishMission()
	{
		_missionTrigger = false;
		_missionStatus = MissionStatus.Finished;
		UIController.Instance.missionPopup.NotiMission();
		SoundManager.Instance.UpgradeSound(2);
		HideMissionZone(false, -10f);
		UIController.Instance.uIGameCompleted.SetCoinMission(1000);
		Events.OnCompleteIncomeMission?.Invoke((TransportType)_typeMissionIndex);

		if(_typeMissionIndex == 5) _TruckManager.Instance.GetFireCamMission().ToggleFireFighting(false); 
		//if(_typeMissionIndex == 7) vehicleDragEnd.gameObject.SetActive(false);
		
		boxInTrigger.size = _triggerSize;
		if(_typeMissionIndex == 6) boxInTrigger.gameObject.SetActive(true);
	}
	
	public void MissionTrigger(bool trigger)
	{
		_missionTrigger = trigger;
		
		pointerArrow.SetActive(_typeMissionIndex != 6 && !trigger);

		if (_typeMissionIndex == 6)
		{
			_ignoreTrigger = true;
			missionZoneParticle.Stop();
			boxInTrigger.gameObject.SetActive(false);
		}

		if (_typeMissionIndex == 10 && _missionStatus == MissionStatus.Started)
		{
			missionZoneParticle.Stop();
			
			arrowItems.SetActive(true);
			particleItems.Play();
		}
		

		if(trigger) boxInTrigger.size = _triggerMaxSize;
	}

	#endregion

	#region Mission Processing
	
	private void HideMissionZone(bool start, float pos)
	{
		if(missionZone == null) return;

		iconRenderer.DOFade(.1f, 1f).From(1).SetUpdate(true);
		missionZoneParticle.transform.DOLocalMoveY(pos, 1f).SetUpdate(true).OnComplete(() =>
		{
			missionZoneParticle.transform.DOKill();

			if (start)
			{
				var missionZoneTransform = missionZone.transform;
				missionZoneTransform.position = missionEndPos.position;
				missionZoneTransform.localRotation = missionEndPos.localRotation;
				
				missionZoneParticle.Play();
				
				missionZoneParticle.transform.localPosition =  Vector3.zero;

				Color currentColor = iconRenderer.color;
				currentColor.a = 1f;
				iconRenderer.color = currentColor;
				
				pointerArrow.SetActive(true);
				iconRenderer.gameObject.SetActive(true);
			}
			else
			{
				iconRenderer.gameObject.SetActive(false);
				pointerArrow.SetActive(false);
				missionZoneParticle.Stop();
				missionZoneParticle.gameObject.SetActive(false);
			}
		});
	}

	public void MissionCanceled()
	{
		missionZoneParticle.gameObject.SetActive(false);
		foreach (var typeObject in typeMission)
		{
			typeObject.SetActive(false);
		}
	}

	public void ShowDelivery()
	{
		if(missionZone == null) return;
		missionZone.gameObject.SetActive(true);
		missionZone.transform.position = _deliPos;
	}
	
    #endregion

	#region Data

	public bool GetMissionTrigger()
	{
		return _missionTrigger;
	}

	public bool CheckMissionStatus(MissionStatus status)
	{
		return status == _missionStatus;
	}

	#endregion

	private void OnDisable()
	{
		craneAxis.DOKill();
		craneHook.DOKill();
		craneRod.DOKill();
		missionZoneParticle.transform.DOKill();
	}
}

public enum MissionStatus
{
	None = 0,
	Accepted = 1,
	Started = 2,
	Finished = 3,
}

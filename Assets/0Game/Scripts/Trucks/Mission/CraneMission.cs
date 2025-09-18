using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CraneMission : MonoBehaviour
{
	[Header("Crane Controller")]
	[SerializeField] private Transform craneAxis;
	[SerializeField] private Transform craneMainHorizontal, craneSubHorizontal, craneExtendHorizontal, craneHook , craneSubBar, itemParent;
	
	[SerializeField] private GameObject cam;

	//Flag
	private bool _doneWorking, _isWorking, _isReceiving;
	
	//Cache
	private Transform _targetPos, _vehicleAttached;
	private Camera _camera;
	private Quaternion _curAxisRos, _curMainHozLocalRos,  _curSubBarLocalRos, _curAttachLocalRos;
	private Vector3 _curHookLocalPos, _curExtendBarLocalPos;


	private void Awake()
	{
		_camera = Camera.main;


		_curAxisRos = craneAxis.localRotation;
		_curMainHozLocalRos = craneMainHorizontal.localRotation;
		_curSubBarLocalRos = craneSubBar.localRotation;
		_curHookLocalPos = craneHook.localPosition;
		_curExtendBarLocalPos = craneExtendHorizontal.localPosition;
	}
	
	public void ToggleMission(bool isDrag, bool receiving, Transform pos)
	{
		cam.SetActive(isDrag);
		_doneWorking = false;
		_targetPos = pos;
		_isReceiving = receiving;
		
		if (isDrag)
		{
			StartCoroutine(IECheckCrane());
		}
	}

	private void CraneRotate(Transform targetPos)
	{
		Vector3 direction = craneAxis.position - targetPos.position;
		
		direction.y = 0;	
		
		Quaternion rotation = Quaternion.LookRotation(direction);

		var curRotation = craneAxis.rotation;
		Vector3 rotateCrane = new Vector3(curRotation.eulerAngles.x, rotation.eulerAngles.y - 45f, curRotation.eulerAngles.z);

		craneAxis.DORotate(rotateCrane, 1f);
	}

	[Button]
	private void CraneHorizontalUp()
	{
		Sequence sequence = DOTween.Sequence();
		
		Quaternion targetRotationMain = craneMainHorizontal.rotation * Quaternion.Euler(-30, 0, 0);
		Quaternion targetRotationSub = craneSubBar.rotation * Quaternion.Euler(-30, 0, 0);

		sequence.AppendCallback(() =>
		{
			//Rotate Main Horiz + SubBar 30
			craneMainHorizontal.DORotateQuaternion(targetRotationMain, 1f);
			craneSubBar.DORotateQuaternion(targetRotationSub, 1f);
		}).AppendCallback(() =>
			{
				if (!_isReceiving)
				{
					//Quaternion targetRotation = Quaternion.Euler(_vehicleAttached.localRotation.eulerAngles.x, _vehicleAttached.localRotation.eulerAngles.y, _curAttachLocalRos.z);
					_vehicleAttached.DOLocalRotateQuaternion(_curAttachLocalRos, .5f);
				}
			})
			.AppendInterval(1.1f).Append(craneSubHorizontal.DOLocalMoveY(20, 1f))
			.AppendCallback(() => craneExtendHorizontal.DOMove(GetPostRod(), 1f)).AppendInterval(1.1f) //Move Extend Horiz 
			.Append(craneHook.DOMoveY(_targetPos.position.y + 1.5f, 1f).OnComplete(() =>
				{
					if (_isReceiving)
					{
						_targetPos.SetParent(itemParent);
						_curAttachLocalRos = _targetPos.localRotation;
						_vehicleAttached = _targetPos;
						Vector3 localPos = new Vector3(0, _targetPos.localPosition.y, 0);
						_targetPos.DOLocalMove(Vector3.zero, 2f);
					}
					else
					{
						_vehicleAttached.SetParent(null);
						_vehicleAttached.DOLocalRotateQuaternion( Quaternion.Euler(0, _vehicleAttached.localRotation.eulerAngles.y, 0), .5f);
						_vehicleAttached.DOMove(_targetPos.position, .5f);
					}
					
				})
			).AppendInterval(0.5f)
			.Append(craneHook.DOLocalMove(_curHookLocalPos, 1f))
			.Append(craneSubHorizontal.DOLocalMoveY(10, 1f))
			.Append(craneExtendHorizontal.DOLocalMove(_curExtendBarLocalPos, 1f))
			.AppendCallback(() =>
			{
				craneMainHorizontal.DOLocalRotateQuaternion(_curMainHozLocalRos, 1f);
				craneSubBar.DOLocalRotateQuaternion(_curSubBarLocalRos, 1f);
			}).AppendInterval(1.1f)
			.AppendCallback(() =>
			{
				if (_isReceiving)
				{
					//Rotate Vehicle Attached
					Quaternion targetRotation = Quaternion.Euler(0, -90, 180);
					_targetPos.DOLocalRotateQuaternion(targetRotation, 1f);
				}
				

			})
			.Append(craneAxis.DOLocalRotateQuaternion(_curAxisRos, 1f))
			.AppendCallback(() =>
			{
				MissionManager.Instance.DragVehicleDone();
				_isWorking = false;
			});
	}

	private void Update()
	{
		if (MissionManager.Instance != null && MissionManager.Instance.GetWheelWorking())
		{
			RotateCrane();
		}
	}


	private void RotateCrane()
	{
		if(_isWorking) return;
		
		float rotationSpeed = UIController.Instance.uIGamePlay.GetAngleClamped() * 100f;

		Vector3 currentRotation = craneAxis.transform.eulerAngles;

		float newRotationY = currentRotation.y + rotationSpeed * Time.deltaTime;

		craneAxis.transform.rotation = Quaternion.Euler(currentRotation.x, newRotationY, currentRotation.z);
	}

	private IEnumerator IECheckCrane()
	{
		while (!_doneWorking && _targetPos != null)
		{
			Vector3 viewportPos = _camera.WorldToViewportPoint(_targetPos.position);
			
			float distanceFromCenterX = Mathf.Abs(viewportPos.x - 0.5f);
			float distanceFromCenterY = Mathf.Abs(viewportPos.y - 0.5f);

			float tolerance = 0.25f; 
	
			if (distanceFromCenterX < tolerance && distanceFromCenterY < tolerance)
			{
				AttachVehicle();
				break;
			}
			
			yield return Yielder.GetWaitForSeconds(.2f);
		}
	}

	private void AttachVehicle()
	{
		if(_isWorking) return;
		_isWorking = true;
		_doneWorking = true;
		
		CraneHorizontalUp();
	}

	public void ReturnVehicle(Transform posEnd)
	{
		CraneRotate(posEnd);
		
		Sequence sequence = DOTween.Sequence();

		var curHookPos = craneHook.position;
		
		sequence.AppendInterval(1f).Append(craneHook.DOMove(posEnd.position, 1f)).AppendInterval(.5f).Append(craneHook.DOMove(curHookPos, 1f)).AppendCallback(() =>
		{
			craneMainHorizontal.DORotateQuaternion(_curAxisRos, 1f);
			craneSubBar.DORotateQuaternion(_curSubBarLocalRos, 1f);
		}).AppendInterval(1f);
	}
	
	private Vector3 GetPostRod()
	{
		//Vector3 endRod = craneRod.position;
		Vector3 startRod = craneMainHorizontal.position;

		Vector3 dirRod = craneExtendHorizontal.position - startRod;
		Vector3 dirItem = Vector3.up * 10;
		
		Vector3 crossDir = Vector3.Cross(dirRod, dirItem);
		
		if (Mathf.Approximately(crossDir.sqrMagnitude, 0))
		{
			return Vector3.zero;
		}
		
		float delta = Vector3.Dot(Vector3.Cross(_targetPos.position - startRod, dirItem), crossDir) / crossDir.sqrMagnitude;
		
		var intersection = startRod + delta * dirRod;
		
	#if UNITY_EDITOR

		GameObject line = new GameObject("LineCheck");

		var lineRenderer = line.AddComponent<LineRenderer>();

		lineRenderer.widthMultiplier = .1f;

		lineRenderer.loop = true;

		lineRenderer.positionCount = 4;
			
		lineRenderer.SetPositions(new [] { startRod, craneExtendHorizontal.position, _targetPos.position, intersection });

	#endif

		return intersection;
	}


}

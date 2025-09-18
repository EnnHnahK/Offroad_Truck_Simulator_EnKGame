using NWH.VehiclePhysics2;
using NWH.WheelController3D;
using UnityEngine;

public class _TruckManager : MonoBehaviour
{
	public static _TruckManager Instance { get; private set; }
	public static bool Swamp = false;

	[Header("Truck Components")]
	[SerializeField] private Rigidbody _rigidbody;
	[SerializeField] private VehicleFunction vehicleFunction;
	[SerializeField] private CameraSwitch _cameraSwitch;
	[SerializeField] private TruckOnCollider _truckOnCollider;
	[SerializeField] private TruckItemCount _truckItemCount;
	[SerializeField] private TruckCheckFlip _truckCheckFlip;
	[SerializeField] private VehicleController _vehicleControl;

	[Header("Truck Data")]
	[SerializeField] private TransportType transportType;
	[SerializeField] private WheelBaseType wheelBaseType = WheelBaseType.Medium;
	[SerializeField] private WheelController[] _wheelControllers;
	[SerializeField] private ParticleSystem parStart;
	[SerializeField] private ParticleSystem nitroPar;
	public bool applyCustomEngine;
	public float maxPower = 400;
	
	[Header("Mission")]
	[SerializeField] private GameObject missionProcess;
	[SerializeField] private SpriteRenderer missionFill;
	[SerializeField] private Transform itemPos, itemMission, cacheTransform;
	private static readonly int Arc2 = Shader.PropertyToID("_Arc2");
	
	[Header("Fire Fighting Mission")]
	[SerializeField] private FireCamMission fireCamMission;

	[Header("Transport Damaged Vehicles")]
	[SerializeField] private CraneMission craneMission;
	
	[Header("Bulldozer Mission")]
	[SerializeField] private BulldozerMission bulldozerMission;
	
	[Header("Trailer Mission")]
	[SerializeField] private TrailerMission trailerMission;

	[Header("Item Crane Mission")]
	[SerializeField] private CraneItemMission craneItemMission;

	private void Start()
	{
		GetTruckOnCollider().OnTruckWin += () =>
        {
        	if (RaceManager.Instance.RaceRunning())
        	{
	            SoundManager.Instance.PlayEmphasis(.5f, .7f);
        	}
        };
		_rigidbody.drag = .05f;

		
#if UNITY_EDITOR
		if (applyCustomEngine)
		{
			HotApplyEngine();
		}
#endif

		if (DataController.CurLevel is >= 3 and <= 5 && (int)transportType is 0 or 1)
		{
			itemPos.localRotation = Quaternion.identity;
			itemMission.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
			
		}
	}
	
	public void SetTruckManagerInstance()
	{
		Instance = this;
	}
	
	#region Truck Components
	
	public Rigidbody GetRigidbody()
	{
		return _rigidbody;
	}

	public VehicleFunction GetVehicleFunction()
	{
		return vehicleFunction;
	}
	
	public VehicleController GetVehicleControl()
	{
		return _vehicleControl;
	}

	public CameraSwitch GetCameraSwitch()
	{
		return _cameraSwitch;
	}

	public TruckOnCollider GetTruckOnCollider()
	{
		return _truckOnCollider;
	}

	public TruckItemCount GetTruckItemCount()
	{
		return _truckItemCount;
	}

	public WheelController[] GetWheels()
	{
		return _wheelControllers;

	}
	#endregion

	#region Vehicle Transform
	
	public Vector3 GetVehiclePos()
	{
		return _rigidbody.transform.position;
	}

	public Quaternion GetVehicleLocalRotation()
	{
		return _rigidbody.transform.localRotation;
	}
	public Vector3 GetItemPosVehicleEuler()
	{
		return itemPos.transform.eulerAngles;
	}
	public Vector3 GetItemPos()
	{
		return itemPos.transform.position;
	}
	
	public TruckCheckFlip GetTruckCheckFlip()
	{
		return _truckCheckFlip;
	}

	
	#endregion
	
	#region Truck Hot Apply Attribute
	
	#if UNITY_EDITOR
	
	[Button]
	private void HotApplyEngine()
	{
		_vehicleControl.powertrain.engine.maxPower = maxPower;
	}
	
	#endif

	
	private float AddBoost()
	{
		return 1.5f;
	}

	private float ResetBoost()
	{
		return 1f; 
	}

	public void ApplyNitro(bool active)
	{
		if (active)
		{
			_vehicleControl.powertrain.engine.powerModifiers.Add(AddBoost);
		}
		else
		{
			_vehicleControl.powertrain.engine.powerModifiers.Add(ResetBoost);
		}
		
	}
	
	public void SetNitroPar(bool isActive)
	{
		if (isActive)
		{
			if(!nitroPar.isPlaying) nitroPar.Play();
		}
		else
		{
			if(nitroPar.isPlaying) nitroPar.Stop();
		}
	}
	
	public void ApplyGripUpgrade(float grip)
	{
		if (LevelController.Instance.weather == TypeWeather.Snow)
		{
			grip *= 0.85f;
		}
		
		foreach (var wheel in _wheelControllers)
		{
			wheel.LateralFrictionGrip = 0.15f + grip;
			wheel.LongitudinalFrictionGrip = grip;
		}
	}
	
	
	public void Slow(float drag = .5f)
	{
		if (MissionManager.Instance != null)
		{
			if (MissionManager.Instance.GetMissionTrigger() && (int)transportType != 6)
			{
				_rigidbody.drag = 20;
				return;
			}
		}
		_rigidbody.drag = drag;
	}
	
	public void Stop()
	{
		_vehicleControl.input.Throttle = 0;
	}

	#endregion
	
	#region Mission

	public void ToggleMissionProcess(bool isActive)
	{
		missionProcess.SetActive(isActive);
	}
	
	public void FillProcess(float fill)
	{
		missionFill.material.SetFloat(Arc2, 360 - fill * 360);
	}
	
	public FireCamMission GetFireCamMission()
	{
		return fireCamMission;
	}
	
	public CraneMission GetCraneMission()
	{
		return craneMission;
	}

	public BulldozerMission GetBulldozerMission()
	{
		return bulldozerMission;
	}

	public TrailerMission GetTrailerMission()
	{
		return trailerMission;
	}

	public CraneItemMission GetCraneItemMission()
	{
		return craneItemMission;
	}
	

	#endregion
	
	#region  Cache Items

	
	public Transform GetTransformCache()
	{
		return cacheTransform;
	}
	
	public void SetParentCache(Transform item)
	{
		item.SetParent(cacheTransform);
	}
	
	public void ToggleCache(bool isActive)
	{
		cacheTransform.gameObject.SetActive(isActive);
	}
	
	#endregion
	
	#region Data

	public int GetValueWheelBase()
	{
		return (int)wheelBaseType;
	}
	
	public int GetTransportType()
	{
		return (int)transportType;
	}
	
	#endregion
}

public enum TransportType{
	Wood = 0,
	Stone = 1,
	Vehicle = 2,
	People = 3,
	Liquid = 4,
	FireFighting = 5,
	Trailer = 6,
	Crane = 7,
	Bulldozer = 8,
	Sup = 9,
	ItemCrane = 10,
}

public enum WheelBaseType
{
	Short = 300,
	Medium = 200,
	Long = 100,
}


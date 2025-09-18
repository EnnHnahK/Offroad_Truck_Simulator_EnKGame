//--------------------------------------------------------------
//
//                    Off-Road Truck Kit
//          Writed by AliyerEdon in fall 2016
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

// This script used for Truck vehicle controller and physics behaviour

using System;
using System.Collections;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class TruckController : MonoBehaviour
{	

	[Space(1)]    
	public bool canControl;
	[Header("WHeel Settings")]

	[Space(3)]
	// Wheel colliders (we will use the last 2 wheel to apply motor torque)
	public WheelCollider[] Wheel_Colliders = new WheelCollider[4];

	// Extra 2 back wheels 
	public WheelCollider extra_Rear_Left_WC, extra_Rear_Right_WC;

	// WHeel transforms (Visuals)
	public GameObject[] Wheel_Transforms = new GameObject[4];

	// Extra wheels transforms
	public GameObject extra_Rear_Left_T, extra_Rear_Right_T;



	[Space(3)]
	[Header("Gears Setup")]
	[Space(3)]
	// Maximus speed can reach
	public float MaxSpeed = 140f;
	// Total number of gears for truck sound system
	public  int numberOfGears = 20;
	// How much time need to delay before switch to next or previous gears
	public float GearShiftDelay = 0.3f;

	// Current gear
	public int currentGear;
	// gear factor is a normalised representation of the current speed within the current gear's range of speeds.
	private float GearFactor;
	// Truck Rigidbody
	private Rigidbody rigid;

	// Engine revs (for display / sound)
	[HideInInspector]
	public float Revs;

	[Space(3)]
	[Header("Lights")]
	[Space(3)]
	// Truck back lights
	public Light[] backLights;

	//Front light for night level
	public GameObject frontLight;


	// Trcuk BackLight material (self-Illuminated)
	public Material backLightMaterial;

	// DashBoard view steeringwheel tranform
	public Transform SteeringWheelTransform;

	// Current speed
	public float CurrentSpeed;

	// Reversing alarm handler
	[HideInInspector]
	public AudioSource reverseAlram;

	// Catch audio component handler
	TruckAudio audioTruck;

	// Vehicle Setup
	[Space(3)]
	[Header("Vehicle Settings")]
	[Space(3)]
	public float motorPower = 1000f;
	public float brakePower  = 400f;

	// Steering input limitter by speed
	public float maxSteer   = 25f;
	public float minSteer   = 7f;

	// Steer,Motor and Brake inputs assigned by keyboardor mobile input buttons
	float steerInput,motorInput,brakeInput;

	// Truck center of mass
	public Vector3 CentreOfMass;


	// Vehicle Effects
	[Space(3)]
	[Header("Vehicle Settings")]
	[Space(3)]
	public ParticleSystem smoke;
	public ParticleSystem roadParticle;
	public float smokeSpeedLimit = 30f;
	public float[] gearRatio;

	// Is reversing? for internal usage
	[HideInInspector]public bool Reversng;
	Vector3 velocity,localVel;

	public bool isGrounded = false;



	void Update ()
	{
		if (SteeringWheelTransform)
			SteeringWheelTransform.rotation = 
				transform.rotation * Quaternion.Euler (0, 0, (Wheel_Colliders [0].steerAngle) * -6);

		// Calculate current speed
		CurrentSpeed = rigid.velocity.magnitude * 2.23693629f;

         // Find truck reversing state
		 velocity = rigid.velocity;
		 localVel = transform.InverseTransformDirection(velocity);

		if (localVel.z > 0)
		{
			Reversng = false;
		}
		else
		{
			Reversng = true;
		}



	}
		
	void Start ()
	{
		if (frontLight) {

			// if (SceneManager.GetActiveScene ().name.Contains ("Garage")) {
			// 	frontLight.SetActive (false);
			// } else {
				
				if (PlayerPrefs.GetInt ("NightMode") == 3)
					frontLight.SetActive (true);
				else
					frontLight.SetActive (false);
			//}
			
		}

		// Catch audio manager component
		audioTruck = GetComponent<TruckAudio> ();

		Wheel_Colliders [0].attachedRigidbody.centerOfMass = CentreOfMass;

		rigid = GetComponent<Rigidbody> ();

		// Start gear managment system
		StartCoroutine (GearChanging ());

		for (int a = 0; a < backLights.Length; a++)
			backLights [a].intensity = 0; 

		backLightMaterial.SetFloat ("_Intensity", 0 );

		emSmoke = smoke.emission;
		emRoad = roadParticle.emission;
	}
		
	IEnumerator GearChanging ()
	{
		while (true) 
		{
			yield return new WaitForSeconds (0.01f);
			if (!Reversng && isGrounded) {
				float f = Mathf.Abs (CurrentSpeed / MaxSpeed);
				float upgearlimit = (1 / (float)numberOfGears) * (currentGear + 1);
				float downgearlimit = (1 / (float)numberOfGears) * currentGear;

				// Changinbg gear down
				if (currentGear > 0 && f < downgearlimit) {
					// Reduce engine audio volume when changing gear
					audioTruck.audioSource.volume = 0.7f;
					audioTruck.ChangeGear ();
					// Delay time for changing gear down
					yield return new WaitForSeconds (0);
					audioTruck.audioSource.volume = 1f;


					currentGear--;
				}

				// Changing gear Up
				if (f > upgearlimit && (currentGear < (numberOfGears - 1))) {
					// Reduce engine audio volume when changing gear
					audioTruck.audioSource.volume = 0.3f;
					audioTruck.ChangeGear ();
					// Delay before changing gear up
					yield return new WaitForSeconds (GearShiftDelay);
					audioTruck.audioSource.volume = 1f;
					currentGear++;
				}
			} else {

				if (Reversng && isGrounded)
					currentGear = 0;
			}
		}
	}
		
	// simple function to add a curved bias towards 1 for a value in the 0-1 range
	private static float CurveFactor (float factor)
	{
		return 1 - (1 - factor) * (1 - factor);
	}
		
	// unclamped version of Lerp, to allow value to exceed the from-to range
	private static float ULerp (float from, float to, float value)
	{
		return (1.0f - value) * from + value * to;
	}

	void FixedUpdate () {

			Engine();

		WheelAlign ();

		Smoke ();

	}
		
	private void CalculateGearFactor ()
	{
		float f = (1 / (float)numberOfGears);
		// gear factor is a normalised representation of the current speed within the current gear's range of speeds.
		// We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
		var targetGearFactor = Mathf.InverseLerp (f * currentGear, f * (currentGear + 1), Mathf.Abs (CurrentSpeed / MaxSpeed));
		GearFactor = Mathf.Lerp (GearFactor, targetGearFactor, Time.deltaTime * 5f);
	}
		
	private void CalculateRevs ()
	{
		// calculate engine revs (for display / sound)
		// (this is done in retrospect - revs are not used in force/power calculations)
		CalculateGearFactor ();
		var gearNumFactor = currentGear / (float)numberOfGears;
		var revsRangeMin = ULerp (0f, 1f, CurveFactor (gearNumFactor));
		var revsRangeMax = ULerp (1f, 1f, gearNumFactor);
		Revs = ULerp (revsRangeMin, revsRangeMax, GearFactor);
	}

	void Engine()
	{
		// Multiply motorPower with motorInput ( Input.GetAxis("Vertical") or 1 and -1 value )
		if (!Reversng) {
			Wheel_Colliders [2].motorTorque = motorPower * motorInput * gearRatio [currentGear];
			Wheel_Colliders [3].motorTorque = motorPower * motorInput * gearRatio [currentGear];
		} else {

			Wheel_Colliders [2].motorTorque = motorPower * motorInput;
			Wheel_Colliders [3].motorTorque = motorPower * motorInput;
		}

		if (Input.GetKey (KeyCode.Space))
		{
			Wheel_Colliders [2].motorTorque = 0;
			Wheel_Colliders [3].motorTorque = 0;
			Wheel_Colliders [2].brakeTorque = brakeInput;
			Wheel_Colliders [3].brakeTorque = brakeInput;
		} else {

			Wheel_Colliders [2].brakeTorque = 0;
			Wheel_Colliders [3].brakeTorque = 0;
		}


		// Steer limitation based on speed
		float speedFactor = 1 - (rigid.velocity.magnitude / MaxSpeed);
		float currentMaxTurnAngle = minSteer + ((maxSteer - minSteer) * speedFactor);
		float turnAmount = currentMaxTurnAngle * steerInput*Time.deltaTime*43f;


		Wheel_Colliders [0].steerAngle = turnAmount;
		Wheel_Colliders [1].steerAngle = turnAmount;

		CalculateRevs ();

	}
		
	// Keyboard input system (Used in truckInput.cs
	public void InputKeyboard()
	{

		float mInput =  Input.GetAxis ("Vertical");
		float sInput = Input.GetAxis ("Horizontal");
		float bInput;

		if (Input.GetKey (KeyCode.Space))
			bInput = 1f * brakePower;
		else
			bInput = 0;
		


		Move (sInput, mInput, bInput);
	}

	public void Move(float steer,float throttle,float brake)
	{

		if (!canControl) {
			brakeInput = 1f*brakePower  ;
			return;

		}

		steerInput = steer;

		if (CurrentSpeed < MaxSpeed) 
			motorInput = throttle;

		brakeInput = brake;   

		   
	}
	// Truck Effects
	ParticleSystem.EmissionModule emSmoke,emRoad ;
	void Smoke()
	{

		if (CurrentSpeed < smokeSpeedLimit) {
			if (motorInput > 0) {
				if (!emSmoke.enabled)
					emSmoke.enabled = true;
			} else {
				if (emSmoke.enabled)
					emSmoke.enabled = false;
			}

			if (emRoad.enabled)
				emRoad.enabled = false;
		}
		else {
			if (emSmoke.enabled)
				emSmoke.enabled = false;   

			if (!emRoad.enabled)
				emRoad.enabled = true;
		}



	}
	// Wheels visual alignment across wheel colliders
	WheelHit wHit;

	void WheelAlign()
	{

		// Find ground hit (for disabling gear changing on the air)
		if(Wheel_Colliders[0].GetGroundHit(out wHit))
			isGrounded = true;
		else
			isGrounded = false;
		 
		// Total wheels
		for (int i = 0; i < 4; i++)
		{
			Quaternion quat;
			Vector3 pos;
			Wheel_Colliders[i].GetWorldPose(out pos,out quat);
			Wheel_Transforms[i].transform.position = pos;
			Wheel_Transforms[i].transform.rotation = quat;
		}

		// Extra rear wheels
		Quaternion quat_extra_left;
		Vector3 pos_extra_left;
		extra_Rear_Left_WC.GetWorldPose(out pos_extra_left,out quat_extra_left);
		extra_Rear_Left_T.transform.position = pos_extra_left;
		extra_Rear_Left_T.transform.rotation = quat_extra_left;

		Quaternion quat_extra_right;
		Vector3 pos_extra_right;
		extra_Rear_Right_WC.GetWorldPose(out pos_extra_right,out quat_extra_right);
		extra_Rear_Right_T.transform.position = pos_extra_right;
		extra_Rear_Right_T.transform.rotation = quat_extra_right;
	}
}
﻿// This script used for car SteeringWheel

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class SteeringWheel : MonoBehaviour
{
	public bool wheelBeingHeld = false;
	
	[SerializeField] private RectTransform wheelUI;
	[SerializeField] private GameObject wheelTouchzone;
	[SerializeField] private float wheelReleasedSpeed = 200f;
	[SerializeField] private float maximumSteeringAngle = 200f;
 
	private Vector2 centerPoint;
	private float wheelAngle = 0f;
	private float wheelPrevAngle = 0f;

	public float GetClampedValue ()
	{
		// returns a value in range [-1,1] similar to GetAxis("Horizontal")
		return wheelAngle / maximumSteeringAngle;
	}

	public float GetAngle ()
	{
		// returns the wheel angle itself without clamp operation
		return wheelAngle;
	}

	void Start ()
	{
		InitEventsSystem();
		UpdateRect();
	}
	
	public void ResetAngle()
	{
		wheelAngle = 0f;
		wheelUI.localEulerAngles = Vector3.back * wheelAngle;
		wheelBeingHeld = false;
	}


	void Update ()
	{
		// If the wheel is released, reset the rotation
		// to initial (zero) rotation by wheelReleasedSpeed degrees per second
		if (!wheelBeingHeld && !Mathf.Approximately (0f, wheelAngle)) 
		{
			float deltaAngle = wheelReleasedSpeed * Time.deltaTime;
			if (Mathf.Abs (deltaAngle) > Mathf.Abs (wheelAngle))
				wheelAngle = 0f;
			else if (wheelAngle > 0f)
				wheelAngle -= deltaAngle;
			else
				wheelAngle += deltaAngle;
		}
 
		// Rotate the wheel image
		wheelUI.localEulerAngles = Vector3.back * wheelAngle;
	}

	void InitEventsSystem ()
	{
		// Warning: Be ready to see some extremely boring code here :-/
		// You are warned!
		EventTrigger events = wheelTouchzone.GetComponent<EventTrigger> ();
 
		if (events == null)
			events = wheelTouchzone.AddComponent<EventTrigger> ();
 
		if (events.triggers == null)
			events.triggers = new System.Collections.Generic.List<EventTrigger.Entry> ();
 
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		EventTrigger.TriggerEvent callback = new EventTrigger.TriggerEvent ();
		UnityAction<BaseEventData> functionCall = new UnityAction<BaseEventData> (PressEvent);
		callback.AddListener (functionCall);
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback = callback;
 
		events.triggers.Add (entry);
 
		entry = new EventTrigger.Entry ();
		callback = new EventTrigger.TriggerEvent ();
		functionCall = new UnityAction<BaseEventData> (DragEvent);
		callback.AddListener (functionCall);
		entry.eventID = EventTriggerType.Drag;
		entry.callback = callback;
 
		events.triggers.Add (entry);
 
		entry = new EventTrigger.Entry ();
		callback = new EventTrigger.TriggerEvent ();
		functionCall = new UnityAction<BaseEventData> (ReleaseEvent);//
		callback.AddListener (functionCall);
		entry.eventID = EventTriggerType.PointerUp;
		entry.callback = callback;
 
		events.triggers.Add (entry);
	}

	void UpdateRect ()
	{
		// Credit to: mwk888 from unityAnswers
		Vector3[] corners = new Vector3[4];
		wheelUI.GetWorldCorners (corners);
 
		for (int i = 0; i < 4; i++) {
			corners [i] = RectTransformUtility.WorldToScreenPoint (null, corners [i]);
		}
 
		Vector3 bottomLeft = corners [0];
		Vector3 topRight = corners [2];
		float width = topRight.x - bottomLeft.x;
		float height = topRight.y - bottomLeft.y;
 
		Rect _rect = new Rect (bottomLeft.x, topRight.y, width, height);
		centerPoint = new Vector2 (_rect.x + _rect.width * 0.5f, _rect.y - _rect.height * 0.5f);
	}

	public void PressEvent (BaseEventData eventData)
	{
		// Executed when mouse/finger starts touching the steering wheel
		Vector2 pointerPos = ((PointerEventData)eventData).position;
 
		wheelBeingHeld = true;
		wheelPrevAngle = Vector2.Angle (Vector2.up, pointerPos - centerPoint);
	}

	public void DragEvent (BaseEventData eventData)
	{

		// Executed when mouse/finger is dragged over the steering wheel
		Vector2 pointerPos = ((PointerEventData)eventData).position;
 
		float wheelNewAngle = Vector2.Angle (Vector2.up, pointerPos - centerPoint);
		// Do nothing if the pointer is too close to the center of the wheel
		if (Vector2.Distance (pointerPos, centerPoint) > 20f) {
			if (pointerPos.x > centerPoint.x)
				wheelAngle += wheelNewAngle - wheelPrevAngle;
			else
				wheelAngle -= wheelNewAngle - wheelPrevAngle;
		}
		
		//Debug.Log("Trigged! wheelAngle = " + wheelAngle);

		// Make sure wheel angle never exceeds maximumSteeringAngle
		wheelAngle = Mathf.Clamp (wheelAngle, -maximumSteeringAngle, maximumSteeringAngle);
		wheelPrevAngle = wheelNewAngle;
	}

	public void ReleaseEvent (BaseEventData eventData)
	{
		// Executed when mouse/finger stops touching the steering wheel
		// Performs one last DragEvent, just in case
		DragEvent (eventData);
 
		wheelBeingHeld = false;
	}
}
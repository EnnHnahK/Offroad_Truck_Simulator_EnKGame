using NWH.Common.Input;
using UnityEngine;
using UnityEngine.EventSystems;

public class SteeringWheelInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	private bool _isThrottle = false;
	private bool _tutorial;
	[SerializeField] private MobileInputButton btnSteering;

	public void OnPointerDown(PointerEventData eventData)
	{
		if(!RaceManager.Instance.RaceRunning()) return;
		
		if(UIController.Instance.uIGamePlay.GetCurFuel() <= 0) return;

		
		UIController.Instance.uIGamePlay.TutorialSteering();

		_isThrottle = true;
		
		UIController.Instance.uIGamePlay.UsingFuel(true);
	}
	
	public void OnPointerUp(PointerEventData eventData)
	{
		if(UIController.Instance.uIGamePlay.GetCurFuel() <= 0) return;
		_isThrottle = false;
		
		UIController.Instance.uIGamePlay.UsingFuel(false);
	}

	public bool GetThrottleStatus()
	{
		return _isThrottle;
	}
	
	public void StopSteeringWheel()
	{
		OnPointerUp(null);
		
		btnSteering.deactive = true;
		btnSteering.isPressed = false;
		_isThrottle = false;
	}
}

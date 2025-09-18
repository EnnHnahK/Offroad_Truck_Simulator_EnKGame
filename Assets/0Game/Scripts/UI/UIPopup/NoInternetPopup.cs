using UnityEngine;

public class NoInternetPopup : BasePopup
{
	[SerializeField] private ButtonEffectLogic btnOK;
	
	protected override void Awake()
	{
		base.Awake();
		btnOK.onClick.AddListener(OpenWifi);
	}
	
	private void FixedUpdate()
	{
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			Hide();
		}
	}
	
	private void OpenWifi()
	{
        #if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
	        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
	        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.settings.WIFI_SETTINGS");
	        currentActivity.Call("startActivity", intent);
        #endif
	}
	
}

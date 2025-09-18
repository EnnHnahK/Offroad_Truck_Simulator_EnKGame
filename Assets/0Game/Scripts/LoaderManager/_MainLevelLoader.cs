using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _MainLevelLoader : MonoBehaviour
{
    public static _MainLevelLoader Instance { get; private set; }

	//private const string LEVEL_PRE_STRING = "Levels/Level";
	private const string TRUCK_PRE_STRING = "Trucks/Truck";

	private float _preLoadingTime = 0.2f;

	//private bool _alreadyLoading = false;
	//private GameObject _currentLevelGameObject = null;

	private int lastIndex;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		lastIndex = DataController.CurLevel;
		LoadLevel(lastIndex);
	}

	public void LoadNextLevel()
	{
		PrefData.MapLevel++;
		PrefData.CurLevel++;
		if (DataController.CurLevel >= DataController.CountLevel)
		{
			PrefData.CurLevel = DataController.CountLevel - 1;
		}
		
	    UIController.Instance.uIChoseLevel.UnlockLevel(DataController.CurLevel);
	    UIController.Instance.uIGarage.LoadData();
	    //DOVirtual.DelayedCall(1f, () => LoadLevel(DataController.CurLevel));
	    LoadLevel(DataController.CurLevel);
	    
	    //Update Base Fuel
	    UIController.Instance.uIGamePlay.SetBaseFuel(DataController.Instance.GetCurLevelBaseFuel());
	}

	public void LoadLevel(int level)
	{
		//StartCoroutine(LoadLevelCoroutine(level));
		StartCoroutine(LoadSceneAsync(level / 3 + 1));
	}

	#region old
	// private IEnumerator LoadLevelCoroutine(int level)
	// {
	// 	if (!_alreadyLoading)
	// 	{
	// 		_alreadyLoading = true;
	// 		DestroyOld();
	// 	}
	//
	// 	yield return Yielder.GetWaitForSeconds(_preLoadingTime);
	// 	var resourceRequest = Resources.LoadAsync<GameObject>(LEVEL_PRE_STRING + level);
	// 	while (resourceRequest.isDone != true)
	// 	{
	// 		yield return null;
	// 	}
	//
	// 	if (resourceRequest.asset == null)
	// 	{
	// 		PrefData.CurLevel = lastIndex;
	// 		LoadLevel(lastIndex);
	// 	}
	// 	else
	// 	{
	// 		lastIndex = level;
	// 		_currentLevelGameObject = Instantiate(resourceRequest.asset as GameObject);
	// 		_alreadyLoading = false;
	// 		LoadComplete();
	// 	}
	// 	VehicleManager.Instance.SpawnTruck();
	// }
	//
	// void DestroyOld()
	// {
	// 	if (_currentLevelGameObject != null)
	// 	{
	// 		Destroy(_currentLevelGameObject);
	// 	}
	// 	UIController.Instance.inputSystem.SetInsLv(false);
	// }
	#endregion
	
	
	IEnumerator LoadSceneAsync(int scene)
	{
		yield return Yielder.GetWaitForSeconds(_preLoadingTime);
		AsyncOperation async = SceneManager.LoadSceneAsync(scene);
		async.allowSceneActivation = false;
		yield return Consts.OneSec;
      
		while (!async.isDone)
		{
			yield return null;
			if (async.progress >= 0.9f)
			{
				async.allowSceneActivation = true;
			}
		}

		async.allowSceneActivation = true;
		VehicleManager.Instance.SpawnTruck();
	}

	IEnumerator LoadSceneAsync(string scene)
	{
		AsyncOperation async = SceneManager.LoadSceneAsync(scene);
		async.allowSceneActivation = false;
		yield return Consts.OneSec;
      
		while (!async.isDone)
		{
			yield return null;
			if (async.progress >= 0.9f)
			{
				async.allowSceneActivation = true;
			}
		}

		async.allowSceneActivation = true;
	}
	
	
	public GameObject LoadTruck(int truckIndex)
	{
		return Resources.Load(TRUCK_PRE_STRING + truckIndex.ToString()) as GameObject;
	}
}
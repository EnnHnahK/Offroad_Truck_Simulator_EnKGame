using System.Collections;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;
    
    public TypeWeather weather;
    public Vector3[] posCar;
    public Transform posSpawn;

    //public Transform posSpawn;
    public TerrainDeformer terrainDeform;
   
    
    //Level Time
    [SerializeField] private Material skyBox;
    [SerializeField] private CurrentTime dayTime;

    private void Awake()
    {
        Instance = this;
        UIController.Instance.uIGamePlay.SetDirtyWindow(weather);
        if (weather == TypeWeather.Rain)
        {
            SoundManager.Instance.PlayLoop(SoundManager.Instance.rain);
        }

        //terrainDeform.GetComponent<TerrainCollider>(). = true;
        
        // // Tìm tất cả các đối tượng có tag "Road"
        // GameObject[] enemies = GameObject.FindGameObjectsWithTag("Road");
        //
        // // Duyệt qua tất cả các đối tượng tìm thấy
        // foreach (GameObject enemy in enemies)
        // {
        //     Debug.Log("Found Road: " + enemy.name);
        // }
    }
    
    private IEnumerator Start()
    {
        RenderSettings.skybox = skyBox;
        yield return new WaitUntil(() => _TruckManager.Instance);
        yield return new WaitUntil(() => _TruckManager.Instance.GetVehicleFunction());
        _TruckManager.Instance.GetVehicleFunction().CheckLight();
    }

    private void OnDisable()
    {
        UIController.Instance.uIGamePlay.SetDirtyWindow(weather, false);
        if (weather == TypeWeather.Rain)
        {
            SoundManager.Instance.StopLoop(SoundManager.Instance.rain);
        }
    }
    
    public CurrentTime GetCurrentTime()
    {
        return dayTime;
    }
}

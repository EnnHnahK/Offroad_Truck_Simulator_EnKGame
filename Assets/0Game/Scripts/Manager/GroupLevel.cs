using UnityEngine;

public class GroupLevel : MonoBehaviour
{
    public static GroupLevel Instance;
    
    public Color colorParWheel;
    public Color colorParMud = new Color(0.4509804f,0.4705883f,0.2509804f , 1);
    
    [SerializeField] private LevelController[] levels;

    [SerializeField] private Light lightSun;

    [SerializeField] private Transform weather;
    [SerializeField] private GameObject curParticle;
    [SerializeField] private GameObject particleRain;

    private float _intensityDf;
    
    private void Awake()
    {
        Instance = this;
        _intensityDf = lightSun.intensity;
        foreach (var level in levels)
        {
            level.gameObject.SetActive(false);
        }
        GameManager.CountPlay++;
        levels[DataController.CurLevel % 3].gameObject.SetActive(true);
        CheckWeather();
    }
    
    public void TakeWeather(Transform tParent = null)
    {
        weather.SetParent(tParent);
        weather.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
    
    private void CheckWeather()
    {
        if (GameManager.CountPlay % 4 == 0)
        {
            lightSun.intensity = _intensityDf -.5f;
            if (curParticle)
            {
                Destroy(curParticle);
            }
            particleRain.SetActive(true);
        }
        else
        {
            lightSun.intensity = _intensityDf;
        }
    }
}

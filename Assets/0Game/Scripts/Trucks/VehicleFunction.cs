using System;
using System.Collections;
using NWH.VehiclePhysics2.GroundDetection;
using NWH.VehiclePhysics2.Powertrain;
using NWH.WheelController3D;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public enum DragType
{
    None,
    Mud,
    Water,
    UpHill,
    Snow,
    Sand
}

public class VehicleFunction : MonoBehaviour
{
    public DragType DragType;
    public UnityEvent<float> powerEngine;

    [SerializeField] private Light[] headLights;
    [SerializeField] private Light[] brakeLights;
    [SerializeField] private Light[] reverseLights;
    [SerializeField] private ParticleSystem[] particlesWheel;
    private WheelController[] wheelsControl;


    private SurfacePreset _presetDf;
    private TransmissionComponent _transmission;

    private float _dragNormal = 0.05f;
    private float _dragInMud = 2.8f;
    private float _dragInWater = 2.2f;
    private float _upHill = .8f;
    private Coroutine _reversing;


    //[SerializeField] private float maxAngularVelo = 20;
    [SerializeField] private float maxEmission = 30;
    [SerializeField] private Vector2 veloLifetime1 = new Vector2(-1, -10);
    [SerializeField] private Vector2 veloLifetime2 = new Vector2(0, -8);

    private IEnumerator Start()
    {;
        if (_TruckManager.Instance == null)
        {
            yield return new WaitUntil(() => _TruckManager.Instance);
        }

        GroupLevel.Instance.TakeWeather(transform);
        wheelsControl = _TruckManager.Instance.GetWheels();
        var vehicleControl = _TruckManager.Instance.GetVehicleControl();
        _presetDf = vehicleControl.groundDetection.groundDetectionPreset.fallbackSurfacePreset;
        _transmission = vehicleControl.powertrain.transmission;
        ChangColorParWheel(DragType);
    }

    void ChangColorParWheel(DragType dragType)
    {
        switch (dragType)
        {
            case DragType.Mud:
                foreach (var par in particlesWheel)
                {
                    if (par)
                    {
                        var mainModule = par.main;
                        mainModule.startColor = GroupLevel.Instance.colorParMud;
                    }
                }
                break;
            default:
                foreach (var par in particlesWheel)
                {
                    if (par)
                    {
                        var mainModule = par.main;
                        mainModule.startColor = GroupLevel.Instance.colorParWheel;
                    }
                }
                break;
        }
    }
    
    private void OnDestroy()
    {
        GroupLevel.Instance.TakeWeather(null);
    }

    private float lastAngle;
    private void LateUpdate()
    {
        //var shiftRPM = _transmission.ReferenceShiftRPM;
        //var maxRPM = (_reversing != null) ? _transmission.TargetDownshiftRPM : _transmission.TargetUpshiftRPM;
        //var maxRPM = _transmission.TargetUpshiftRPM;
        float ratio = 0 ;
        for (int i = 0; i < particlesWheel.Length; i++)
        {
            var wheel = wheelsControl[i];
            var parWheel = particlesWheel[i];
            if (parWheel == null)
            {
                continue;
            }

            if (i < 2)
            {
                var curAngle = parWheel.transform.localEulerAngles;
                curAngle.y = wheel.SteerAngle;
                parWheel.transform.localEulerAngles = curAngle;

            }

            var ratioWheel = wheel.AngularVelocity / 10f;
            if (ratioWheel > ratio)
            {
                ratio = ratioWheel;
            }
            if (wheel.FrictionPreset != _presetDf.frictionPreset && _reversing == null || DragType == DragType.Mud )
            {
                if (parWheel.isPlaying)
                {
                    if (ratio < .5f)
                    {
                        parWheel.gameObject.SetActive(false);
                        continue;
                    }
                }
                else
                {
                    parWheel.gameObject.SetActive(true);
                }

                var emission = parWheel.emission;
                emission.rateOverTime = Mathf.Lerp(0, maxEmission, ratio);
                var velocityOverLifetime = parWheel.velocityOverLifetime;
                Vector2 randomZ = new Vector2(Mathf.Lerp(veloLifetime1.x, veloLifetime1.y, ratio), Mathf.Lerp(veloLifetime2.x, veloLifetime2.y, ratio));
                velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(randomZ.x, randomZ.y);
            }
            else
            {
                if (parWheel.isPlaying)
                {
                    parWheel.gameObject.SetActive(false);
                }
            }
        }
        
        powerEngine?.Invoke(ratio);
    }

    public void CheckLight()
    {
#if UNITY_EDITOR || DEBUGLOG
        Debug.LogError("Night but Light not set");
#endif
        return;
        if (LevelController.Instance.GetCurrentTime() == CurrentTime.Night)
        {
            foreach (Light headlight in headLights)
            {
                headlight.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Light headlight in headLights)
            {
                headlight.gameObject.SetActive(false);
            }
        }
    }

    public void BrakeLightIntensity(bool on)
    {
        foreach (var l in brakeLights)
        {
            l.gameObject.SetActive(@on);
        }
    }

    public void ReverseLightIntensity(bool on)
    {
        if (on)
        {
            if (_reversing == null)
            {
                _reversing = StartCoroutine(AnimRevert());
            }
        }
        else
        {
            foreach (var l in reverseLights)
            {
                l.gameObject.SetActive(false);
            }

            StopCoroutine(_reversing);
            _reversing = null;
        }
    }

    IEnumerator AnimRevert()
    {
        while (gameObject)
        {
            foreach (var l in reverseLights)
            {
                l.gameObject.SetActive(true);
            }

            yield return Yielder.GetWaitForSeconds(.5f);
            foreach (var l in reverseLights)
            {
                l.gameObject.SetActive(false);
            }

            yield return Yielder.GetWaitForSeconds(.5f);
        }
    }

    public void Slow(DragType dragType)
    {
        if (DragType == dragType)
            return;

        DragType = dragType;
        float drag = _dragNormal;
        ChangColorParWheel(dragType);
        if (dragType == DragType.UpHill)
        {
            drag = _upHill;
        }

        if (dragType == DragType.Mud)
        {
            drag = _dragInMud;
        }
        else if (dragType == DragType.Water)
        {
            drag = _dragInWater;
        }

        _TruckManager.Instance.Slow(drag);
    }
}
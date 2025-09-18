using System;
using NWH.VehiclePhysics2.Modules.Trailer;
using UnityEngine;

public class TrailerMission : MonoBehaviour
{
    [SerializeField] private GameObject trailerCam;
    [SerializeField] private DirectionArrow directionArrow;
    private TrailerHitchModule _trailerHitchModule;

    private void Start()
    {
        _trailerHitchModule = _TruckManager.Instance.GetVehicleControl().GetComponent<TrailerHitchModuleWrapper>()?.GetModule() as TrailerHitchModule;
    }

    public void ToggleTrailerCam(bool isActive, bool isFixed = false)
    {
        if ((UIController.Instance.uINotify.GetWorkingStatus() || isFixed) && !MissionManager.Instance.CheckMissionStatus(MissionStatus.Finished))
        {
            trailerCam.SetActive(isActive);
            _TruckManager.Instance.Slow();
        }
    }

    public void TrailerAttached()
    {
        MissionManager.Instance.TrailerAttached();
    }

    [Button]
    public void TrailerDetach()
    {
        _trailerHitchModule.DetachTrailer(_TruckManager.Instance.GetVehicleControl());
        //MissionManager.Instance.TrailerDetached();

    }

    public void SetTarget(Transform target)
    {
        if(!directionArrow.gameObject.activeSelf) directionArrow.gameObject.SetActive(true);
        
        directionArrow.SetTarget(target);
    }
    
    public void UnTarget()
    {
        if(directionArrow.gameObject.activeSelf) directionArrow.gameObject.SetActive(false);
    }
    
    
}

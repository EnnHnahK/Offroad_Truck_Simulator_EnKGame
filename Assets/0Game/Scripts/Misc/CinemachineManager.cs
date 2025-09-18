using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CinemachineManager : MonoBehaviour
{
    public static CinemachineManager Instance { get; private set; }

    private CinemachineVirtualCamera _vcam;

    private CinemachineTransposer _vcamTransposer;

    private CinemachineComposer _vcamComposer;
    private Vector3 runForwardOffset;
    private Vector3 _vcamComposerDefaultOffset;
    private Transform _vehicleController;
    private Vector3 idleLeftOffset;
    public float idleSpeedToChangeOffset = 5;

    private void Awake()
    {
        Instance = this;

        _vcam = GetComponent<CinemachineVirtualCamera>();
        _vcamTransposer = _vcam.GetCinemachineComponent<CinemachineTransposer>();
        
        _vcamComposer = _vcam.GetCinemachineComponent<CinemachineComposer>();

        runForwardOffset = _vcamTransposer.m_FollowOffset;
        idleLeftOffset = runForwardOffset;
        _vcamComposerDefaultOffset = _vcamComposer.m_TrackedObjectOffset;
    }

    public void SetupCam()
    {
        
        _vehicleController = _TruckManager.Instance.GetVehicleControl().transform;
        
        
        _vcam.LookAt = _vehicleController;
        _vcam.Follow = _vehicleController;

        _vcam.PreviousStateIsValid = false;

        _TruckManager.Instance.GetTruckOnCollider().OnTruckLose += CancelFollow;
        _TruckManager.Instance.GetTruckOnCollider().OnTruckWin += CancelFollow;
    }


    private void CancelFollow()
    {
        _vcam.Follow = null;
    }

    public void SetUpVirtualCamera(float xIdleOffset, Vector3 followOffset, Vector3 trackedObjectOffset)
    {
        if (followOffset != Vector3.zero)
        {
            _vcamTransposer.m_FollowOffset = followOffset;
        }
        else
        {
            _vcamTransposer.m_FollowOffset = runForwardOffset;
        }

        idleLeftOffset.x = xIdleOffset;

        if (trackedObjectOffset != Vector3.zero)
        {
            _vcamComposer.m_TrackedObjectOffset = trackedObjectOffset;
        }
        else
        {
            _vcamComposer.m_TrackedObjectOffset = _vcamComposerDefaultOffset;
        }
    }

    public void SetBindingMode(bool lookAt)
    {
        //_vcamTransposer.
    }

    public bool runRotate;
    public float camRotateSpeed;
    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (_TruckManager.Instance)
        {
            SetRotate(_TruckManager.Instance.GetVehicleControl().Speed);
        }
        else
        {
            SetRotate(0);
        }
    }

    public void SetRotate(float speed)
    {
        if (!runRotate)
        {
            return;
        }

        //_vcamChannel.m_AmplitudeGain = canControl ? 0 : 1;
        Vector3 targetOffset = runForwardOffset;
        if (speed < idleSpeedToChangeOffset)
        {
            targetOffset = idleLeftOffset;
        }

        _vcamTransposer.m_FollowOffset = Vector3.SmoothDamp(_vcamTransposer.m_FollowOffset, targetOffset, ref velocity,
            Time.deltaTime * camRotateSpeed);

    }

    public void SetRunRotate(bool run)
    {
        runRotate = run;
        if (!run)
        {
            _vcamTransposer.m_FollowOffset = idleLeftOffset;
        }
    }

    [SerializeField] private Transform cmCamTemp;

    public void LookAt(Transform pos, bool lookBack = false)
    {
        if (lookBack)
        {
            var curCamLook = _vcam.LookAt;
            DOVirtual.DelayedCall(3f, () =>
            {
                SmoothLookAt(curCamLook);
            });
        }
        SmoothLookAt(pos);
    }

    private void SmoothLookAt(Transform pos)
    {
        cmCamTemp.position = _vcam.LookAt.position;
        _vcam.LookAt = cmCamTemp;
        
        cmCamTemp.DOMove(pos.position, 2f).OnComplete(() =>
        {
            _vcam.LookAt = pos;
        });

    }
    
}

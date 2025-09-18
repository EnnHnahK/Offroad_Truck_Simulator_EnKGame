using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CraneItemMission : MonoBehaviour
{
    [Header("Crane Controller")]
    [SerializeField] private Transform[] pistonTransforms, supHorizontal, itemTransform;
    
    [SerializeField] private Transform craneAxis, mainCrane, mainHorizontal, craneHook;

    private bool _isWorking, _wheelWorking, _doneWorking;
    private int _countItem = 0;

    private Vector3[] _curEulerPistons, _curSubHorizLocalPos;
    private Vector3 _curEulerMain, _curEulerMainHoriz;

    private Transform _targetPos;
    private Camera _camera;

    [SerializeField] private GameObject cam;
    private void Awake()
    {
        _camera = Camera.main;
        
        _curEulerPistons = new Vector3[pistonTransforms.Length];
        _curSubHorizLocalPos = new Vector3[supHorizontal.Length];
        
        for (int i = 0; i < pistonTransforms.Length; i++)
        {
            _curEulerPistons[i] = pistonTransforms[i].localEulerAngles;
        }

        for (int i = 0; i < _curSubHorizLocalPos.Length; i++)
        {
            _curSubHorizLocalPos[i] = supHorizontal[i].localPosition;
        }
        
        _curEulerMain = mainCrane.localEulerAngles;
        _curEulerMainHoriz = mainHorizontal.localEulerAngles;
    }

    private void OnEnable()
    {
        _wheelWorking = false;
        _countItem = 0;
    }

    public void ToggleMission(Transform pos, bool isReceiving)
    {
        CraneSetup(isReceiving);

        _targetPos = pos;
    }
    
    private void Update()
    {
        if (_wheelWorking)
        {
            RotateCrane();
        }
    }
    
    [Button]
    private void CraneSetup(bool isReceiving)
    {
        var euler = new Vector3(0, 0, 249f);
        
        mainCrane.DOLocalRotate(euler, 1f, RotateMode.FastBeyond360);
        
        for (int i = 0; i < pistonTransforms.Length; i++)
        {
            if (i == pistonTransforms.Length - 1)
            {
                euler.z = 264;
            }
            pistonTransforms[i].DOLocalRotate(euler, 1f, RotateMode.FastBeyond360);
        }
        euler.z = 111;
        mainHorizontal.DOLocalRotate(euler, 1f, RotateMode.FastBeyond360);

        DOVirtual.DelayedCall(1.2f, () =>
        {
            _wheelWorking = true;
            UIController.Instance.SetWorkingInInput(_wheelWorking);
            
            cam.SetActive(true);

            StartCoroutine(!isReceiving ? IECraneDelivery() : IECraneRotate());
        });
    }

    
    private void RotateCrane()
    {
        if(_isWorking) return;
		
        float rotationSpeed = UIController.Instance.uIGamePlay.GetAngleClamped() * 100f;

        Quaternion currentRotation = craneAxis.localRotation;
        
        Quaternion rotationIncrement = Quaternion.Euler(rotationSpeed * Time.deltaTime, 0, 0);
        
        craneAxis.localRotation = currentRotation * rotationIncrement;
    }

    private IEnumerator IECraneRotate()
    {
        yield return Yielder.GetWaitForSeconds(1f);
        while (!_doneWorking && _targetPos != null)
        {
            Vector3 viewportPos = _camera.WorldToViewportPoint(_targetPos.position);

            float distanceFromCenterX = Mathf.Abs(viewportPos.x - 0.5f);
            float distanceFromCenterY = Mathf.Abs(viewportPos.y - 0.5f);

            float tolerance = 0.1f;
            
            if (distanceFromCenterX < tolerance && distanceFromCenterY < tolerance * 3)
            {
                    _wheelWorking = false;
                    var dis = Vector3.Distance(_TruckManager.Instance.GetVehiclePos(), _targetPos.position) - 2.7f;
                
                    for (int j = 0; j < supHorizontal.Length; j++)
                    {
                        var posY = supHorizontal[j].localPosition.y;
                        supHorizontal[j].DOLocalMoveY(posY + Mathf.Min(dis - j, 1f), .5f).From(posY);
                        yield return Yielder.GetWaitForSeconds(.5f);
                    }
                    
                    var item = MissionManager.Instance.GetItemCraneTransform(_countItem);
                    var curCraneHookPos = craneHook.position;
                    var itemPos = item.position;
                    itemPos.y += .88f;
                    craneHook.DOMoveY(itemPos.y, 1f).OnComplete(() =>
                    {
                        item.SetParent(craneHook);
                        var itemLocalPos = item.localPosition;
                        item.DOLocalMove(new Vector3(0, itemLocalPos.y, 0), 1f);
                    });
                    yield return Yielder.GetWaitForSeconds(2f);
                    craneHook.DOMoveY(curCraneHookPos.y, 1f);
                    yield return Yielder.GetWaitForSeconds(1f);

                    for (int j = 0; j < supHorizontal.Length; j++)
                    {
                        var posY = _curSubHorizLocalPos[j].y;
                        supHorizontal[j].DOLocalMoveY(posY, .5f);
                        yield return Yielder.GetWaitForSeconds(.5f);
                    }

                    Quaternion targetRotation = Quaternion.Euler(0, 270, 0);
                    craneAxis.DOLocalRotateQuaternion(targetRotation, 1f);
                    yield return Yielder.GetWaitForSeconds(1f);
                    item.SetParent(itemTransform[_countItem]);
                    item.DOLocalRotateQuaternion(Quaternion.identity, 1f);
                    item.DOLocalMove(Vector3.zero, 1f);
                    yield return Yielder.GetWaitForSeconds(1f);
                    _wheelWorking = true;

                    _countItem++;
                /*}*/
                
                if (_countItem >= 5)
                {
                    _wheelWorking = false;
                    CraneEnd(true);
                    _countItem--;
                    yield break;
                }
            }

            yield return Yielder.GetWaitForSeconds(.2f);
        }
    }
    
      private IEnumerator IECraneDelivery()
    {
        var item = MissionManager.Instance.GetItemCraneTransform(_countItem);
                    
        Quaternion firstTarget = Quaternion.Euler(0, 270, 0);
        craneAxis.DOLocalRotateQuaternion(firstTarget, 1f);
                    
        yield return Yielder.GetWaitForSeconds(1f);
                    
        item.SetParent(craneHook);
        var itemLocalPos = item.localPosition;
        item.DOLocalMove(new Vector3(0, itemLocalPos.y, 0), 1f);
                    
        yield return Yielder.GetWaitForSeconds(1f);
        
        while (!_doneWorking && _targetPos != null)
        {
            Vector3 viewportPos = _camera.WorldToViewportPoint(_targetPos.position);

            float distanceFromCenterX = Mathf.Abs(viewportPos.x - 0.5f);
            float distanceFromCenterY = Mathf.Abs(viewportPos.y - 0.5f);

            float tolerance = 0.1f;
            
            if (distanceFromCenterX < tolerance && distanceFromCenterY < tolerance * 3)
            {
                
                    _wheelWorking = false;
                    
                    var dis = Vector3.Distance(_TruckManager.Instance.GetVehiclePos(), _targetPos.position) - 2.7f;
                
                    for (int j = 0; j < supHorizontal.Length; j++)
                    {
                        var posY = supHorizontal[j].localPosition.y;
                        supHorizontal[j].DOLocalMoveY(posY + Mathf.Min(dis - j, 1f), .5f).From(posY);
                        yield return Yielder.GetWaitForSeconds(.5f);
                    }

                    var curItem = MissionManager.Instance.GetItemCraneTransform(_countItem);
                    var itemCraneParent = MissionManager.Instance.GetItemCraneParent(_countItem);
                    var curCraneHookPos = craneHook.position;
                    var itemPos = itemCraneParent.position;
                    itemPos.y += .88f;
                    craneHook.DOMoveY(itemPos.y, 1f).OnComplete(() =>
                    {
                        curItem.SetParent(itemCraneParent);
                        curItem.DOLocalMove(new Vector3(0, 0, 0), 1f);
                    });
                    yield return Yielder.GetWaitForSeconds(2f);
                    craneHook.DOMoveY(curCraneHookPos.y, 1f);
                    yield return Yielder.GetWaitForSeconds(1f);

                    for (int j = 0; j < supHorizontal.Length; j++)
                    {
                        var posY = _curSubHorizLocalPos[j].y;
                        supHorizontal[j].DOLocalMoveY(posY, .5f);
                        yield return Yielder.GetWaitForSeconds(.5f);
                    }

                    _countItem--;
                    
                    if (_countItem  < 0)
                    {
                        _wheelWorking = false;
                        
                        craneAxis.DOLocalRotateQuaternion( Quaternion.Euler(0, 270, 0), 1f);
                        
                        CraneEnd(false);
                        yield break;
                    }
                    
                    var nextItem = MissionManager.Instance.GetItemCraneTransform(_countItem);
                    
                    Quaternion targetQuaternion = Quaternion.Euler(0, 270, 0);
                    craneAxis.DOLocalRotateQuaternion(targetQuaternion, 1f);
                    
                    yield return Yielder.GetWaitForSeconds(1f);
                    
                    nextItem.SetParent(craneHook);
                    var nextItemLocalPosition = nextItem.localPosition;
                    nextItem.DOLocalMove(new Vector3(0, nextItemLocalPosition.y, 0), 1f);
                    
                    yield return Yielder.GetWaitForSeconds(1f);
                    

                    _wheelWorking = true;

            }

            yield return Yielder.GetWaitForSeconds(.2f);
        }
    }
    
    [Button]
    private void CraneEnd(bool isReceiving)
    {
        for (int i = 0; i < pistonTransforms.Length; i++)
        {
            pistonTransforms[i].DOLocalRotate(_curEulerPistons[i], 1f, RotateMode.FastBeyond360);
        }
        mainCrane.DOLocalRotate(_curEulerMain, 1f, RotateMode.FastBeyond360);
        mainHorizontal.DOLocalRotate(_curEulerMainHoriz, 1f, RotateMode.FastBeyond360);
        
        DOVirtual.DelayedCall(1.2f, () =>
        {
            UIController.Instance.SetWorkingInInput(_wheelWorking);
            MissionManager.Instance.DragItemDone(isReceiving);
            cam.SetActive(false);
        });
    }

}

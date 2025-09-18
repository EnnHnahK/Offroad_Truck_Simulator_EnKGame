using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TestWater : MonoBehaviour
{
    public List<Collider> _cols = new ();
    //private float _time = 2;
    private Vector3 angle;

    private bool _slow = false;
    private bool _first = true;
    private Coroutine _coroutine;
    
    private void Start()
    {
        angle = transform.eulerAngles + new Vector3(-90, 0, 0);
    }

    IEnumerator CdColWater(Collider other)
    {
        do
        {
            InsRipple( other.ClosestPoint(transform.position));
            yield return Yielder.GetWaitForSeconds(2);
            
        } while (_cols.Contains(other) && _TruckManager.Instance.GetVehicleControl().Speed > .2f);
    }

    void Slow()
    {
        //Debug.Log("Drag on");
        if (_slow)
        {
            return;
        }
        _slow = true;
        _TruckManager.Instance.GetVehicleFunction().Slow(DragType.Water);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogError(other.gameObject.layer);
        if (other.gameObject.layer == 6)
        {
            if (other is MeshCollider)
            {
                return;
            }
            //InsRipple();
            //Debug.LogError("add" + other.name);
            if (_cols.Contains(other))
            {
                return;
            }
            _cols.Add(other);
            StartCoroutine(CdColWater(other));
            Slow();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (!_cols.Contains(other))
            {
                return;
            }
            //Debug.LogError("remove" + other.name);
            _cols.Remove(other);
            InsRipple(other.ClosestPoint(transform.position));
            if (_cols.Count <= 0)
            {
                _slow = false;
                //Debug.Log("Drag off");
                _TruckManager.Instance.GetVehicleFunction().Slow(DragType.None);
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }
                _coroutine = StartCoroutine(CDFirst());
            }
        }
    }

    IEnumerator CDFirst()
    {
        yield return Yielder.GetWaitForSeconds(2);
        if (_cols.Count <= 0)
        {
            _first = true;
        }
    }
    
    void InsRipple(Vector3 pos)
    {
        GameObject ripple;
        if (_first)
        {
            _first = false;
            ripple = ObjectPool.Instance.Get(ObjectPool.Instance.rippleBig);
        }
        else
        {
            ripple = ObjectPool.Instance.Get(ObjectPool.Instance.ripple);
        }
        ripple.transform.localScale = Vector3.one * .24f;
        ripple.transform.SetPositionAndRotation(pos + Vector3.up * .3f, Quaternion.Euler(angle));
        //SoundManager.Instance.PlayShot(SoundManager.Instance.carPuddle, .4f);
        DOVirtual.DelayedCall(1, () => ObjectPool.Instance.Return(ripple));
    }
}

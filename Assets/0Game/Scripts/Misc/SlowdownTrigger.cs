using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowdownTrigger : MonoBehaviour
{
    private bool _slow = false; 
    private List<Collider> _cols = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (other is MeshCollider)
            {
                return;
            }
            _cols.Add(other);
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
            _cols.Remove(other);
            if (_cols.Count <= 0)
            {
                _slow = false;
                _TruckManager.Instance.GetVehicleFunction().Slow(DragType.None);
            }
        }
    }

    void Slow()
    {
        if (_slow)
        {
            return;
        }
        _slow = true;
        _TruckManager.Instance.GetVehicleFunction().Slow(DragType.UpHill);
    }

}

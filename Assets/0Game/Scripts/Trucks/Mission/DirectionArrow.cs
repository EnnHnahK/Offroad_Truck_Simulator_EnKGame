using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    private Transform _target;

    public void SetTarget(Transform pos)
    {
        _target = pos;
    }
    
    void FixedUpdate()
    {
        if(_target == null) return;

        var pos = _target.position;
        pos.y = transform.position.y;
        transform.LookAt(pos);
    }
}

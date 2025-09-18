using UnityEngine;

public class FinishGate : MonoBehaviour
{
    Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            OnHitByTruck(collision.contacts[0].point);
        }
    }

    public void OnHitByTruck(Vector3 hit_position)
    {
        _rb.isKinematic = false;
        var rel = transform.position;
        rel.y = hit_position.y;
        var direction = (rel - hit_position).normalized;
        var force = direction * 10 + transform.up * 5;
        _rb.AddForceAtPosition(force, hit_position, ForceMode.Impulse);
        Destroy(this);
    }

}

using UnityEngine;

public class ItemInTruck : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Dummy") && !other.gameObject.CompareTag("Vehicle"))
        {
            _TruckManager.Instance.SetParentCache(transform);
        }
    }
}

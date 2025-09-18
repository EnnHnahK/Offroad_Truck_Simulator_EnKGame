using NWH.VehiclePhysics2;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string AnimeState;
    [SerializeField] Collider _col;
    [SerializeField] Transform _ragdoll;
    [SerializeField] AudioClip[] _hit_sfx;

    private Rigidbody[] rb_ragdoll;
    private Collider[] col_ragdoll;
    private int anim_state_id;

    private void Awake()
    {
        anim_state_id = Animator.StringToHash(AnimeState);
        rb_ragdoll = _ragdoll.GetComponentsInChildren<Rigidbody>();
        col_ragdoll = _ragdoll.GetComponentsInChildren<Collider>();
        animator?.Play(anim_state_id, 0, 0);
        foreach (var item in col_ragdoll)
        {
            item.enabled = false;
        }
        foreach (var item in rb_ragdoll)
        {
            item.isKinematic = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            var car = collision.gameObject.GetComponent<VehicleController>();
            if (!car)
                return;
            OnHitByTruck(car, collision.contacts[0].point);
        }
    }

    public void OnHitByTruck(VehicleController car, Vector3 hit_position)
    {
        HitSFX();
        var rel = transform.position;
        rel.y = hit_position.y;
        var direction = (rel - hit_position).normalized;
        var force = direction * car.Speed * 2 + transform.up * car.Speed;

        _col.enabled = false;
        animator.enabled = false;
        foreach (var item in col_ragdoll)
        {
            item.enabled = true;
        }
        foreach (var item in rb_ragdoll)
        {
            item.isKinematic = false;
            item.AddForceAtPosition(force, hit_position, ForceMode.Impulse);
        }
    }

    [Button]
    private void HitSFX()
    {
        int index = Random.Range(0, _hit_sfx.Length);
        SoundManager.Instance.PlayShot(_hit_sfx[index], 0.2f);
    }

}

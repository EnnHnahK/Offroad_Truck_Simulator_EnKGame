using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Lake : MonoBehaviour
{
    [SerializeField] private ParticleSystem parFall;
    private bool _played;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Vehicle"))
        {
            if (_played)
            {
                return;
            }
            _played = true;
            parFall.transform.position = other.ClosestPoint(other.transform.position);
            parFall.Play();
            //particle roi xuong lake
            SoundManager.Instance.PlayShot(SoundManager.Instance.fallLake);
            DOVirtual.DelayedCall(1, () => other.gameObject.SetActive(false));
        }
    }
}

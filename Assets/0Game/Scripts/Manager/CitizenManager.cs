using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenManager : MonoBehaviour
{
    public Action onCollider;
    public AudioSource audioSource;
    public AudioClip[] Conversations;

    private void Awake()
    {
        onCollider += StopConversation;
        var index = UnityEngine.Random.Range(0, Conversations.Length);
        audioSource.clip = Conversations[index];
        audioSource.Play();
    }

    private void StopConversation()
    {
        if(audioSource.isPlaying)
            audioSource.Stop();
    }
}

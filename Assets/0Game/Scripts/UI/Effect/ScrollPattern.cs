using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPattern : MonoBehaviour
{ 
    private Material _material;
    
    void Start()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }


    public float currentscroll, speed = 10f;
    void Update()
    {
        currentscroll += speed * Time.deltaTime;
        _material.mainTextureOffset = new Vector2(0, currentscroll);
    }
}

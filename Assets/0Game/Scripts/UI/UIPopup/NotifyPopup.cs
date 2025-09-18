using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotifyPopup : MonoBehaviour
{
    [SerializeField] private ButtonEffectLogic btnOk;
    
    [SerializeField] private Text titleText, desText;

    private void Awake()
    {
        btnOk.onClick.AddListener(HideNoti);
    }

    public void Init(string title, string des)
    {
        titleText.text = title;
        desText.text = des;
    }

    private void HideNoti()
    {
        gameObject.SetActive(false);
    }
}

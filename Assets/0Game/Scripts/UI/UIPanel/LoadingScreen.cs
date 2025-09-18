using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private CanvasGroup main;


    public void SetTimeLoading(float time , bool effect, Action action)
    {
        gameObject.SetActive(true);
        if (effect)
        {
            main.DOFade(1f, 0.3f).From(0.3f);
        }
        loadingSlider.DOValue(1, time).From(0).OnComplete(() =>
        {
            action?.Invoke();
            Hide();
        } );
    }
    
    private void Hide()
    {
        main.DOFade(0.7f, 0.3f).From(1f).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        main.DOKill();
        loadingSlider.DOKill();
    }
}

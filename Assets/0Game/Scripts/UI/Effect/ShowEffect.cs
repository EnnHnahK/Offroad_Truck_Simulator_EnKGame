using System;
using DG.Tweening;
using UnityEngine;

public class ShowEffect : MonoBehaviour
{
    public enum Direction
    {
        Left,
        Right,
        Down,
        Up,
    }

    [SerializeField] private RectTransform _rect;

    public Direction directionState;

    public float timeShow = .25f, effectMove = 15f; 
    private float timeAfterEffect = .1f;
    
    private Vector2 _prePos, _targetPos, _moveDelta;
    private bool _targetHadSet = false;

    //Set true if not call Effect in scripts
    public bool showActive;

    private void Awake()
    {
        _prePos = _rect.anchoredPosition;
        _targetPos = _prePos;
        _moveDelta = Vector2.zero;
    }

    private void OnEnable()
    {
        if(showActive) Effect();
    }

    [Button]
    public void Effect()
    {
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        
        if (!_targetHadSet)
        {
            _targetHadSet = true;
            switch (directionState)
            {
                case Direction.Left:
                    _targetPos -= new Vector2(_rect.sizeDelta.x + 100f, 0);
                    _moveDelta = new Vector2(effectMove, 0);
                    
                    //Full Stretch
                    if (_rect.sizeDelta == Vector2.zero) _targetPos = new Vector2(-Screen.width / 2f - 100f, 0);
                    break;
                case Direction.Right:
                    _targetPos += new Vector2(_rect.sizeDelta.x + 100f, 0);
                    _moveDelta = new Vector2(-effectMove, 0);
                    
                    //Full Stretch
                    if (_rect.sizeDelta == Vector2.zero) _targetPos = new Vector2(Screen.width / 2f + 100f, 0);
                    break;
                case Direction.Up:
                    _targetPos -= new Vector2(0, _rect.sizeDelta.y + 100f);
                    
                    //Full Stretch
                    if (_rect.sizeDelta == Vector2.zero) _targetPos = new Vector2(0, -Screen.height / 2f - 100f);
                    break;
                case Direction.Down:
                    _targetPos += new Vector2(0, _rect.sizeDelta.y + 100f);
                    
                    //Full Stretch
                    if (_rect.sizeDelta == Vector2.zero) _targetPos = new Vector2(0, Screen.height / 2f + 100f);
                    break;
            }
        }
        
        _rect.DOAnchorPos(_prePos + _moveDelta, timeShow).From(_targetPos).SetUpdate(true).OnComplete(() =>
        {
            _rect.DOAnchorPos(_prePos, timeAfterEffect).SetUpdate(true);
        });
    }
    
    public void Hide()
    {
        if(!gameObject.activeSelf) return;
        _rect.DOKill();
        _rect.DOAnchorPos(_prePos + _moveDelta, timeAfterEffect).SetUpdate(true).OnComplete(() =>
        {
            _rect.DOAnchorPos(_targetPos, timeShow).OnComplete(() =>
            {
                if(showActive) gameObject.SetActive(false);
            });
        });
    }

    private void OnDisable()
    {
        _rect.DOKill();
    }
}

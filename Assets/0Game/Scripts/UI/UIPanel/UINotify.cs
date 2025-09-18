using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UINotify : MonoBehaviour
{
    [SerializeField] private GameObject coinFly;
    [SerializeField] private Text txtCoinFly;
    [SerializeField] private List<Text> txt;
    [SerializeField] private List<GameObject> notifies;

    private Queue<Coroutine> _coroutines = new ();
    private int _index;

    [SerializeField] private Text coinEarnText;
    [SerializeField] private GameObject finish, working;
    [SerializeField] private NotifyPopup _notifyPopup;

    [SerializeField] private ShowEffect workingNotify;

    private bool _workingShowing; 

    public void CoinFly(int value)
    {
        PrefData.PlayerCoin += value;
        txtCoinFly.text = $"+{value}";
        coinFly.SetActive(true);
        DOVirtual.DelayedCall(1, () => coinFly.SetActive(false));
    }
    
    public void Notify(string text)
    {
        if (_index >= notifies.Count)
        {
            _index = 0;
        }
        txt[_index].text = text;
        var obj = notifies[_index];
        if (obj.activeInHierarchy)
        {
            obj.SetActive(false);
            StopCoroutine(_coroutines.Dequeue());
        }
        obj.SetActive(true);
        var c = StartCoroutine(WaitHideNotify(obj));
        _coroutines.Enqueue(c);
        _index++;
    }

    public void NotiFinishRace(int dis)
    {
        finish.SetActive(true);
        DOTween.To(() => 0, x => coinEarnText.text = x + "m", (int)dis, .7f).SetEase(Ease.InOutQuad);
        DOVirtual.DelayedCall(1.5f, () => finish.SetActive(false));
    }

    public void NotiPop(string title, string des)
    {
        _notifyPopup.Init(title, des);
        _notifyPopup.gameObject.SetActive(true);
    }
    
    IEnumerator WaitHideNotify(GameObject obj)
    {
        yield return new WaitForSeconds(1.5f);
        obj.SetActive(false);
        _coroutines.Dequeue();
    }
    
    public void WorkingNotify(bool show){
       if(_workingShowing == show) return;
       _workingShowing = show;
        if (show)
        {
            _TruckManager.Instance.Slow();
            working.SetActive(true);
            workingNotify.Effect();
        }
        else
        {
            workingNotify.Hide();
            DOVirtual.DelayedCall(.25f, () =>
            {
                _TruckManager.Instance.Slow();
                working.SetActive(false);
            });
        }
    }

    public bool GetWorkingStatus()
    {
        return _workingShowing;
    }
}

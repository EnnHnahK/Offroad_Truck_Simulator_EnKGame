using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DG.Tweening;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIGarage : BasePanel
{
    [Header("Navigation")]
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnSetting, btnRemoveAds, btnShop, btnChooseLevel, btnChooseVehicle, btnDailyMission;
    [SerializeField] private Text levelText;
    
    [Header("Coin Bar")]
    [SerializeField] private Text coinBarText;
    [SerializeField] private Text[] txtNotifiesCoin;
    [SerializeField] private GameObject[] notifiesCoin;
    private int _indexNotifies;
    
    [Header("Reward Vehicle")]
    [SerializeField] private Image rewardImage;
    [SerializeField] private Text countdownText , desCountDownText;
    [SerializeField] private GameObject mainReward, notifyReward;
    
    private Coroutine _rewardCoroutine;
    private DateTime _lastReward, _targetTime;
    private bool _rewardLoaded, _rewardAvailable;
    
    [Header("Upgrade Group")]
    [SerializeField] private Text[] upgradeText;
    [SerializeField] private Button btnFuelUpgrade, btnGripUpgrade, btnBonusUpgrade;
    [SerializeField] private Text fuelLevelText, fuelCoinText, gripLevelText, gripCoinText, bonusLevelText, bonusCoinText;
    [SerializeField] private GameObject[] fuelBars, gripBars, bonusBars, noMoneyUpgrade , adsUpgrade, upgradeCoinBar;
    [SerializeField] private Animator upgradeNoti, upgradeAnimator;
    
    private int _priceFuel, _priceGrip, _priceBonus, _basePrice, _maxUpgrade = 88, _indexAdsUpgrade = -1;
    public int[] upgradeLevel = new int[] {0, 0, 0};
    private bool[] _canUpgrade = new bool[3];
    private List<int> _notEnoughIndex = new ();
    
    //Anim Key
    private static readonly int ShakeFuel = Animator.StringToHash("ShakeFuel");
    private static readonly int ShakeGrip = Animator.StringToHash("ShakeGrip");
    private static readonly int ShakeBonus = Animator.StringToHash("ShakeBonus");
   
    private Queue<Coroutine> _coroutinesNotifies = new();
    
    //Cache I2 Text
    private string _maxLevelTxt, _upgradeTxt;
    
    [Header("Effect")]
    [SerializeField] private ShowEffect desPlay;
   
    [Header("Fragments Reward")]
    [SerializeField] private GameObject rewardLv;
    [SerializeField] private Text[] txtRws;

    private void OnEnable()
    {
        if (DataController.RewardVehicle <= 7)
        {
            if (_rewardCoroutine != null) StopCoroutine(_rewardCoroutine);
            if (_rewardLoaded && !_rewardAvailable) _rewardCoroutine = StartCoroutine(IERemainingTime());
        }

        canTarget = false;
        DOVirtual.DelayedCall(.5f,() => canTarget = true);
    }

    protected override void Awake()
    {
        base.Awake();
        btnFuelUpgrade.onClick.AddListener(() => { if (UpgradeCheck( 0) && canTarget) { UpgradeFuel(); } });
        btnGripUpgrade.onClick.AddListener(() => { if (UpgradeCheck(1) && canTarget) { UpgradeGrip(); } });
        btnBonusUpgrade.onClick.AddListener(() => { if (UpgradeCheck(2) && canTarget) { UpgradeBonus(); } });
        btnPlay.onClick.AddListener(() => { if (canTarget && _TruckManager.Instance) { Play(); } });
        btnSetting.onClick.AddListener(() => { ShowAfterHide(UIController.Instance.uISetting); });
        btnDailyMission.onClick.AddListener(() => { ShowAfterHide(UIController.Instance.uIDailyMission); });
        btnChooseLevel.onClick.AddListener(() =>
        {
            if (!canTarget) return;
            desPlay.Hide();
            ShowAfterHide(UIController.Instance.uIChoseLevel);
        });
        btnChooseVehicle.onClick.AddListener(() =>
        {
            if (!canTarget) return;

            if (_rewardAvailable)
            {
                _rewardAvailable = false;

                notifyReward.SetActive(false);

                RewardedVehicle();
            }

            desPlay.Hide();
            ShowAfterHide(UIController.Instance.uIChooseVehicle);
        });
        btnShop.onClick.AddListener(() =>
        {
            if (!canTarget) return;
            ShowShop();
        });
    }
    

    private void Start()
    {

        if (String.IsNullOrEmpty(_maxLevelTxt))
        {
            _maxLevelTxt = LocalizationManager.GetTranslation(Consts.maxlevel);
            _upgradeTxt = LocalizationManager.GetTranslation(Consts.upgrade);
        }

        //Load Data
        LoadData();
        rewardImage.sprite = DataController.Instance.dataVehicle.vehicles[DataController.RewardVehicle].vehicleImage;
        Utils.SetNativeImage(rewardImage, rewardImage.GetComponent<RectTransform>().sizeDelta);

        //Calculate vehicle rewards
        if (DataController.RewardVehicle <= 7)
        {
            VehicleReward();
        }
        else
        {
            mainReward.SetActive(false);
        }
    }
    
    public override void Show()
    {
        if(UIController.PopShowed) return;
        
        base.Show();
        desPlay.gameObject.SetActive(false);
        DOVirtual.DelayedCall(.7f, desPlay.Effect);
        CheckUpgrade();
        CheckFirstLv();
    }


    #region LoadData

    //Load Data
    public void LoadData()
    {
        _basePrice = DataController.CurLevel * 1000;
        _indexAdsUpgrade  = -1;
        
        //Get Data in PrefData //0 - Fuel // 1 - Grip //2 - Bonus
        upgradeLevel[0] = PrefData.GetLevelFuel(DataController.CurLevel);
        upgradeLevel[1] = PrefData.GetLevelGrip(DataController.CurLevel);
        upgradeLevel[2] = DataController.LevelBonus;
        
        //Update Text
        fuelLevelText.text = upgradeLevel[0].ToString();
        gripLevelText.text = upgradeLevel[1].ToString();
        bonusLevelText.text = upgradeLevel[2].ToString();

        //Update Price
        _priceFuel = CalculateVA(upgradeLevel[0]);
        _priceGrip = CalculateVA(upgradeLevel[1]);
        _priceBonus = CalculateVA(upgradeLevel[2]);

        for (int i = 0; i < upgradeCoinBar.Length; i++)
        {
            if (!upgradeCoinBar[i].activeSelf)
            {
                upgradeCoinBar[i].SetActive(true);
            }
            
            if(noMoneyUpgrade[i].activeSelf) noMoneyUpgrade[i].SetActive(false);
        }
        
        fuelCoinText.text = Utils.FormatNumber(_priceFuel);
        gripCoinText.text = Utils.FormatNumber(_priceGrip);
        bonusCoinText.text = Utils.FormatNumber(_priceBonus);

        upgradeText[0].text = upgradeLevel[0] == 88 ? _maxLevelTxt : _upgradeTxt;
        upgradeText[1].text = upgradeLevel[1] == 88 ? _maxLevelTxt : _upgradeTxt;
        upgradeText[2].text = upgradeLevel[2] == 88 ? _maxLevelTxt : _upgradeTxt;

        //Update Upgrade Bar
        for (int i = 0; i < 4; i++)
        {
            //Fuel
            UpgradeBar(fuelBars[i], upgradeLevel[0], i);

            //Grip
            UpgradeBar(gripBars[i], upgradeLevel[1], i);

            //Bonus
            UpgradeBar(bonusBars[i], upgradeLevel[2], i);
        }

        //CoinBar
        coinBarText.text = Utils.FormatNumber(DataController.PlayerCoin);
        levelText.text = LocalizationManager.GetTranslation(Consts.mapLevel) + (DataController.CurLevel + 1);

        //Load Completed -> Check Upgrade
        CheckUpgrade();
    }

    void UpgradeBar(GameObject obj, int level, int i)
    {
        if (level % 4 == 0)
        {
            obj.SetActive(true);
        }
        else
        {
            obj.SetActive(i < level % 4);
        }
    }
    

    #endregion

    #region Upgrade Vehicle

    private void UpgradeFuel()
    {
        //Tutorial
        if (DataController.StepTutorial == 3)
        {
            UITutorial.Instance.NextStep();
        }

        if (upgradeLevel[0] >= _maxUpgrade)
        {
            UIController.Instance.Notify(_maxLevelTxt);
            return;
        }

        SoundManager.Instance.UpgradeSound(2);
        if(_indexAdsUpgrade != 0) IncreaseCoin(-_priceFuel);
        NotiUpgrade(GetUpgradePos(0));
        upgradeLevel[0]++;
        PrefData.SetLevelFuel(DataController.CurLevel, upgradeLevel[0]);
        Events.OnUpgradeVehicle?.Invoke(0, 1);
        fuelLevelText.text = upgradeLevel[0].ToString();
        _priceFuel = CalculateVA(upgradeLevel[0]);
        fuelCoinText.text = Utils.FormatNumber(_priceFuel);

        if (upgradeLevel[0] == _maxUpgrade)
        {
            upgradeText[0].text = _maxLevelTxt;
            CompareUpgrade(0, false, ShakeFuel);
        }
        for (int i = 0; i < 4; i++)
        {
            UpgradeBar(fuelBars[i], upgradeLevel[0], i);
        }
    }

    private void UpgradeGrip()
    {
        if (DataController.StepTutorial == 4)
        {
            UITutorial.Instance.NextStep();
        }

        if (upgradeLevel[1] >= _maxUpgrade)
        {
            UIController.Instance.Notify(_maxLevelTxt);
            return;
        }

        SoundManager.Instance.UpgradeSound(1);
        if(_indexAdsUpgrade != 1)  IncreaseCoin(-_priceGrip);
        NotiUpgrade(GetUpgradePos(1));
        upgradeLevel[1]++;
        PrefData.SetLevelGrip(DataController.CurLevel, upgradeLevel[1]);
        Events.OnUpgradeVehicle?.Invoke(1, 1);
        gripLevelText.text = upgradeLevel[1].ToString();
        _priceGrip = CalculateVA(upgradeLevel[1]);
        gripCoinText.text = Utils.FormatNumber(_priceGrip);
        if (upgradeLevel[1] == _maxUpgrade)
        {
            upgradeText[1].text = _maxLevelTxt;
            CompareUpgrade(1, false, ShakeGrip);
        }

        for (int i = 0; i < 4; i++)
        {
            UpgradeBar(gripBars[i], upgradeLevel[1], i);
        }
    }

    private void UpgradeBonus()
    {
        if (DataController.StepTutorial == 5)
        {
            UITutorial.Instance.NextStep();
        }

        if (upgradeLevel[2] >= _maxUpgrade)
        {
            UIController.Instance.Notify(_maxLevelTxt);
            return;
        }

        SoundManager.Instance.UpgradeSound(2);
        if(_indexAdsUpgrade != 2)  IncreaseCoin(-_priceBonus);
        NotiUpgrade(GetUpgradePos(2));
        upgradeLevel[2] ++;
        PrefData.LevelBonus = upgradeLevel[2] ;
        bonusLevelText.text = upgradeLevel[2] .ToString();
        _priceBonus = CalculateVA(upgradeLevel[2] );
        bonusCoinText.text = Utils.FormatNumber(_priceBonus);

        if (upgradeLevel[2]  == _maxUpgrade)
        {
            upgradeText[2].text = _maxLevelTxt;
            CompareUpgrade(2, false, ShakeBonus);
        }

        for (int i = 0; i < 4; i++)
        {
            UpgradeBar(bonusBars[i], upgradeLevel[2] , i);
        }
    }

    private void NotiUpgrade(Vector3 pos)
    {
        upgradeNoti.transform.position = pos;
        upgradeNoti.Play("NotiUpgrade", 0, 0);
    }

    private int CalculateVA(int level)
    {
        return _basePrice + level * (100 + DataController.CurLevel);
    }

    private void CheckUpgrade()
    {
        var coin = DataController.PlayerCoin;
        
        CompareUpgrade(0, _priceFuel < coin, ShakeFuel);
        CompareUpgrade(1, _priceGrip < coin, ShakeGrip);
        CompareUpgrade(2, _priceBonus < coin, ShakeBonus);

        AdsUpgradeCheck();
    }

    void CompareUpgrade(int indexObj, bool enough, int keyShake)
    {
        var animActive = upgradeAnimator.gameObject.activeInHierarchy;

        if (upgradeLevel[indexObj] >= _maxUpgrade)
        {
            if (_notEnoughIndex.Contains(indexObj))
            {
                adsUpgrade[indexObj].SetActive(false);
                upgradeText[indexObj].gameObject.SetActive(true);
                _notEnoughIndex.Remove(indexObj);
            }
            upgradeAnimator.SetBool(keyShake, false);
            if(upgradeCoinBar[indexObj].activeSelf) upgradeCoinBar[indexObj].SetActive(false);
            return;
        }
        

        if (enough)
        {
            _canUpgrade[indexObj] = true;
            noMoneyUpgrade[indexObj].SetActive(false);
            if(animActive) upgradeAnimator.SetBool(keyShake, true);
            if(!upgradeCoinBar[indexObj].activeSelf) upgradeCoinBar[indexObj].SetActive(true);
            if (_notEnoughIndex.Contains(indexObj))
            {
                _notEnoughIndex.Remove(indexObj);
            }
        }
        else
        {
            _canUpgrade[indexObj] = false;
            noMoneyUpgrade[indexObj].SetActive(true);
            if (animActive) upgradeAnimator.SetBool(keyShake, false);

            if (!_notEnoughIndex.Contains(indexObj))
            {
                _notEnoughIndex.Add(indexObj);
            }
        }
    }

    private void AdsUpgradeCheck()
    {
        if (_indexAdsUpgrade != -1)
        {
            adsUpgrade[_indexAdsUpgrade].SetActive(false);
            upgradeText[_indexAdsUpgrade].gameObject.SetActive(true);
        }

        if (_notEnoughIndex.Count == 0)
        {
            //Reset Index Ads
            _indexAdsUpgrade = -1;
            return;
        }
        
        _indexAdsUpgrade =  _notEnoughIndex[Random.Range(0, _notEnoughIndex.Count)];

        adsUpgrade[_indexAdsUpgrade].SetActive(true);
        upgradeText[_indexAdsUpgrade].gameObject.SetActive(false);
        noMoneyUpgrade[_indexAdsUpgrade].SetActive(false);

        switch (_indexAdsUpgrade)
        {
            case 0: upgradeAnimator.SetBool(ShakeFuel, true); break;
            case 1: upgradeAnimator.SetBool(ShakeGrip, true); break;
            case 2: upgradeAnimator.SetBool(ShakeBonus, true); break;
        }
    }
    

    private bool UpgradeCheck(int index)
    {
        if (index == _indexAdsUpgrade)
        {
            /*AdsAdapter.instance.ShowRewardedVideo(
					 return true;
				() => UIController.Instance.Notify(Consts.ads_fail), DataController.CurLevel, 
				AdsAdapter.@where.upgrade_reward);*/
            return true;
        }

        if(!_canUpgrade[index]) ShowShop();
        return _canUpgrade[index];
    }

    #endregion

    #region Vehicle Reward

    private void VehicleReward()
    {
        if (PrefData.GetTimeReward() == "")
        {
            PrefData.SetTimeReward(DateTime.Now);
            _lastReward = DateTime.Now;
        }
        else
        {
            _lastReward = new DateTime(long.Parse(PrefData.GetTimeReward()));
        }


#if UNITY_EDITOR
        _targetTime = _lastReward.Add(TimeSpan.FromMinutes(1));
#endif

#if !UNITY_EDITOR
		_targetTime = _lastReward.Add(TimeSpan.FromHours(0.5));
#endif
        if (isActiveAndEnabled) _rewardCoroutine = StartCoroutine(IERemainingTime());
        _rewardLoaded = true;
    }

    private IEnumerator IERemainingTime()
    {
        StringBuilder sb = new StringBuilder();
        while (true)
        {
            TimeSpan remainingTime = _targetTime - DateTime.Now;

            if (remainingTime <= TimeSpan.Zero)
            {
#if UNITY_EDITOR
                Debug.Log("Reward Vehicle Available");
#endif

                _rewardAvailable = true;

                notifyReward.SetActive(true);

                desCountDownText.text = LocalizationManager.GetTranslation(Consts.vehicle_unlocked);

                countdownText.text = "";

                yield break;
            }

            sb.Clear();
            sb.AppendFormat("{0:D2}:{1:D2}:{2:D2}", (int) remainingTime.TotalHours, remainingTime.Minutes,
                remainingTime.Seconds);
            countdownText.text = sb.ToString();

            yield return Yielder.GetWaitForSeconds(1);
        }
    }

    [Button]
    public void RewardedVehicle(bool changeVehicle = false)
    {
        if (changeVehicle) PrefData.VehicleInUse = DataController.RewardVehicle;
        PrefData.SetVehicleOwned(DataController.RewardVehicle);
        UIController.Instance.uIChooseVehicle.UnlockVehicle(DataController.RewardVehicle);

        PrefData.RewardVehicleIndex++;

        desCountDownText.text = LocalizationManager.GetTranslation(Consts.new_vehicle_in);

        if (DataController.RewardVehicle >= 8)
        {
            mainReward.SetActive(false);
        }
        else
        {
            PrefData.SetTimeReward(DateTime.Now);
            rewardImage.sprite =
                DataController.Instance.dataVehicle.vehicles[DataController.RewardVehicle].vehicleImage;
            VehicleReward();
        }

        if (isActiveAndEnabled && changeVehicle) StartCoroutine(IEReward());
    }

    private IEnumerator IEReward()
    {
        yield return new WaitUntil(() => VehicleManager.Instance && LevelController.Instance);
        UIController.Instance.uIChooseVehicle.ChooseVehicle(DataController.CurVehicle, false);
    }

    #endregion

    #region Event Button

    private void ShowShop()
    {
        canTarget = false;
        desPlay.Hide();
        ShowAfterHide(UIController.Instance.uIShop);
    }

    private void Play()
    {
        canTarget = false;

        desPlay.Hide();
        DOVirtual.DelayedCall(.35f, () => ShowAfterHide(UIController.Instance.uIGamePlay));
        RaceManager.Instance.StartRace();
    }

    #endregion


    public void IncreaseCoin(int add)
    {
        if (add < 0)
        {
            NotifyCoin(Utils.FormatNumber(add));
        }
        if (add == 0)
        {
            coinBarText.text = (DataController.PlayerCoin).ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            var c = DataController.PlayerCoin;
            PrefData.PlayerCoin += add;
            if(add > 0) Events.OnEarnCoin?.Invoke(add);
            DOTween.To(() => c, x => c = x, DataController.PlayerCoin, 2f)
                .OnUpdate(() => { coinBarText.text = Utils.FormatNumber(c); }).OnComplete(() =>
                {
                    coinBarText.text = Utils.FormatNumber(DataController.PlayerCoin);
                });
            CheckUpgrade();
        }
    }

    private void NotifyCoin(string text)
    {
        if (_indexNotifies >= notifiesCoin.Length)
        {
            _indexNotifies = 0;
        }

        txtNotifiesCoin[_indexNotifies].text = text;
        var obj = notifiesCoin[_indexNotifies];
        if (obj.activeInHierarchy)
        {
            obj.SetActive(false);
            StopCoroutine(_coroutinesNotifies.Dequeue());
        }

        obj.SetActive(true);
        var c = StartCoroutine(WaitHideNotify(obj));
        _coroutinesNotifies.Enqueue(c);
        _indexNotifies++;
    }

    IEnumerator WaitHideNotify(GameObject obj)
    {
        yield return Yielder.GetWaitForSeconds(.75f);
        obj.SetActive(false);
        _coroutinesNotifies.Dequeue();
    }

    public void LocalizationUpdate()
    {
        _maxLevelTxt = LocalizationManager.GetTranslation(Consts.maxlevel);
        _upgradeTxt = LocalizationManager.GetTranslation(Consts.upgrade);

        upgradeText[0].text = upgradeLevel[0] == _maxUpgrade ? _maxLevelTxt : _upgradeTxt;
        upgradeText[1].text = upgradeLevel[1] == _maxUpgrade ? _maxLevelTxt : _upgradeTxt;
        upgradeText[2].text = upgradeLevel[2] == _maxUpgrade ? _maxLevelTxt : _upgradeTxt;
    }

    #region Get Private Variable

    public Vector2 GetUpgradePos(int index)
    {
        switch (index)
        {
            case 0: //Fuel
                return btnFuelUpgrade.transform.position;
            case 1: //Grip
                return btnGripUpgrade.transform.position;
            case 2: //Bonus
                return btnBonusUpgrade.transform.position;
        }

        return Vector2.zero;
    }

    public int GetLevelStats(int index)
    {
        return upgradeLevel[index] ;
    }

    #endregion

    #region Reward First Level
    private void CheckFirstLv()
    {
        if (PrefData.GetFirstLv(DataController.CurLevel))
        {
            rewardLv.SetActive(true);
            var rw = DataController.Instance.GetRewardLevel();
            for (int i = 0; i < rw.Length; i++)
            {
                if (rw[i] == 0)
                {
                    txtRws[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    txtRws[i].transform.parent.gameObject.SetActive(true);
                    txtRws[i].text = "x" + rw[i];
                }
            }
        }
        else
        {
            rewardLv.SetActive(false);
        }
    }

    #endregion
    
    private void OnDisable()
    {
        if (_coroutinesNotifies.Count > 0)
        {
            _coroutinesNotifies.Clear();
            foreach (var notify in notifiesCoin)
            {
                notify.SetActive(false);
            }
        }
    }
}
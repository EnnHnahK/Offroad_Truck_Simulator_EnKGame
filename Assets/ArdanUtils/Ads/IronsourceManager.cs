//using System;
//using UnityEngine;
//public class IronsourceManager : SingletonDontDestroy<IronsourceManager> {
//    const string appkey = "d9d00779";
//    Action onVideoComplete, onVideoFail;
//    bool isBackupForVd;
//    bool isVideoRewarded;
//    protected override void Awake()
//    {
//        base.Awake();

//        //For Rewarded Video
//        IronSource.Agent.init(appkey, IronSourceAdUnits.REWARDED_VIDEO);
//        //For Interstitial
//        IronSource.Agent.init(appkey, IronSourceAdUnits.INTERSTITIAL);
//        //For Offerwall
//        IronSource.Agent.init(appkey, IronSourceAdUnits.OFFERWALL);
//        //For Banners
//        IronSource.Agent.init(appkey, IronSourceAdUnits.BANNER);

//        IronSource.Agent.validateIntegration();
//        IronSource.Agent.setMetaData("AppLovin_AgeRestrictedUser", "false");
//        IronSource.Agent.setAdaptersDebug(true);
//        Listener();
//    }

//    void OnApplicationPause(bool isPaused)
//    {
//        IronSource.Agent.onApplicationPause(isPaused);
//    }

//    void Listener()
//    {
//        IronSourceEvents.onInterstitialAdClosedEvent += OnInterstitialClose;
//        IronSourceEvents.onRewardedVideoAdRewardedEvent += OnVideoRewarded;
//        IronSourceEvents.onRewardedVideoAdClosedEvent += OnVideoRewardedClose;
//        IronSourceEvents.onRewardedVideoAdShowFailedEvent += OnVideoRewardedFail;
//    }

//    private void OnVideoRewardedFail(IronSourceError obj)
//    {
//        onVideoFail?.Invoke();
//        onVideoComplete = null;
//        onVideoFail = null;
//    }

//    private void OnVideoRewardedClose()
//    {
//        if (isVideoRewarded)
//        {
//            onVideoComplete?.Invoke();
//        }
//        else
//        {
//            onVideoFail?.Invoke();
//        }
//        onVideoComplete = null;
//        onVideoFail = null;
//        isVideoRewarded = false;
//    }

//    private void OnVideoRewarded(IronSourcePlacement obj)
//    {
//        isVideoRewarded = true;
//    }

//    private void OnInterstitialClose()
//    {
//        if (isBackupForVd)
//        {
//            onVideoComplete?.Invoke();
//            onVideoComplete = null;
//            onVideoFail = null;
//        }
//        isBackupForVd = false;
//    }

//    public void ShowVideoRewarded(Action oncomplete, Action onfail, string where, int level)
//    {
//        ShowVideo(where, level);
//    }

//    public void ShowInterstitial(string where, int level)
//    {
//        ShowInter(where, level);
//    }

//    private void ShowVideo(string where, int level)
//    {
//        if (IronSource.Agent.isRewardedVideoAvailable())
//        {
//            Debug.Log("Show video");
//            IronSource.Agent.showRewardedVideo();
//            LogManager.LogAds(AdsType.Video, where, level);
//            EventManager.Instance.Push(EventKey.UpdateVideoCount);
//        }
//        else
//        {
//            isBackupForVd = true;
//            if (IronSource.Agent.isInterstitialReady())
//            {
//                Debug.Log("Show inter backup");
//                ShowInter(where, level);
//            }
//            else
//            {
//                Debug.Log("Video fail => inter fail => end");
//                onVideoFail?.Invoke();
//                onVideoComplete = null;
//                onVideoFail = null;
//            }
//        }
//    }

//    private void ShowInter(string where, int level)
//    {
//        if (IronSource.Agent.isInterstitialReady())
//        {
//            Debug.Log("Show inter");
//            IronSource.Agent.showInterstitial();
//            LogManager.LogAds(AdsType.Inter, where, level);
//            EventManager.Instance.Push(EventKey.UpdateInterCount);
//        }
//        else
//        {
//            Debug.Log("Inter fail");
//            IronSource.Agent.loadInterstitial();
//        }
//    }

//    public void ShowBanner()
//    {
//        Debug.Log("Show banner");
//        IronSource.Agent.displayBanner();
//    }

//    public void HideBanner()
//    {
//        Debug.Log("Hide banner");
//        IronSource.Agent.hideBanner();
//    }
//}

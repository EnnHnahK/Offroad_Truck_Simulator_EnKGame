//using System;
//using System.Globalization;
//using UnityEngine;
//public enum AdsType {
//    Video = 0, Inter = 1
//}
//public class AdsWhere {
//    public static string WinToNextLevel = "win_to_next_level";
//    public static string WinToHome = "win_to_home";
//    public static string WinToReplay = "win_to_replay";
//    public static string LoseToHome = "lose_to_home";
//    public static string LoseToRevive = "lose_to_revive";
//    public static string PauseToHome = "pause_to_home";
//    public static string GiftIngame = "gift_ingame";

//    public static string BuyGunShop = "buy_gun_shop";
//    public static string BuyCandyShop = "buy_candy_shop";
//    public static string BuySkinShop = "buy_skin_shop";

//    public static string BuyGunIngame = "buy_gun_ingame";
//    public static string BuyCandyIngame = "buy_candy_ingame";
//    public static string BuySkinIngame = "buy_skin_ingame";
//}

//public enum LevelPass {
//    Pass,
//    Fail,
//    Repass,
//    Refail
//}

//public enum Currency {
//    Coin, Gem, Candy
//}

//public enum CurrencySpendType {
//    Spend, Receive
//}
//public class LogManager {
//    /// <summary>
//    /// Log xem người chơi xem inter/video ở vị trí nào trong game và người chơi đang ở lv bao nhiêu
//    /// </summary>
//    public static void LogAds(AdsType adsType, string where, int level)
//    {
//        new ClientLogAds(adsType, where, level).Send();
//    }
//    /// <summary>
//    /// Log xem người chơi xem bao nhiêu inter/video trong 1 lần mở app
//    /// </summary>
//    public static void LogAdsSession(AdsType adsType, int count, int level)
//    {
//        new ClientLogAdsSession(adsType, count, level).Send();
//    }

//    /// <summary>
//    /// Log xem người chơi level này thắng/thua/thắng lại/thua lại trong bao nhiêu giây
//    /// </summary>
//    public static void LogLevel(int level, LevelPass passType, int duration)
//    {
//        new ClientLogLevel(level, passType, duration).Send();
//    }
//    /// <summary>
//    /// Log xem người chơi tiêu/nhận tiền, loại tiền gì, số lượng bao nhiêu
//    /// </summary>
//    public static void LogResource(Currency currency, CurrencySpendType spendType, int count)
//    {
//        new ClientLogResource(currency, spendType, count).Send();
//    }
//    /// <summary>
//    /// Log xem người chơi mua IAP gì, giá bao nhiêu, mua khi đang ở lv bao nhiêu
//    /// </summary>
//    public static void LogInApp(string productId, double price, int level)
//    {
//        new ClientLogIAP(productId, price, level).Send();
//    }
//}

//public abstract class ClientLogToServer { // CS
//    public void Send() { }
//}
//public class ClientLogAds : ClientLogToServer {
//    public ClientLogAds(AdsType adsType, string where, int level)
//    {
//        TrackingManager.Instance.SendInt("ads_" + adsType.ToString(), where, level);
//        Debug.Log($"[LogAds] type {adsType} | where {where} | level {level}");
//    }
//}

//public class ClientLogAdsSession : ClientLogToServer {
//    public ClientLogAdsSession(AdsType adsType, int count, int level)
//    {
//        TrackingManager.Instance.SendInt("adsSession_" + adsType.ToString(), count.ToString(), level);
//        Debug.Log($"[LogAdsSession] type {adsType} | count {count} | level {level}");
//    }
//}

//public class ClientLogLevel : ClientLogToServer {
//    public ClientLogLevel(int level, LevelPass passType, int duration)
//    {
//        TrackingManager.Instance.SendInt("level_" + level.ToString(), passType.ToString(), duration);
//        Debug.Log($"[LogLevel] level {level} | passType {passType} | duration {duration}");
//    }
//}

//public class ClientLogResource : ClientLogToServer {
//    public ClientLogResource(Currency currency, CurrencySpendType spendType, int count)
//    {
//        TrackingManager.Instance.SendInt("resource_" + currency.ToString(), spendType.ToString(), count);
//        Debug.Log($"[LogResource] currency {currency} | spendType {spendType} | count {count}");
//    }
//}

//public class ClientLogIAP : ClientLogToServer {
//    public ClientLogIAP(string productId, double price, int level)
//    {
//        TrackingManager.Instance.SendInt("iap_" + productId, price.ToString("0:0.00", CultureInfo.InvariantCulture), level);
//        Debug.Log($"[LogIAP] productId {productId} | price {price} | level {level}");
//    }
//}
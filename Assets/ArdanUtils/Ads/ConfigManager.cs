//using System;
//using UnityEngine;

//public class StringConfig {
//    public static string hlw_endDate = "hlw_endDate";
//}

//public class LongConfig {
//    public static string inter_canShow_minLevel = "inter_canShow_minLevel";
//    public static string inter_ratioShow = "inter_ratioShow";
//    public static string inter_ratioShow_lost = "inter_ratioShow_lost";
//}

//public class DoubleConfig {
//}

//public class ConfigManager : Singleton<ConfigManager> {

//    public override void Init()
//    {
//        base.Init();
//        System.Collections.Generic.Dictionary<string, object> defaults =
//            new System.Collections.Generic.Dictionary<string, object>();

//        // These are the values that are used if we haven't fetched data from the
//        // server
//        // yet, or if we ask for values that the server doesn't have:
//        defaults.Add(LongConfig.inter_canShow_minLevel, 2);
//        defaults.Add(LongConfig.inter_ratioShow, 100);
//        defaults.Add(LongConfig.inter_ratioShow_lost, 100);
//        defaults.Add(StringConfig.hlw_endDate, "11/01/2020");

//        Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
//    }

//    public string GetStringValue(string key)
//    {
//        return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).StringValue;
//    }

//    public long GetLongValue(string key)
//    {
//        return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).LongValue;
//    }

//    public double GetDoubleValue(string key)
//    {
//        return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).DoubleValue;
//    }

//    public bool GetBoolValue(string key)
//    {
//        return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).BooleanValue;
//    }
//}
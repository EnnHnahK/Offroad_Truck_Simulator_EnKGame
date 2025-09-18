//public class TrackingManager : Singleton<TrackingManager> {
//    void Example()
//    {
//        // Log an event with a float parameter
//        Firebase.Analytics.FirebaseAnalytics
//          .LogEvent("progress", "percent", 0.4f);

//        // Log an event with an int parameter.
//        Firebase.Analytics.FirebaseAnalytics
//          .LogEvent(
//            Firebase.Analytics.FirebaseAnalytics.EventPostScore,
//            Firebase.Analytics.FirebaseAnalytics.ParameterScore,
//            42
//          );

//        // Log an event with a string parameter.
//        Firebase.Analytics.FirebaseAnalytics
//          .LogEvent(
//            Firebase.Analytics.FirebaseAnalytics.EventJoinGroup,
//            Firebase.Analytics.FirebaseAnalytics.ParameterGroupId,
//            "spoon_welders"
//          );
//    }
//    public void SendString(string name, string parameterName, string parameterValue)
//    {
//        Firebase.Analytics.FirebaseAnalytics
//          .LogEvent(name, parameterName, parameterValue);
//    }

//    public void SendFloat(string name, string parameterName, float parameterValue)
//    {
//        Firebase.Analytics.FirebaseAnalytics
//          .LogEvent(name, parameterName, parameterValue);
//    }
//    public void SendInt(string name, string parameterName, int parameterValue)
//    {
//        Firebase.Analytics.FirebaseAnalytics
//          .LogEvent(name, parameterName, parameterValue);
//    }
//}
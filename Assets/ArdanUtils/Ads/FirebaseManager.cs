//using System;
//using UnityEngine;
//using Firebase;
//using System.Threading.Tasks;
//using Firebase.RemoteConfig;

//public class FirebaseManager : MonoBehaviour {
//    public static bool initComplete;
//    FirebaseApp app;
//    private void Awake()
//    {
//        InitFirebase();
//    }

//    void InitFirebase()
//    {
//        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
//            var dependencyStatus = task.Result;
//            if (dependencyStatus == DependencyStatus.Available)
//            {
//                // Create and hold a reference to your FirebaseApp,
//                // where app is a Firebase.FirebaseApp property of your application class.
//                app = FirebaseApp.DefaultInstance;
//                FetchData();
//                initComplete = true;
//                // Set a flag here to indicate whether Firebase is ready to use by your app.
//            }
//            else
//            {
//                Debug.LogError(String.Format(
//                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
//                // Firebase Unity SDK is not safe to use here.
//            }
//        });
//    }// Initialize remote config, and set the default values.
//    void InitializeFirebase()
//    {
//        FetchData();
//    }

//    // Start a fetch request.
//    public void FetchData()
//    {
//        Debug.Log("Fetching data...");
//        // FetchAsync only fetches new data if the current data is older than the provided
//        // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
//        // By default the timespan is 12 hours, and for production apps, this is a good
//        // number.  For this example though, it's set to a timespan of zero, so that
//        // changes in the console will always show up immediately.
//        Task fetchTask = FirebaseRemoteConfig.FetchAsync(TimeSpan.Zero);
//        fetchTask.ContinueWith(FetchComplete);
//    }

//    void FetchComplete(Task fetchTask)
//    {
//        if (fetchTask.IsCanceled)
//        {
//            Debug.Log("Fetch canceled.");
//        }
//        else if (fetchTask.IsFaulted)
//        {
//            Debug.Log("Fetch encountered an error.");
//        }
//        else if (fetchTask.IsCompleted)
//        {
//            Debug.Log("Fetch completed successfully!");
//        }

//        var info = FirebaseRemoteConfig.Info;

//        switch (info.LastFetchStatus)
//        {
//            case LastFetchStatus.Success:
//                FirebaseRemoteConfig.ActivateFetched();

//                Debug.Log(string.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));
//                string stop = FirebaseRemoteConfig.GetValue("stops").StringValue;
//                Debug.Log("Value: " + (string.IsNullOrEmpty(stop) ? "NA" : stop));

//                // Also tried this way, but then it doesn't enter the IF block
//                /*if (FirebaseRemoteConfig.ActivateFetched())
//                { 
//                     Debug.Log(string.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));

//                     string stop = FirebaseRemoteConfig.GetValue("stops").StringValue;
//                     Debug.Log("Value: " + (string.IsNullOrEmpty(stop) ? "NA" : stop));
//                }*/
//                break;
//            case LastFetchStatus.Failure:
//                switch (info.LastFetchFailureReason)
//                {
//                    case FetchFailureReason.Error:
//                        Debug.Log("Fetch failed for unknown reason");
//                        break;
//                    case FetchFailureReason.Throttled:
//                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
//                        break;
//                }
//                break;
//            case LastFetchStatus.Pending:
//                Debug.Log("Latest Fetch call still pending.");
//                break;
//        }
//    }
//}
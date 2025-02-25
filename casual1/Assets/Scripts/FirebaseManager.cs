using Firebase.Extensions;
using UnityEngine;

class FirebaseManager
{
	private static Firebase.FirebaseApp firebaseApp;
	private static bool init = false;

	public static void CheckInitFirebase()
	{
#if !UNITY_EDITOR
		if (!init)
		{
			try
			{
				Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
				{
					var dependencyStatus = task.Result;
					if (dependencyStatus == Firebase.DependencyStatus.Available)
					{
						// Create and hold a reference to your FirebaseApp,
						// where app is a Firebase.FirebaseApp property of your application class.
						firebaseApp = Firebase.FirebaseApp.DefaultInstance;
						init = true;

						// Set a flag here to indicate whether Firebase is ready to use by your app.
						LogManager.Log("Firebase Init Complete");

					}
					else
					{
						LogManager.LogError(System.String.Format("Firebase Could not resolve all dependencies: {0}", dependencyStatus));
						// Firebase Unity SDK is not safe to use here.
					}
				});
			}
			catch (System.Exception e)
			{
				LogManager.LogError(System.String.Format("Firebase CheckInitFirebase Error: {0}", e.Message));
			}

		}
#endif
	}
}
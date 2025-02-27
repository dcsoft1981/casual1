using DG.Tweening.Core;
using Firebase.Analytics;
using System;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ShareManager : MonoBehaviour
{
	public static ShareManager instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}


	public void DoWorkShareNormal()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("Level ").Append(LocalDataManager.instance.GetCurLevel()).Append(" here! Ready to play together ?");
		DoWorkShare(sb.ToString(), "MenuButton");
	}

	public void DoWorkShareTierUP()
	{
		GradeDBEntity entity = LocalDataManager.instance.GetCurGradeDBEntity();
		StringBuilder sb = new StringBuilder();
		sb.Append("I`d got ").Append(entity.grade).Append(" in ").Append(LocalDataManager.instance.GetTierGetTime(entity.id)).Append("\nPlay TokTok with me!!");
		DoWorkShare(sb.ToString(), "TierUP");
	}


	private void DoWorkShare(string text, string where)
	{
		string url = "www.naver.com";
#if UNITY_ANDROID
		ShareText(text);
#elif UNITY_IOS
		ShareText(text, Define.IOS_MARKET_URL, Define.ANDROID_MARKET_URL);
#else
        LogManager.Log("Share Doesnt Work!!!");
#endif

		// FirebaseLog
		if (!Define.FIREBASE_WORK)
			return;
#if !UNITY_EDITOR
		int level = LocalDataManager.instance.GetCurLevel();
		try
		{
			string key = "button_share";
			Parameter[] parameters = {
				new Parameter("button_where", where),
				new Parameter("user_level", level)
			};
			FirebaseAnalytics.LogEvent(key, parameters);
			LogManager.Log("Firebase DoWorkShare : " + key);
		}
		catch (Exception e)
		{
			LogManager.LogError("Firebase DoWorkShare : " + e.Message);
		}
#endif
	}

	// 공유할 텍스트와 URL을 받아서 공유 팝업을 호출합니다.
	public void ShareText(string text)
	{
#if UNITY_ANDROID
		StringBuilder sb = new StringBuilder();
		sb.Append(text).Append("\n")
			.Append("Google Play\n").Append(Define.ANDROID_MARKET_URL).Append("\n")
			.Append("App Store\n").Append(Define.IOS_MARKET_URL);
		ShareOnAndroid(sb.ToString());
#elif UNITY_IOS
		StringBuilder sb = new StringBuilder();
		sb.Append(text).Append("\n")
			.Append("App Store\n").Append(Define.IOS_MARKET_URL).Append("\n")
			.Append("Google Play\n").Append(Define.ANDROID_MARKET_URL);
        ShareOniOS(sb.ToString());
#else
        LogManager.Log("Share Doesnt Work!!!");
#endif
	}

#if UNITY_ANDROID
	void ShareOnAndroid(string shareMessage)
	{
		// Android의 Intent API를 호출하여 공유 팝업을 띄웁니다.
		using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
		{
			using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent"))
			{
				intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
				intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);
				intentObject.Call<AndroidJavaObject>("setType", "text/plain");

				using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
					AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "공유하기");
					currentActivity.Call("startActivity", chooser);
				}
			}
		}
	}
#endif

#if UNITY_IOS
    void ShareOniOS(string shareMessage)
    {
        // iOS 네이티브 플러그인 함수를 호출합니다.
        _ShowShareSheet(shareMessage);
    }

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void _ShowShareSheet(string message);
#endif
}
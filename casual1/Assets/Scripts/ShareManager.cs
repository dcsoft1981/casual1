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
		string str = "Let`s Play TokTok with me!!";
		DoWorkShare(str, "MenuButton");
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
		url = "https://play.google.com/store/apps/details?id=com.dcsoft1981.casual1";
#elif UNITY_IOS
        url = "https://play.google.com/store/apps/details?id=com.dcsoft1981.casual1"; // TODO ios �� �ּ� ����
#else
        Debug.Log("Share Doesnt Work!!!");
#endif
		ShareText(text, url);
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
			Debug.Log("Firebase DoWorkShare : " + key);
		}
		catch (Exception e)
		{
			Debug.LogError("Firebase DoWorkShare : " + e.Message);
		}
#endif
	}

	// ������ �ؽ�Ʈ�� URL�� �޾Ƽ� ���� �˾��� ȣ���մϴ�.
	public void ShareText(string text, string url)
	{
		string shareMessage = text + "\n" + url;

#if UNITY_ANDROID
		ShareOnAndroid(shareMessage);
#elif UNITY_IOS
        ShareOniOS(shareMessage);
#else
        Debug.Log("Share Doesnt Work!!!");
#endif
	}

#if UNITY_ANDROID
	void ShareOnAndroid(string shareMessage)
	{
		// Android�� Intent API�� ȣ���Ͽ� ���� �˾��� ���ϴ�.
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
					AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "�����ϱ�");
					currentActivity.Call("startActivity", chooser);
				}
			}
		}
	}
#endif

#if UNITY_IOS
    void ShareOniOS(string shareMessage)
    {
        // iOS ����Ƽ�� �÷����� �Լ��� ȣ���մϴ�.
        _ShowShareSheet(shareMessage);
    }

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void _ShowShareSheet(string message);
#endif
}
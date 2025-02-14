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

	public void DoWorkShare()
	{
		string text = "test test test";
		string url = "www.naver.com";
		ShareText(text, url);
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
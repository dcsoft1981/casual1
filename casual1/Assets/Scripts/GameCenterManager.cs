using UnityEngine;
using System;
using System.Threading.Tasks;
#if UNITY_IOS
using Apple.GameKit;
#elif UNITY_ANDROID

#endif

class GameCenterManager
{
	private static bool init = false;

	public static void CheckInit()
	{
		if(init)
			return;


#if UNITY_IOS
		AuthenticateGameCenter();	
#elif UNITY_ANDROID

#endif

	}

	public static void AuthenticateAndReportScore(long score)
	{
#if UNITY_IOS
		AuthenticateAndReportScoreIOS(score);
#elif UNITY_ANDROID

#endif
	}


#if UNITY_IOS
	private static async Task AuthenticateGameCenter()
    {
		try
		{
			Debug.Log($"GameCenter Init Start");
			if (!GKLocalPlayer.Local.IsAuthenticated)
			{
				var player = await GKLocalPlayer.Authenticate();
				Debug.Log($"GameCenter Init Success: {player.DisplayName}");
				init = true;
			}
			else
			{
				Debug.Log("GameCenter Init Completed");
			}
		}
		catch(Exception e)
		{
			Debug.Log($"GameCenter Init Failure: {e.Message}");
		}	
	}

	public static void AuthenticateAndReportScoreIOS(long score)
    {
		try
		{
			// Game Center Social인증
			Social.localUser.Authenticate(success =>
			{
				if (success)
				{
					Debug.Log("GameCenter SocialInit Success");
					ReportScore(score);
				}
				else
				{
					Debug.Log("GameCenter SocialInit Failure");
				}
			});
		}
		catch(Exception e)
		{
			Debug.Log($"GameCenter SocialInit Failure: {score}");
		}
    }

	public static void ReportScore(long score)
    {
		try
		{
			// 'toktok_level' 순위표에 점수 제출
			Social.ReportScore(score, "toktok_level", success =>
			{
				if (success)
				{
					Debug.Log($"GameCenter ReportScore Success: {score}");
				}
				else
				{
					Debug.Log($"GameCenter ReportScore failure: {score}");
				}
			});
		}
		catch(Exception e)
		{
			Debug.Log($"GameCenter ReportScore Failure: {score}");
		}
    }
#endif
}
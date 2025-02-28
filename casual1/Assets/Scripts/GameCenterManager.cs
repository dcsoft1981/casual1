using UnityEngine;
using System;
using System.Threading.Tasks;
#if UNITY_IOS
//using Apple.GameKit;
#elif UNITY_ANDROID

#endif

class GameCenterManager
{
	private static bool init = false;

	public static void CheckInit()
	{
		if (!Define.MARKET_ABILITY)
			return;

		if(init)
			return;


#if UNITY_IOS
		//AuthenticateGameCenter();	
#elif UNITY_ANDROID

#endif

	}

	public static void AuthenticateAndReportScore(long score)
	{
		if (!Define.MARKET_ABILITY)
			return;

#if UNITY_IOS
		//AuthenticateAndReportScoreIOS(score);
#elif UNITY_ANDROID

#endif
	}

/*
#if UNITY_IOS
	private static async Task AuthenticateGameCenter()
    {
		try
		{
			LogManager.Log($"GameCenter Init Start");
			if (!GKLocalPlayer.Local.IsAuthenticated)
			{
				var player = await GKLocalPlayer.Authenticate();
				LogManager.Log($"GameCenter Init Success: {player.DisplayName}");
				init = true;
			}
			else
			{
				LogManager.Log("GameCenter Init Completed");
			}
		}
		catch(Exception e)
		{
			LogManager.Log($"GameCenter Init Failure: {e.Message}");
		}	
	}

	private static void AuthenticateAndReportScoreIOS(long score)
    {
		try
		{
			// Game Center Social인증
			Social.localUser.Authenticate(success =>
			{
				if (success)
				{
					LogManager.Log("GameCenter SocialInit Success");
					ReportScore(score);
				}
				else
				{
					LogManager.Log("GameCenter SocialInit Failure");
				}
			});
		}
		catch(Exception e)
		{
			LogManager.Log($"GameCenter SocialInit Failure: {score}");
		}
    }

	private static void ReportScore(long score)
    {
		try
		{
			// 'toktok_level' 순위표에 점수 제출
			Social.ReportScore(score, "toktok_level", success =>
			{
				if (success)
				{
					LogManager.Log($"GameCenter ReportScore Success: {score}");
				}
				else
				{
					LogManager.Log($"GameCenter ReportScore failure: {score}");
				}
			});
		}
		catch(Exception e)
		{
			LogManager.Log($"GameCenter ReportScore Failure: {score}");
		}
    }
#endif
*/
}
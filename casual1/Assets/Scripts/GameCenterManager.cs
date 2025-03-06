using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms;
#if UNITY_IOS
using Apple.GameKit;
#elif UNITY_ANDROID

#endif

class GameCenterManager
{
	private static bool initUser = false;
	private static bool initSocial = false;

	public static bool IsIstinialized()
	{
		if (initUser && initSocial)
			return true;

		return false;
	}

	public static void CheckAuthInit()
	{
		if (!Define.MARKET_ABILITY)
			return;

		int curLevel = LocalDataManager.instance.GetCurLevel();
		if (curLevel < 3)
			return;

		if (IsIstinialized())
			return;


#if UNITY_IOS
		AuthenticateGameCenter();
#elif UNITY_ANDROID

#endif

	}

	public static void AuthenticateAndReportScore(long score)
	{
		if (!Define.MARKET_ABILITY)
			return;

		if(IsIstinialized())
		{
#if UNITY_IOS
			ReportScoreIOS(score);
#elif UNITY_ANDROID

#endif
		}
	}

	public static void ShowLeaderboardUI()
	{
		if (!Define.MARKET_ABILITY)
			return;

		if(IsIstinialized())
		{
#if UNITY_IOS
		ShowLeaderboardUIIOS();
#elif UNITY_ANDROID

#endif
		}
	}

	public static Define.RankInfo RequestAllRank()
	{
#if UNITY_EDITOR
		Define.RankInfo info = new Define.RankInfo();
		info.id = "toktok_level";
		info.name = "AllRank";
		info.score = LocalDataManager.instance.GetCurLevel();
		info.rank = 1;
		return info;
#endif

		if (!Define.MARKET_ABILITY)
			return null;

		if (IsIstinialized())
		{

#if UNITY_IOS
		return LoadUserScoreAndRankIOS();
#elif UNITY_ANDROID
			return null;
#endif
		}
	}


	// Android GPGS Work
#if UNITY_ANDROID

#endif

	// IOS GameCenter Work
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
				initUser = true;
			}
			else
			{
				LogManager.Log("GameCenter Init Aleady Completed");
			}
			SocialLoginGameCenter();
		}
		catch(Exception e)
		{
			LogManager.Log($"GameCenter Init Failure: {e.Message}");
		}	
	}

	private static async Task SocialLoginGameCenter()
	{
		if (!Social.localUser.authenticated)
		{
			try
			{
				// Game Center Social인증
				Social.localUser.Authenticate(success =>
				{
					if (success)
					{
						LogManager.Log("GameCenter SocialInit Success");
						initSocial = true;
					}
					else
					{
						LogManager.Log("GameCenter SocialInit Failure");
					}
				});
			}
			catch(Exception e)
			{
				LogManager.Log($"GameCenter SocialInit Failure");
			}
		}
	}

	private static void ReportScoreIOS(long score)
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

	private static void ShowLeaderboardUIIOS()
	{
		Social.ShowLeaderboardUI();
	}

	public static Define.RankInfo LoadUserScoreAndRankIOS()
	{
		if (Social.localUser.authenticated)
		{
			ILeaderboard leaderboard = Social.CreateLeaderboard();
			leaderboard.id = "toktok_level";
			leaderboard.userScope = UserScope.FriendsOnly; // 또는 UserScope.Global
			leaderboard.range = new UnityEngine.SocialPlatforms.Range(1, 1); // 상위 1명의 점수만 로드
			leaderboard.LoadScores(success =>
			{
				if (success && leaderboard.localUserScore != null)
				{
					long userScore = leaderboard.localUserScore.value;
					int userRank = leaderboard.localUserScore.rank;
					LogManager.Log($"GameCenter Leaderboard Score: {userScore}, Rank: {userRank}");
					// 점수와 랭킹을 UI에 표시하거나 추가 로직 수행
					Define.RankInfo info = new Define.RankInfo();
					info.id = leaderboard.id;
					info.name = "RANK";
					info.score = userScore;
					info.rank = userRank;
					return info;
				}
				else
				{
					LogManager.Log("GameCenter Leaderboard Info Not Exist");
				}
			});
		}
		else
		{
			LogManager.Log("GameCenter Leaderboard User Not authenticated");
		}

		return null;
	}
#endif
}
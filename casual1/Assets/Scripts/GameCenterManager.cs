using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms;
#if UNITY_IOS
using Apple.GameKit;
#elif UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

class GameCenterManager
{
	private static bool initUser = false;
	private static bool initSocial = false;

	private static Define.RankInfo rankInfoAll = null;
	private static bool rankAllRequesting = false;

	public static bool IsIstinialized()
	{
		if (initUser && initSocial)
			return true;

		LogManager.Log("IsIstinialized : " + initUser + " , " + initSocial);
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

		GameCenterManager.ResetRankInfo();
#if UNITY_IOS
		AuthenticateGameCenter();
#elif UNITY_ANDROID
		AuthenticateGPGS();
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
			ReportScoreAndroid(score);
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
			ShowLeaderboardUIAndroid();
#endif
		}
	}

	public static void RequestAllRank()
	{
		if (!Define.MARKET_ABILITY)
			return;

		if (IsIstinialized())
		{

#if UNITY_IOS
			LoadUserScoreAndRankIOS();
#elif UNITY_ANDROID
			LoadUserScoreAndRankAndroid();
#endif
		}
	}

	public static void ResetRankInfo()
	{
		rankInfoAll = null;
	}

	public static Define.RankInfo GetRankInfoAll()
	{
#if UNITY_EDITOR
		Define.RankInfo info = new Define.RankInfo();
		info.id = "toktok_level";
		info.name = "Total";
		info.score = LocalDataManager.instance.GetTotalClear();
		info.rank = 1;
		return info;
#endif
		return rankInfoAll;
	}

	public static void CheckAchievementStatus(List<string> ids)
	{
		if (!Define.MARKET_ABILITY)
			return;

		if (IsIstinialized())
		{

#if UNITY_IOS
			CheckAchievementStatusIOS(ids);
#elif UNITY_ANDROID
			CheckAchievementStatusAndroid(ids);
#endif
		}
	}

	public static void ShowAchieveUI()
	{
		if (!Define.MARKET_ABILITY)
			return;

		if (IsIstinialized())
		{

#if UNITY_IOS
			ShowAchieveUIIOS();
#elif UNITY_ANDROID
			ShowAchieveUIAndroid();
#endif
		}
	}



	// Android GPGS Work
#if UNITY_ANDROID
	private static async Task AuthenticateGPGS()
	{
		try
		{
			// Google Play Games 플랫폼 활성화
			PlayGamesPlatform.Activate();

			// 사용자 인증
			PlayGamesPlatform.Instance.Authenticate(success =>
			{
				if (success == SignInStatus.Success)
				{
					LogManager.Log("GPGS Auth Success");
					// 추가적인 게임 서비스 기능을 여기에 구현할 수 있습니다.
					initUser = true;
					initSocial = true;
				}
				else
				{
					LogManager.Log("GPGS Auth Failure : " + success);
					// 로그인 실패 처리 로직을 여기에 구현할 수 있습니다.
				}
			});
		}
		catch (Exception e)
		{
			LogManager.Log($"GPGS Init Failure: {e.Message}");
		}
	}

	private static void ReportScoreAndroid(long score)
	{
		try
		{
			// 'toktok_level' 순위표에 점수 제출
			PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard_total_clear, success =>
			{
				if (success)
				{
					LogManager.Log($"GPGS ReportScore Success: {score}");
				}
				else
				{
					LogManager.Log($"GPGS ReportScore failure: {score}");
				}
			});
		}
		catch (Exception e)
		{
			LogManager.Log($"GPGS ReportScore Failure: {score}");
		}
	}

	private static void ShowLeaderboardUIAndroid()
	{
		try
		{
			LogManager.Log("GPGS ShowLeaderboardUIAndroid");
			PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_total_clear);
		}
		catch(Exception e)
		{
			LogManager.LogError("GPGS ShowLeaderboardUIAndroid Failure : " + e.Message);
		}
		
	}

	public static void LoadUserScoreAndRankAndroid()
	{
		if (rankAllRequesting)
		{
			LogManager.Log("GPGS Requesting AllUserRank");
			return;
		}

		rankAllRequesting = true;
		try
		{
			PlayGamesPlatform.Instance.LoadScores(
				GPGSIds.leaderboard_total_clear,
				LeaderboardStart.TopScores,
				1,
				LeaderboardCollection.Public,
				LeaderboardTimeSpan.AllTime,
				(LeaderboardScoreData data) =>
				{
					if (data.Valid)
					{
						if (data.PlayerScore != null)
						{
							long userScore = data.PlayerScore.value;
							int userRank = data.PlayerScore.rank;
							LogManager.Log($"GPGS Leaderboard Score: {userScore}, Rank: {userRank}");
							// 점수와 랭킹을 UI에 표시하거나 추가 로직 수행
							Define.RankInfo info = new Define.RankInfo();
							info.id = GPGSIds.leaderboard_total_clear;
							info.name = "Total";
							info.score = userScore;
							info.rank = userRank;
							rankInfoAll = info;
						}
						else
						{
							LogManager.LogError("GPGS Leaderboard score Not Exist.");
						}
					}
					else
					{
						LogManager.LogError("GPGS Leaderboard load Failure.");
					}
					rankAllRequesting = false;
				});
		}
		catch (Exception ex)
		{
			LogManager.LogError($"GPGS Exception occurred while loading leaderboard scores: {ex.Message}");
			rankAllRequesting = false;
		}
	}

	public static void CheckAchievementStatusAndroid(List<string> ids)
	{
		foreach(string id in ids)
		{
			ReportAchievementAndroid(id);
		}
	}

	public static void ReportAchievementAndroid(string achievementID)
	{
		try
		{
			PlayGamesPlatform.Instance.ReportProgress(achievementID, 100, success =>
			{
				if (success)
				{
					LocalDataManager.instance.SetAchieveRecordState(achievementID);
					Debug.Log($"GPGS '{achievementID}' ReportAchievementAndroid success");
				}
				else
				{
					Debug.LogError($"GPGS '{achievementID}' ReportAchievementAndroid failure");
				}
			});
		}
		catch (Exception ex)
		{
			LogManager.LogError($"GameCenter ReportAchievementIOS occurred error: {ex.Message}");
		}
	}

	private static void ShowAchieveUIAndroid()
	{
		PlayGamesPlatform.Instance.ShowAchievementsUI();
	}
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
						LogManager.Log("GameCenter SocialInit Failure1");
					}
				});
			}
			catch(Exception e)
			{
				LogManager.Log($"GameCenter SocialInit Failure2");
			}
		}
		else
		{
			LogManager.Log("GameCenter SocialInit Aleady Success");
			initSocial = true;
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

	public static void LoadUserScoreAndRankIOS()
	{
		if (Social.localUser.authenticated)
		{
			if(rankAllRequesting)
			{
				LogManager.Log("GameCenter Requesting AllUserRank");
				return;
			}

			rankAllRequesting = true;
			ILeaderboard leaderboard = Social.CreateLeaderboard();
			leaderboard.id = "toktok_level";
			leaderboard.userScope = UserScope.Global; // 또는 UserScope.Global  
			//leaderboard.range = new UnityEngine.SocialPlatforms.Range(1, 1); // 상위 1명의 점수만 로드

			try
			{
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
						info.name = "AllUser";
						info.score = userScore;
						info.rank = userRank;
						rankInfoAll = info;
					}
					else
					{
						LogManager.Log("GameCenter Leaderboard Info Not Exist");
					}
					rankAllRequesting = false;
				});
			}
			catch(Exception ex)
			{
				LogManager.LogError($"GameCenter Exception occurred while loading leaderboard scores: {ex.Message}");
    			rankAllRequesting = false;
			}

			
		}
		else
		{
			LogManager.Log("GameCenter Leaderboard User Not authenticated");
		}
	}

	public static void CheckAchievementStatusIOS(List<string> ids)
    {
		try
		{
			Social.LoadAchievements(achievements =>
			{
				if (achievements == null)
				{
					LogManager.LogError("GameCenter achieve load failure");
					return;
				}

				foreach(string achievementID in ids)
				{
					bool isAchieved = false;
					foreach (IAchievement achievement in achievements)
					{
						if (achievement.id == achievementID && achievement.completed)
						{
							isAchieved = true;
							break;
						}
					}

					if (isAchieved)
					{
						LocalDataManager.instance.SetAchieveRecordState(achievementID);
						LogManager.Log($"GameCenter '{achievementID}'aleady cleared");
					}
					else
					{
						ReportAchievementIOS(achievementID);
					}
				}
			});
		}
		catch(Exception ex)
		{
			LogManager.LogError($"GameCenter CheckAchievementStatusIOS occurred error: {ex.Message}");
		}
    }

	public static void ReportAchievementIOS(string achievementID)
    {
		try
		{
			Social.ReportProgress(achievementID, 100, success =>
			{
				if (success)
				{
					LocalDataManager.instance.SetAchieveRecordState(achievementID);
					Debug.Log($"GameCenter '{achievementID}' ReportAchievementIOS success");
				}
				else
				{
					Debug.LogError($"GameCenter '{achievementID}' ReportAchievementIOS failure");
				}
			});
		}
		catch(Exception ex)
		{
			LogManager.LogError($"GameCenter ReportAchievementIOS occurred error: {ex.Message}");
		}
    }

	private static void ShowAchieveUIIOS()
	{
		Social.ShowAchievementsUI();
	}
#endif
}
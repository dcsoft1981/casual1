#if UNITY_ANDROID && !UNITY_EDITOR
using Google.Play.Review;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MobileMonetizationPro
{
    public class MobileMonetizationPro_UserRatingsPopUpForGame : MonoBehaviour
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
#endif

		[System.Serializable]
        public enum Options
        {
            UseAppOpenCounts,
            UseSessionCounts,
        }
        public Options LaunchChecks;

        public int LaunchCountsBeforeShowingPopup = 5; // Number of times the game should be opened before showing review popup
        private int openCount = 0;
        
        private void Awake()
        {

        }

        void Start()
        {
			if (!IsRatePopupCalled())
            {
                int curLevel = LocalDataManager.instance.GetCurLevel();
                if(curLevel >= Define.RATEPOPUP_LEVEL)
                {
					if (LaunchChecks == Options.UseAppOpenCounts)
					{
						if (PlayerPrefs.GetInt("IsAppOpened") == 0)
						{
							openCount = PlayerPrefs.GetInt("OpenCount", 0);
							openCount++;
							PlayerPrefs.SetInt("OpenCount", openCount);
							PlayerPrefs.Save();
							PlayerPrefs.SetInt("IsAppOpened", 1);
						}

					}
					else
					{
						openCount = PlayerPrefs.GetInt("OpenCount", 0);
						openCount++;
						PlayerPrefs.SetInt("OpenCount", openCount);
						PlayerPrefs.Save();
					}
				}
                else
                {
					// 51레벨 이전에는 로직을 타지 않는다.
				}


                if (openCount >= LaunchCountsBeforeShowingPopup)
                {
                    RateWork();
                }
            }
        }


#if UNITY_ANDROID && !UNITY_EDITOR
    IEnumerator RequestReviewForAndroid()
    {
        _reviewManager = new ReviewManager();

        //Request a review info object
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            LogManager.LogError("RequestReviewForAndroid1:" + requestFlowOperation.Error.ToString());
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();

        PlayerPrefs.SetInt("OpenCount", 0);
        PlayerPrefs.SetInt("IsAppOpened", 0);

        //Launch the in app review flow
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
			// Log error. For example, using requestFlowOperation.Error.ToString().
			LogManager.LogError("RequestReviewForAndroid2:" + requestFlowOperation.Error.ToString());
			yield break;
        }

        LogManager.Log("RequestReviewForAndroid Good");
		SetNoMorePopup();
		// The flow has finished. The API does not indicate whether the user
		// reviewed or not, or even whether the review dialog was shown. Thus, no
		// matter the result, we continue our app flow.
		}
#endif

        void SetNoMorePopup()
        {
			PlayerPrefs.SetInt("DoNotShowRatePopUp", 1);
			LogManager.Log("SetNoMorePopup");
		}

        void RequestReviewForiOS()
        {
#if UNITY_IOS
            PlayerPrefs.SetInt("OpenCount", 0);
            PlayerPrefs.SetInt("IsAppOpened", 0);
            UnityEngine.iOS.Device.RequestStoreReview();

            LogManager.Log("RequestReviewForiOS Good");
		    SetNoMorePopup();
#endif
		}

        public void RateWork()
        {
            LogManager.Log("DoRateWork");
            if(IsRatePopupCalled())
            {
                //마켓 링크
                string link = Define.ANDROID_MARKET_URL;
#if UNITY_IOS
                link = Define.IOS_MARKET_URL;
#endif
				Application.OpenURL(link);
			}
			else
            {
                // 마켓 평점
#if UNITY_ANDROID && !UNITY_EDITOR
                StartCoroutine(RequestReviewForAndroid());
#endif
				RequestReviewForiOS();
			}
		}

        private bool IsRatePopupCalled()
        {
            if (PlayerPrefs.GetInt("DoNotShowRatePopUp") == 0)
                return false;
            else
                return true;
		}
    }
}
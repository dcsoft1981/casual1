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

        public bool UseNativeAndroidReviewPopUp = true;
        public bool UseNativeIosReviewPopUp = true;

        public string LinkToTheGame = "";

        [System.Serializable]
        public enum Options
        {
            UseAppOpenCounts,
            UseSessionCounts,
        }
        public Options LaunchChecks;

        public int LaunchCountsBeforeShowingPopup = 5; // Number of times the game should be opened before showing review popup
        public GameObject CustomReviewPopup; // Custom review popup UI game object
        public Button CustomRateUIButton; // Rate button UI
        public Button CustomLaterUIButton; // Rate button UI

        public int StarRatingsByDefault = 5;
        public Color StarColor = Color.yellow; // Color to change the stars
        public GameObject[] CustomStars; // Array of custom stars UI game objects
        public Sprite StarSprite; // Image to be activated on star click

        private int openCount = 0;
        private Color[] originalStarColors; // Array to store the original colors of stars
        private Sprite[] originalStarSprites; // Array to store the original sprites of stars
        private bool[] starActiveStates; // Array to store the active states of stars

        private void Awake()
        {
            if (CustomStars.Length >= 1)
            {
                // Initialize originalStarColors and originalStarSprites arrays with the original colors and sprites of stars
                originalStarColors = new Color[CustomStars.Length];
                originalStarSprites = new Sprite[CustomStars.Length];
                starActiveStates = new bool[CustomStars.Length];

                for (int i = 0; i < CustomStars.Length; i++)
                {
                    originalStarColors[i] = CustomStars[i].GetComponent<Image>().color;
                    originalStarSprites[i] = CustomStars[i].GetComponent<Image>().sprite;
                    starActiveStates[i] = false;
                }

                OnStarClick(StarRatingsByDefault - 1);
            }
        }
        //private void OnApplicationPause()
        //{
        //    PlayerPrefs.SetInt("IsAppOpened", 0);
        //}
        //private void OnApplicationQuit()
        //{
        //    PlayerPrefs.SetInt("IsAppOpened", 0);
        //}
        void Start()
        {
			if (PlayerPrefs.GetInt("DoNotShowRatePopUp") == 0)
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



                if (openCount >= LaunchCountsBeforeShowingPopup)
                {
                    RateWork();

                }

                if (CustomRateUIButton != null)
                {
                    CustomRateUIButton.onClick.AddListener(OpenReviewLink);
                }
                if (CustomLaterUIButton != null)
                {
                    CustomLaterUIButton.onClick.AddListener(CloseReviewLink);
                }

                for (int i = 0; i < CustomStars.Length; i++)
                {
                    int index = i; // To capture the correct index in the lambda expression
                    CustomStars[i].GetComponent<Button>().onClick.AddListener(() => OnStarClick(index));
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
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
#endif

        void RequestReviewForiOS()
        {
#if UNITY_IOS
            PlayerPrefs.SetInt("OpenCount", 0);
            PlayerPrefs.SetInt("IsAppOpened", 0);
            UnityEngine.iOS.Device.RequestStoreReview();
#endif
        }

        void ShowReviewPopup()
        {
            // Show custom review popup
            if (CustomReviewPopup != null)
            {
                CustomReviewPopup.SetActive(true);
            }
        }

        void OpenReviewLink()
        {
            // Open the link to Google.com
            Application.OpenURL(LinkToTheGame);
            CloseReviewLink();
            PlayerPrefs.SetInt("DoNotShowRatePopUp", 1);
        }

        void CloseReviewLink()
        {
            // Show custom review popup
            if (CustomReviewPopup != null)
            {
                CustomReviewPopup.SetActive(false);
            }
            PlayerPrefs.SetInt("OpenCount", 0);
            PlayerPrefs.SetInt("IsAppOpened", 0);
        }
        void OnStarClick(int starIndex)
        {
            // Activate the images above stars up to the clicked star
            for (int i = 0; i <= starIndex; i++)
            {
                CustomStars[i].GetComponent<Image>().color = StarColor;
                if (StarSprite != null)
                {
                    CustomStars[i].GetComponent<Image>().sprite = StarSprite;
                }
                CustomStars[i].GetComponent<Image>().enabled = true;
                starActiveStates[i] = true;
            }

            // Restore previous state for stars beyond the clicked star
            for (int i = starIndex + 1; i < CustomStars.Length; i++)
            {
                CustomStars[i].GetComponent<Image>().color = originalStarColors[i];
                CustomStars[i].GetComponent<Image>().sprite = originalStarSprites[i];
                //CustomStars[i].GetComponent<Image>().enabled = starActiveStates[i];
            }
        }

        public void RateWork()
        {
			if (UseNativeAndroidReviewPopUp == true)
			{
#if UNITY_ANDROID && !UNITY_EDITOR
                    StartCoroutine(RequestReviewForAndroid());
#endif
			}
			else
			{
				ShowReviewPopup();
			}

			if (UseNativeIosReviewPopUp == true)
			{
				RequestReviewForiOS();
			}
			else
			{
				ShowReviewPopup();
			}
		}
    }
}
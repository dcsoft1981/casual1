using UnityEngine;
using TMPro;
using GoogleMobileAds.Api;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Ump.Api;
using System.Collections.Generic;
using GoogleMobileAds.Common;
using System.Collections;

namespace MobileMonetizationPro
{
    public class MobileMonetizationPro_AdmobAdsInitializer : MonoBehaviour
    {
        public static MobileMonetizationPro_AdmobAdsInitializer instance;

        public bool EnableGdprConsentMessage = true;

        //public TextMeshProUGUI T;

        [Header("Ad unit Id's")]
        public string AndroidBannerId = "";
        public string AndroidInterstitalId = "";
        public string AndroidRewardedId = "";
        public string AndroidNativeId = "";
        public string AndroidAppOpenId = "";
        public string AndroidRewardedInterstitialID = "";
        
        //public string AndroidRewardedInterstitalId = "ca-app-pub-3940256099942544/5354046379";

        public string IOSBannerId = "ca-app-pub-3940256099942544/2934735716";
        public string IOSInterstitalId = "ca-app-pub-3940256099942544/44114689102";
        public string IOSRewardedId = "ca-app-pub-3940256099942544/1712485313";
        public string IOSNativeId = "ca-app-pub-3940256099942544/3986624511";
        public string IOSAppOpenId = "ca-app-pub-3940256099942544/9257395921";
        public string IOSRewardedInterstitialID = "ca-app-pub-3940256099942544/6978759866";
        //public string IOSRewardedInterstitalId = "ca-app-pub-3940256099942544/5354046379";

        BannerView bannerView;
        InterstitialAd interstitialAd;
        RewardedAd rewardedAd = null;
        NativeAd nativeAd;

        string bannerId;
        string interId;
        string rewardedId;
        string nativeId;
        string appopenId;
        string rewardedinterstitalId;
        //string rewardedinterstitalId;

        [Header("Ads Settings")]
        public bool ShowBannerAdsInStart = true;
        public AdPosition ChooseBannerPosition = AdPosition.Bottom;
        public AdSize BannerAdSize = AdSize.Banner;
        public bool EnableTimedInterstitalAds = true;
        public int InterstitialAdIntervalSeconds = 10;

        public bool ResetInterstitalAdTimerOnRewardedAd = true;

        [Header("AppOpen Ads Settings")]
        public int AppOpensToCheckBeforeShowingAppOpenAd = 3;
        public float DelayShowAppOpenAd = 2f;

        [HideInInspector]
        public bool CanShowAdsNow = false;

        [HideInInspector]
        public float Timer = 0;

        [HideInInspector]
        public bool IsAdSkipped = false;
        [HideInInspector]
        public bool IsAdCompleted = false;
        [HideInInspector]
        public bool IsAdUnknown = false;

        MobileMonetizationPro_AdmobAdsManager AdsManagerAdmobAdsScript;

        [HideInInspector]
        public Image ImageToUseToDisplayNativeAd;

        [HideInInspector]
        public bool IsBannerStartShowing = false;

        [HideInInspector]
        public bool IsAdsInitializationCompleted = false;

        AppOpenAd AppOpenAdV;
        //private RewardedInterstitialAd _rewardedInterstitialAd;

        private int openCount = 0;

        private RewardedInterstitialAd _rewardedInterstitialAd;

		//alex 광고 20250213 배너와 보상형 광고만 설정
		private bool adState20250213 = true;
        private int rewardADRetryCount = 0;

		[Header("Alex Ads Settings")]
		public LobbyScene lobbyScene;

		private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                PlayerPrefs.SetInt("Admob_IsAppOpened", 0);
            }
            else
            {
                // If an instance already exists, destroy this duplicate
                Destroy(gameObject);
            }


#if UNITY_ANDROID
        bannerId = AndroidBannerId;
        interId = AndroidInterstitalId;
        rewardedId = AndroidRewardedId;
        nativeId = AndroidNativeId;
        appopenId = AndroidAppOpenId;
        rewardedinterstitalId = AndroidRewardedInterstitialID;
            //rewardedinterstitalId = AndroidRewardedInterstitalId;
#elif UNITY_IPHONE
            bannerId = IOSBannerId;
            interId = IOSInterstitalId;
            rewardedId = IOSRewardedId;
            nativeId = IOSNativeId;
            appopenId = IOSAppOpenId;
            rewardedinterstitalId = IOSRewardedInterstitialID;
#endif

        }
        void OnConsentInfoUpdated(FormError consentError)
        {
            if (consentError != null)
            {
                // Handle the error.
                UnityEngine.Debug.LogError(consentError);
                return;
            }

            // If the error is null, the consent information state was updated.
            // You are now ready to check if a form is available.
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
            {
                if (formError != null)
                {
                // Consent gathering failed.
                UnityEngine.Debug.LogError(consentError);
                    return;
                }

            // Consent has been gathered.
            if (ConsentInformation.CanRequestAds())
                {
                //AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
                MobileAds.RaiseAdEventsOnUnityMainThread = true;
                    MobileAds.Initialize(initStatus =>
                    {
                        IsAdsInitializationCompleted = true;
                        if (ShowBannerAdsInStart == true)
                        {
                            LoadBanner();
                        }
                        LoadAppOpenAd();
                        LoadInterstitial();
                        LoadRewarded();
                        RequestNativeAd();
                        LoadRewardedInterstitialAd();
                        if (PlayerPrefs.GetInt("Admob_IsAppOpened") == 0)
                        {
                            openCount = PlayerPrefs.GetInt("AdmobAd_AppOpenCount", 0);
                            openCount++;
                            PlayerPrefs.SetInt("AdmobAd_AppOpenCount", openCount);
                            PlayerPrefs.Save();
                            PlayerPrefs.SetInt("Admob_IsAppOpened", 1);
                        }

                        if (openCount >= AppOpensToCheckBeforeShowingAppOpenAd)
                        {
							StartCoroutine(ShowAppOpenAdWithDelay());
							PlayerPrefs.SetInt("AdmobAd_AppOpenCount", 0);
                            openCount = PlayerPrefs.GetInt("AdmobAd_AppOpenCount", 0);
                        }

                    });
                }
            });

        }
        //private void OnDestroy()
        //{
        //    AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        //}
        public void LoadAppOpenAd()
        {
            if (adState20250213)
                return;
            //// Clean up the old ad before loading a new one.
            //if (AppOpenAdV != null)
            //{
            //    AppOpenAdV.Destroy();
            //    AppOpenAdV = null;
            //}

            //Debug.Log("AlexADLoading the app open ad.");

            //// Create our request used to load the ad.
            //var adRequest = new AdRequest.Builder().Build();

            //// send the request to load the ad.
            //AppOpenAd.Load(appopenId, Screen_Orientation, adRequest,
            //    (AppOpenAd ad, LoadAdError error) =>
            //    {
            //    // if error is not null, the load request failed.
            //    if (error != null || ad == null)
            //        {
            //            Debug.LogError("AlexADErrorapp open ad failed to load an ad " +
            //                           "with error : " + error);
            //            return;
            //        }

            //        Debug.Log("AlexADApp open ad loaded with response : "
            //                  + ad.GetResponseInfo());

            //        AppOpenAdV = ad;
            //        RegisterEventHandlers(ad);
            //    });

            // Clean up the old ad before loading a new one.
            if (AppOpenAdV != null)
            {
                AppOpenAdV.Destroy();
                AppOpenAdV = null;
            }

            Debug.Log("AlexADLoading the app open ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            AppOpenAd.Load(appopenId, adRequest,
                (AppOpenAd ad, LoadAdError error) =>
                {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                    {
                        Debug.LogError("AlexADErrorapp open ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("AlexADApp open ad loaded with response : "
                              + ad.GetResponseInfo());

                    AppOpenAdV = ad;
                    RegisterEventHandlers(ad);
                });
        }
        //private void OnAppStateChanged(AppState state)
        //{
        //    Debug.Log("AlexADApp State changed to : " + state);

        //    // if the app is Foregrounded and the ad is available, show it.
        //    if (state == AppState.Foreground)
        //    {
        //        StartCoroutine(ShowAppOpenAdWithDelay());
        //    }
        //}
        IEnumerator ShowAppOpenAdWithDelay()
        {
            yield return new WaitForSeconds(DelayShowAppOpenAd);

			if (adState20250213)
			{
				//alex 광고 20250213 배너와 보상형 광고만 설정
			}
			else
			{
				ShowAppOpenAd();
			}
        }

        public void ShowAppOpenAd()
        {
            if (AppOpenAdV != null && AppOpenAdV.CanShowAd())
            {
                Debug.Log("AlexADShowing app open ad.");
                AppOpenAdV.Show();
            }
            else
            {
                Debug.LogError("AlexADErrorApp open ad is not ready yet.");
            }
        }
        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("App open ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("AlexADApp open ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("AlexADApp open ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("AlexADApp open ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("AlexADApp open ad full screen content closed.");
                LoadAppOpenAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("AlexADErrorApp open ad failed to open full screen content " +
                               "with error : " + error);
                LoadAppOpenAd();

            };
        }
        private void OnEnable()
        {
            if (EnableGdprConsentMessage == true)
            {
                // Create a ConsentRequestParameters object.
                ConsentRequestParameters request = new ConsentRequestParameters();

                // Check the current consent information status.
                ConsentInformation.Update(request, OnConsentInfoUpdated);
            }
            else
            {

                MobileAds.RaiseAdEventsOnUnityMainThread = true;
                MobileAds.Initialize(initStatus =>
                {
                    IsAdsInitializationCompleted = true;
                    if (ShowBannerAdsInStart == true)
                    {
                        LoadBanner();
                    }

					LoadAppOpenAd();
					LoadInterstitial();
					LoadRewarded();
					RequestNativeAd();
					LoadRewardedInterstitialAd();
					if (PlayerPrefs.GetInt("Admob_IsAppOpened") == 0)
                    {
                        openCount = PlayerPrefs.GetInt("AdmobAd_AppOpenCount", 0);
                        openCount++;
                        PlayerPrefs.SetInt("AdmobAd_AppOpenCount", openCount);
                        PlayerPrefs.Save();
                        PlayerPrefs.SetInt("Admob_IsAppOpened", 1);
                    }

                    if (openCount >= AppOpensToCheckBeforeShowingAppOpenAd)
                    {
                        StartCoroutine(ShowAppOpenAdWithDelay());
                        PlayerPrefs.SetInt("AdmobAd_AppOpenCount", 0);
                        openCount = PlayerPrefs.GetInt("AdmobAd_AppOpenCount", 0);
                    }

                });
            }
        }
        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            //T.text = GetDeviceID();
		}
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (IsAdsInitializationCompleted == true)
            {
                if (IsBannerStartShowing == true || ShowBannerAdsInStart == true)
                {
                    LoadBanner();
                }

				LoadInterstitial();
				LoadRewarded();
				RequestNativeAd();
				LoadRewardedInterstitialAd();
				if (PlayerPrefs.GetInt("Admob_IsAppOpened") == 0)
                {
                    openCount = PlayerPrefs.GetInt("AdmobAd_AppOpenCount", 0);
                    openCount++;
                    PlayerPrefs.SetInt("AdmobAd_AppOpenCount", openCount);
                    PlayerPrefs.Save();
                    PlayerPrefs.SetInt("Admob_IsAppOpened", 1);
                }

                if (openCount >= AppOpensToCheckBeforeShowingAppOpenAd)
                {
                    StartCoroutine(ShowAppOpenAdWithDelay());
                    PlayerPrefs.SetInt("AdmobAd_AppOpenCount", 0);
                    openCount = PlayerPrefs.GetInt("AdmobAd_AppOpenCount", 0);
                }
            }

        }
        private void Update()
        {
            if (PlayerPrefs.GetInt("AdsRemovedSuccessfully") == 0)
            {
                if (Timer >= InterstitialAdIntervalSeconds)
                {
                    Timer = 0;
                    CanShowAdsNow = true;
                }
                else
                {
                    if (EnableTimedInterstitalAds == true)
                    {
                        Timer += Time.deltaTime;
                        if (PlayerPrefs.GetInt("AdsRemoved") == 1)
                        {
                            if (PlayerPrefs.GetInt("AdsRemovedSuccessfully") == 0)
                            {
                                DestroyBannerAd();
                                PlayerPrefs.SetInt("AdsRemovedSuccessfully", 1);
                            }
                        }
                    }
                }
            }
        }
        public void CheckForAdmobManagerScript()
        {
            if (AdsManagerAdmobAdsScript == null)
            {
                if (FindFirstObjectByType<MobileMonetizationPro_AdmobAdsManager>() != null)
                {
                    AdsManagerAdmobAdsScript = FindFirstObjectByType<MobileMonetizationPro_AdmobAdsManager>();
                }
                if (AdsManagerAdmobAdsScript != null)
                {
                    AdsManagerAdmobAdsScript.CheckForAdCompletion();
                }
            }
            else
            {
                AdsManagerAdmobAdsScript.CheckForAdCompletion();
            }
        }

        #region Banner

        public void LoadBanner()
        {
            // Previous Code If you want to use than uncomment from line 443 to 463
            //if (PlayerPrefs.GetInt("AdsRemoved") == 0 && IsAdsInitializationCompleted == true)
            //{
            //    //create a banner
            //    CreateBannerView();

            //    //listen to banner events
            //    ListenToBannerEvents();

            //    //load the banner
            //    if (bannerView == null)
            //    {
            //        CreateBannerView();
            //    }

            //    var adRequest = new AdRequest();
            //    adRequest.Keywords.Add("unity-admob-sample");

            //    Debug.Log("AlexADLoading banner Ad !!");
            //    bannerView.LoadAd(adRequest);//show the banner on the screen
            //    IsBannerStartShowing = true;
            //}

            // New Code If you want to use previous code than comment from line 466 to 493
            int curLevel = LocalDataManager.instance.GetCurLevel();
            if (curLevel < Define.STABLEUSER_LEVEL)
                return;

            if (PlayerPrefs.GetInt("AdsRemoved") == 0 && IsAdsInitializationCompleted == true)
            {
                if (bannerView == null)
                {
                    //create a banner
                    CreateBannerView();

                    //listen to banner events
                    ListenToBannerEvents();

                    //load the banner
                    if (bannerView == null)
                    {
                        CreateBannerView();
                    }

                    var adRequest = new AdRequest();
                    adRequest.Keywords.Add("unity-admob-sample");

                    Debug.Log("AlexADLoading banner Ad !!");
                    bannerView.LoadAd(adRequest);//show the banner on the screen
                    IsBannerStartShowing = true;
                }
                else
                {
                    bannerView.Show();
                }
            }
        }

        void CreateBannerView()
        {
            if (PlayerPrefs.GetInt("AdsRemoved") == 0)
            {
                if (bannerView != null)
                {
                    DestroyBannerAd();
                }
                bannerView = new BannerView(bannerId, BannerAdSize, ChooseBannerPosition);
            }
        }
        void ListenToBannerEvents()
        {
            if (PlayerPrefs.GetInt("AdsRemoved") == 0)
            {
                bannerView.OnBannerAdLoaded += () =>
            {
                Debug.Log("AlexADBanner view loaded an ad with response : "
                    + bannerView.GetResponseInfo());
            };
                // Raised when an ad fails to load into the banner view.
                bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
                {
                    Debug.LogError("AlexADErrorBanner view failed to load an ad with error : "
                        + error);
                };
                // Raised when the ad is estimated to have earned money.
                bannerView.OnAdPaid += (AdValue adValue) =>
                {
                    Debug.Log("AlexADBanner view paid {0} {1}." +
                        adValue.Value +
                        adValue.CurrencyCode);
                };
                // Raised when an impression is recorded for an ad.
                bannerView.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("AlexADBanner view recorded an impression.");
                };
                // Raised when a click is recorded for an ad.
                bannerView.OnAdClicked += () =>
                {
                    Debug.Log("AlexADBanner view was clicked.");
                };
                // Raised when an ad opened full screen content.
                bannerView.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log("AlexADBanner view full screen content opened.");
                };
                // Raised when the ad closed full screen content.
                bannerView.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("AlexADBanner view full screen content closed.");
                };
            }
        }
        public void DestroyBannerAd()
        {
            if (bannerView != null)
            {
                Debug.Log("AlexADDestroying banner Ad");
                bannerView.Destroy();
                bannerView = null;
            }
        }
        #endregion

        #region Interstitial

        public void LoadInterstitial()
        {
			if (adState20250213)
				return;

			if (PlayerPrefs.GetInt("AdsRemoved") == 0 && IsAdsInitializationCompleted == true)
            {
                if (interstitialAd != null)
                {
                    interstitialAd.Destroy();
                    interstitialAd = null;
                }
                var adRequest = new AdRequest();
                adRequest.Keywords.Add("unity-admob-sample");

                InterstitialAd.Load(interId, adRequest, (InterstitialAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.Log("AlexADInterstitial ad failed to load" + error);
                        return;
                    }

                    Debug.Log("AlexADInterstitial ad loaded !!" + ad.GetResponseInfo());

                    interstitialAd = ad;
                    InterstitialEvent(interstitialAd);
                });
            }
        }
        public void ShowInterstitialAd()
        {
            if (PlayerPrefs.GetInt("AdsRemoved") == 0)
            {
                if (interstitialAd != null && interstitialAd.CanShowAd())
                {
                    interstitialAd.Show();
                }
                else
                {
                    Debug.Log("AlexADIntersititial ad not ready!!");
                }
            }
        }
        public void InterstitialEvent(InterstitialAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log("AlexADInterstitial ad paid {0} {1}." +
                    adValue.Value +
                    adValue.CurrencyCode);
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("AlexADInterstitial ad recorded an impression.");
                IsAdSkipped = true;
                CheckForAdmobManagerScript();

            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("AlexADInterstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("AlexADInterstitial ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("AlexADInterstitial ad full screen content closed.");
                IsAdSkipped = true;
                CheckForAdmobManagerScript();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("AlexADErrorInterstitial ad failed to open full screen content " +
                               "with error : " + error);
            };
        }

        #endregion

        #region Rewarded

        public void DestroyRewardedAd()
        {
			if (rewardedAd != null)
			{
				rewardedAd.Destroy();
				rewardedAd = null;
				Debug.Log("AlexADRewarded Destroied");
			}
		}

        public void LoadRewarded()
        {
            if (IsAdsInitializationCompleted == true)
            {
                if (rewardedAd != null && rewardedAd.CanShowAd())
                {
					// 이미 광고 있음
					// 기존 광고 시청 후 초기화
					Debug.Log("AlexADRewarded ad Exist");
					return;
                }

                DestroyRewardedAd();

				var adRequest = new AdRequest();
                adRequest.Keywords.Add("unity-admob-sample");

                RewardedAd.Load(rewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.Log("AlexADRewarded failed to load");
                        if(error != null)
                        {
							Debug.Log("AlexADRewarded failed to load error: " + error.ToString());
						}
						if (ad != null)
						{
							Debug.Log("AlexADRewarded failed to load ad: " + ad.ToString());
						}
						SwapAdBtnToPlayBtn();
                        return;
                    }

                    Debug.Log("AlexADRewarded ad loaded !!");
                    rewardedAd = ad;
                    RewardedAdEvents(rewardedAd);
                });
            }
        }
        public void ShowRewardedAd()
        {
			if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardADRetryCount = 0;
				rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log("AlexADShowRewardedAd Good");
                });
            }
            else
            {
                rewardADRetryCount++;
				Debug.Log("AlexADRewarded ad not ready : " + rewardADRetryCount);
                if(rewardedAd != null)
                {
					Debug.Log("AlexADRewarded ad not ready RewardedAd: " + rewardedAd.CanShowAd() + " AD:" + rewardedAd.ToString());
				}
                if(rewardADRetryCount > 2)
                {
                    rewardADRetryCount = 0;
					SwapAdBtnToPlayBtn();
				}
                else
                {
					// 1회 더 시도: 0.5초 딜레이 후 재시도
					StartCoroutine(DelayAndRetryShowRewardedAd());
				}
			}
        }

		private IEnumerator DelayAndRetryShowRewardedAd()
		{
			yield return new WaitForSeconds(0.5f);  // 0.5초 대기
			ShowRewardedAd();  // 재시도
		}
		
        public void RewardedAdEvents(RewardedAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log("AlexADRewarded ad paid {0} {1}." +
                    adValue.Value +
                    adValue.CurrencyCode);
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("AlexADRewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("AlexADRewarded ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("AlexADRewarded ad full screen content opened.");
				AudioManager.instance.TickTockStop();
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("AlexADRewarded ad full screen content closed.");
				DestroyRewardedAd();
				IsAdCompleted = true;
                CheckForAdmobManagerScript();
				AudioManager.instance.TickTockPlay();
			};
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("AlexADErrorRewarded ad failed to open full screen content " +
                               "with error : " + error);
            };
        }

        #endregion


        #region Native

        public void RequestNativeAd()
        {
			if (adState20250213)
				return;
			
            if (PlayerPrefs.GetInt("AdsRemoved") == 0)
            {
                AdLoader adLoader = new AdLoader.Builder(nativeId).ForNativeAd().Build();

                adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
                adLoader.OnAdFailedToLoad += this.HandleNativeAdFailedToLoad;

                adLoader.LoadAd(new AdRequest());
            }
        }

        private void HandleNativeAdLoaded(object sender, NativeAdEventArgs e)
        {
            if (PlayerPrefs.GetInt("AdsRemoved") == 0)
            {
                Debug.Log("AlexADNative ad loaded");
                this.nativeAd = e.nativeAd;

                Texture2D iconTexture = this.nativeAd.GetIconTexture();
                Sprite sprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), Vector2.one * .5f);

                ImageToUseToDisplayNativeAd.sprite = sprite;
            }

        }

        private void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Debug.Log("AlexADNative ad failed to load" + e.ToString());

        }
        #endregion

        #region RewardedInterstital
        public void LoadRewardedInterstitialAd()
        {
			if (adState20250213)
				return;
			
            // Clean up the old ad before loading a new one.
			if (_rewardedInterstitialAd != null)
            {
                _rewardedInterstitialAd.Destroy();
                _rewardedInterstitialAd = null;
            }

            Debug.Log("AlexADLoading the rewarded interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            // send the request to load the ad.
            RewardedInterstitialAd.Load(rewardedinterstitalId, adRequest,
                (RewardedInterstitialAd ad, LoadAdError error) =>
                {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                    {
                        Debug.LogError("AlexADErrorrewarded interstitial ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("AlexADRewarded interstitial ad loaded with response : "
                              + ad.GetResponseInfo());

                    _rewardedInterstitialAd = ad;

                    // Register to ad events to extend functionality.
                    RegisterEventHandlers(ad);
                });
        }
        public void ShowRewardedInterstitialAd()
        {
            const string rewardMsg =
                "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

            if (_rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd())
            {
                _rewardedInterstitialAd.Show((Reward reward) =>
                {
                    // TODO: Reward the user.
                    Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                });
            }
        }
        private void RegisterEventHandlers(RewardedInterstitialAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Rewarded interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("AlexADRewarded interstitial ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("AlexADRewarded interstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("AlexADRewarded interstitial ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                IsAdCompleted = true;
                CheckForAdmobManagerScript();
                Debug.Log("AlexADRewarded interstitial ad full screen content closed.");
                LoadRewardedInterstitialAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("AlexADErrorRewarded interstitial ad failed to open " +
                               "full screen content with error : " + error);
                LoadRewardedInterstitialAd();
            };
        }
        #endregion
        //public static string GetDeviceID()
        //{
        //    //Get Android ID
        //    AndroidJavaClass clsunity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //    AndroidJavaObject objActivity = clsunity.GetStatic<AndroidJavaObject>("currentActivity");
        //    AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
        //    AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");

        //    string android_id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");

        //    // Get bytes of Android ID
        //    System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        //    byte[] bytes = ue.GetBytes(android_id);

        //    // Encrypt bytes with md5
        //    System.Security.Cryptography.MD5CryptoServiceProvider mD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        //    byte[] hashbytes = mD5.ComputeHash(bytes);

        //    // Convert the encrypted bytes back to a string (base 16)
        //    string hashString = "";

        //    for (int i = 0; i < hashbytes.Length; i++)
        //    {
        //        hashString += System.Convert.ToString(hashbytes[i], 16).PadLeft(2, '0');
        //    }
        //    string device_id = hashString.PadLeft(32, '0');
        //    return device_id;
        //}
        public void OpenInspector()
        {
            MobileAds.OpenAdInspector(error =>
            {
                // Error will be set if there was an issue and the inspector was not displayed.
            });
        }
		//public void LoadRewardedInterstitialAd()
		//{
		//    // Clean up the old ad before loading a new one.
		//    if (_rewardedInterstitialAd != null)
		//    {
		//        _rewardedInterstitialAd.Destroy();
		//        _rewardedInterstitialAd = null;
		//    }

		//    Debug.Log("AlexADLoading the rewarded interstitial ad.");

		//    // create our request used to load the ad.
		//    var adRequest = new AdRequest();
		//    adRequest.Keywords.Add("unity-admob-sample");

		//    // send the request to load the ad.
		//    RewardedInterstitialAd.Load(rewardedinterstitalId, adRequest,
		//        (RewardedInterstitialAd ad, LoadAdError error) =>
		//        {
		//          // if error is not null, the load request failed.
		//          if (error != null || ad == null)
		//            {
		//                Debug.LogError("AlexADErrorrewarded interstitial ad failed to load an ad " +
		//                               "with error : " + error);
		//                return;
		//            }

		//            Debug.Log("AlexADRewarded interstitial ad loaded with response : "
		//                      + ad.GetResponseInfo());

		//            _rewardedInterstitialAd = ad;
		//        });
		//}
		//public void ShowRewardedInterstitialAd()
		//{
		//    const string rewardMsg =
		//        "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

		//    if (_rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd())
		//    {
		//        _rewardedInterstitialAd.Show((Reward reward) =>
		//        {
		//            // TODO: Reward the user.
		//            Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
		//            _rewardedInterstitialAd.Destroy();
		//        });
		//    }
		//}
		//private void RegisterEventHandlers(RewardedInterstitialAd ad)
		//{
		//    // Raised when the ad is estimated to have earned money.
		//    ad.OnAdPaid += (AdValue adValue) =>
		//    {
		//        Debug.Log(String.Format("Rewarded interstitial ad paid {0} {1}.",
		//            adValue.Value,
		//            adValue.CurrencyCode));
		//    };
		//    // Raised when an impression is recorded for an ad.
		//    ad.OnAdImpressionRecorded += () =>
		//    {
		//        Debug.Log("AlexADRewarded interstitial ad recorded an impression.");
		//    };
		//    // Raised when a click is recorded for an ad.
		//    ad.OnAdClicked += () =>
		//    {
		//        Debug.Log("AlexADRewarded interstitial ad was clicked.");
		//    };
		//    // Raised when an ad opened full screen content.
		//    ad.OnAdFullScreenContentOpened += () =>
		//    {
		//        Debug.Log("AlexADRewarded interstitial ad full screen content opened.");
		//    };
		//    // Raised when the ad closed full screen content.
		//    ad.OnAdFullScreenContentClosed += () =>
		//    {
		//        Debug.Log("AlexADRewarded interstitial ad full screen content closed.");
		//        LoadRewardedInterstitialAd();

		//    };
		//    // Raised when the ad failed to open full screen content.
		//    ad.OnAdFullScreenContentFailed += (AdError error) =>
		//    {
		//        Debug.LogError("AlexADErrorRewarded interstitial ad failed to open " +
		//                       "full screen content with error : " + error);
		//        LoadRewardedInterstitialAd();
		//    };
		//}

		public void SwapAdBtnToPlayBtn()
		{
			lobbyScene = FindFirstObjectByType<LobbyScene>();
			if (lobbyScene == null)
			{
				Debug.Log("AlexAD SwapAdBtnToPlayBtn cannot find LobbyScene.");
			}
            else
            {
				lobbyScene.SwapAdBtnToPlayBtn();
			}
		}

        public void OnClearADWork()
        {
			lobbyScene = FindFirstObjectByType<LobbyScene>();
			if (lobbyScene == null)
			{
				Debug.Log("AlexAD OnClearADWork cannot find LobbyScene.");
			}
			else
			{
				lobbyScene.OnClearADWork();
			}
		}
	}
}
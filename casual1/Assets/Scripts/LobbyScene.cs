using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;
using static Define;
using System.Text;
using UnityEngine.Scripting;
using System.Configuration;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.Networking;

[Preserve]
public class LobbyScene : MonoBehaviour
{
	[SerializeField] private GameObject canvas;
	[SerializeField] private GameObject btnPlay;
	[SerializeField] private GameObject btnTempAD;
	[SerializeField] private GameObject btnPlayer;
	[SerializeField] private GameObject btnMenu;
	[SerializeField] private GameObject popupPlayer;
	[SerializeField] private GameObject popupMenu;
	[SerializeField] private GameObject popupCheat;
	[SerializeField] private TextMeshProUGUI textTitle1;

	[SerializeField] private GameObject gradeScroll;
	[SerializeField] private GameObject gradeCurrent;
	[SerializeField] private GameObject gradeEtc;
	[SerializeField] private GameObject scrollbar;

	[SerializeField] private GameObject gimmickScroll;
	[SerializeField] private GameObject gimmickPrefab;

	[SerializeField] private ScrollRect gradeScrollRect;


	private Image buttonPlayImage;
	private Image buttonADImage;
	private Image buttonPlayerImage;
	private Image buttonMenuImage;
	private List<string> titlePrefix;

	public float scrollBarValue = 0;

	public Toggle soundOntoggle;
	public Toggle vibrateOntoggle;
	public Toggle guidelineOntoggle;
	private Sprite spriteInfinity;

	[SerializeField] private Camera cam;

	private float time;
	[SerializeField] private Color topColor = Color.blue;    // 그라데이션의 상단 색상
	[SerializeField] private Color bottomColor = Color.green; // 그라데이션의 하단 색상
	public float duration = 5.0f;           // 그라데이션 전환에 걸리는 시간

	[SerializeField] private GameObject marks;
	[SerializeField] private GameObject markPrefab;
	[SerializeField] private GameObject btnCheat;
	private List<Mark> listMark = new List<Mark>();

	[SerializeField] private TextMeshProUGUI textVersion;

	public RawImage rawImage;
	public VideoPlayer videoPlayer;
	public float videoWidth = 1200f;
	public float videoHeight = 1920f;

	[SerializeField] private GameObject btnAllRank;

	private void Awake()
	{
		// https://docs.febucci.com/text-animator-unity/effects/built-in-effects-list
		buttonPlayImage = btnPlay.transform.Find("Image").GetComponent<Image>();
		buttonADImage = btnTempAD.transform.Find("Image").GetComponent<Image>();
		buttonPlayerImage = btnPlayer.GetComponent<Image>();
		buttonMenuImage = btnMenu.GetComponent<Image>();
		titlePrefix = new List<string>();
		titlePrefix.Add("<rainb f=3>");
		titlePrefix.Add("<pend>");
		titlePrefix.Add("<dangle>");
		titlePrefix.Add("<fade d=1>");
		titlePrefix.Add("<rot>");
		titlePrefix.Add("<bounce a=3>");
		titlePrefix.Add("<slide>");
		titlePrefix.Add("<swing>");
		titlePrefix.Add("<wave>");
		titlePrefix.Add("<incr a=0.9 f=3>");
		titlePrefix.Add("<shake a=3>");
		titlePrefix.Add("<wiggle>");
		titlePrefix.Add("");

		spriteInfinity = Resources.Load<Sprite>("Infinity");
	}

	void Start()
    {
		int level = 0;
		TextMeshProUGUI buttonText = btnPlay.GetComponentInChildren<TextMeshProUGUI>();
		if (buttonText != null)
		{
			level = LocalDataManager.instance.GetCurLevel();
			buttonText.text = level.ToString();
		}
		if (level == 1)
		{
			canvas.SetActive(false);
		}
		Color curGradeColor = LocalDataManager.instance.GetCurColor();
		buttonPlayImage.color = curGradeColor;
		buttonADImage.color = curGradeColor;
		buttonPlayerImage.color = curGradeColor;
		buttonMenuImage.color = curGradeColor;
		IngameType ingameType = LocalDataManager.instance.GetIngameType();
		if(ingameType == IngameType.INFINITY)
		{
			buttonPlayImage.sprite = spriteInfinity;
			if(buttonText != null)
				buttonText.gameObject.SetActive(false);
		}
		SetTitleText();
		SetGradeScrollInfo();
		SetGimmickScrollInfo();
		SetMarksInfo();
		SetReddot();
		SetCheat();
		SetClientVersion();
		AudioManager.instance.TickTockPlay();

		time = 0.0f;

		// firebase 초기화
		FirebaseManager.CheckInitFirebase();

		if (level == 1)
		{
			// Sfx 끄기
			AudioManager.instance.TickTockStop();
			// 플레이 버튼 레드닷 활성화
			LocalDataManager.instance.SetReddotPlay(true);
			// RawImage 활성화
			rawImage.gameObject.SetActive(true);
			StartCoroutine(PlayVideoWithFade());
		}
		else
		{
			if(LocalDataManager.instance.ShowAD())
			{
				// 광고 제생(네트워크 환경, 광고 불러옴 체크)
				bool ableShowAD = true;

				if(ableShowAD)
				{
					// 공고 재생 후 플레이 버튼 활성화
					// 임시로 광고 팝업 활성화
					btnTempAD.SetActive(true);
				}
				else
				{
					// 광고 제생 불가 상황에서는 플레이 버튼 활성화
					ActiveBtnPlay();
				}
			}
			else
			{
				ActiveBtnPlay();
			}

			InvokeRepeating("CheckGameCenterInit", 0f, 3f);
			InvokeRepeating("ReqRankInfo", 0.5f, 1f);
			InvokeRepeating("ShowRankInfo", 0.8f, 1f);
		}
	}

	void Update()
	{
		time += Time.deltaTime;
		float t = Mathf.PingPong(time / duration, 1.0f);
		cam.backgroundColor = Color.Lerp(bottomColor, topColor, t);
	}

	private void LoadIngameScene()
	{
		StopMarksJump();
		SceneManager.LoadScene("IngameScene");
	}

	private void LoadLevelSceneForced()
	{
		LoadIngameScene();
	}

	public void LoadLevelScene()
    {
		if (LocalDataManager.instance.GetReddotPlay())
		{
			LocalDataManager.instance.SetReddotPlay(false);
			btnPlay.transform.Find(Define.REDDOT).gameObject.SetActive(false);
		}
		LoadIngameScene();
	}

	public void OnClickShare()
	{
		popupPlayer.SetActive(true);
		SetGradeScrollValue();
		if (LocalDataManager.instance.GetReddotPlayer())
		{
			LocalDataManager.instance.SetReddotPlayer(false);
			btnPlayer.transform.Find(Define.REDDOT).gameObject.SetActive(false);
			StopTweenAni(btnPlayer.transform);
		}
		//popupPlayer.transform.localScale = Vector3.zero;
		//popupPlayer.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCirc);
	}

	public void OnCloseShare()
	{
		popupPlayer.SetActive(false);
		//popupPlayer.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => popupPlayer.SetActive(false));
	}

	public void OnClickMenu()
	{
		popupMenu.SetActive(true);
		if (LocalDataManager.instance.GetReddotMenu())
		{
			LocalDataManager.instance.SetReddotMenu(false);
			btnMenu.transform.Find(Define.REDDOT).gameObject.SetActive(false);
			StopTweenAni(btnMenu.transform);
		}
		SetSetting();
		//popupMenu.transform.localScale = Vector3.zero;
		//popupMenu.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCirc);
	}

	public void OnCloseMenu()
	{
		popupMenu.SetActive(false);
		//popupMenu.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => popupMenu.SetActive(false));
	}

	public void OnClickCheat()
	{
		if (!LocalDataManager.instance.GetCheatStage())
			return;
		popupCheat.SetActive(true);
		popupCheat.transform.localScale = Vector3.zero;
		popupCheat.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCirc);
	}

	public void OnCloseCheat()
	{
		popupCheat.SetActive(false);
	}

	void SetTitleText()
	{
		// 정보 링크
		// https://docs.febucci.com/text-animator-unity/effects/built-in-effects-list
		int randValue = Random.Range(0, titlePrefix.Count);
		string str = titlePrefix[randValue] + "TokTok";
		textTitle1.SetText(str);
		LogManager.Log("SetTitleText : " + str);
	}

	void SetGradeScrollInfo()
	{
		float gradeCount = 0;
		foreach(GradeDBEntity entity in LocalDataManager.instance.GetGradeEntity())
		{
			if(entity.minLevel <= (LocalDataManager.instance.GetMaxLevel()+1))
			{
				CreateGrade(entity);
				gradeCount++;
			}
		}
		float curGrade = (float)LocalDataManager.instance.GetCurGrade();
		if(curGrade == 1f)
		{
			scrollBarValue = 1f;
		}
		else
		{
			scrollBarValue = 1f - (curGrade / gradeCount);
		}
		LogManager.Log("SetGradeScrollInfo : " + scrollBarValue + " CurGrade : " + curGrade);
	}

	void SetGradeScrollValue()
	{
		//scrollbar.GetComponent<Scrollbar>().value = scrollBarValue;
		LogManager.Log("SetGradeScrollValue : " + scrollbar.GetComponent<Scrollbar>().value + "  :  " + scrollBarValue);
		gradeScrollRect.verticalNormalizedPosition = scrollBarValue;
	}

	void CreateGrade(GradeDBEntity entity)
	{
		GameObject createGrade = null;
		if (entity.id == LocalDataManager.instance.GetCurGrade())
		{
			createGrade = gradeCurrent;
		}
		else
		{
			createGrade = gradeEtc;
		}
		LocalDataManager.instance.CreateGradeInfo(entity, createGrade, gradeScroll.transform);
	}

	void SetGimmickScrollInfo()
	{
		foreach (GimmickDBEntity entity in LocalDataManager.instance.GetGimmickEntity())
		{
			LocalDataManager.instance.CreateGimmickInfo(entity, gimmickPrefab, gimmickScroll.transform);
		}
	}

	public void SetSetting()
	{
		bool soundOff = LocalDataManager.instance.GetSoundOff();
		if(soundOff)
		{
			// 사운드 꺼짐
			soundOntoggle.isOn = false;
		}
		else
		{
			// 사운드 켜짐
			soundOntoggle.isOn = true;
		}

		bool vibrateOff = LocalDataManager.instance.GetVibrateOff();
		if (vibrateOff)
		{
			// 진동 꺼짐
			vibrateOntoggle.isOn = false;
		}
		else
		{
			// 진동 켜짐
			vibrateOntoggle.isOn = true;
		}

		bool guidelineOff = LocalDataManager.instance.GetGuideLineOff();
		if (guidelineOff)
		{
			// 가이드라인 꺼짐
			guidelineOntoggle.isOn = false;
		}
		else
		{
			// 가이드라인 켜짐
			guidelineOntoggle.isOn = true;
		}
	}

	public void SoundOn(bool value)
	{
		soundOntoggle.isOn = value;
		LocalDataManager.instance.SetSoundOff(!value);
		if (value)
		{
			AudioManager.instance.TickTockPlay();
		}
		else
		{
			AudioManager.instance.TickTockStop();
		}
	}

	public void VibrateOn(bool value)
	{
		vibrateOntoggle.isOn = value;
		LocalDataManager.instance.SetVibrateOff(!value);
	}

	public void GuideLineOn(bool value)
	{
		guidelineOntoggle.isOn = value;
		LocalDataManager.instance.SetGuideLineOff(!value);
	}

	public void SetMarksInfo()
	{
		listMark.Clear();
		int markCount = 0;
		foreach (GradeDBEntity entity in LocalDataManager.instance.GetGradeEntity())
		{
			if (LocalDataManager.instance.GetCurLevel() >= entity.minLevel)
			{
				CreateMark(entity);
				markCount++;
			}
		}
		RectTransform rectTransform = marks.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(markCount*100, rectTransform.sizeDelta.y);
	}

	private void CreateMark(GradeDBEntity entity)
	{
		GameObject markGameObject = Instantiate(markPrefab, Vector3.zero, Quaternion.identity);
		markGameObject.transform.SetParent(marks.transform);
		Mark mark = markGameObject.GetComponent<Mark>();
		mark.SetData(entity);
		listMark.Add(mark);
	}

	public void StopMarksJump()
	{
		foreach(Mark mark in listMark)
		{
			mark.StopJump();
		}
	}

	public void PunchScale(Transform t)
	{
		// 버튼의 transform에 DOPunchScale 효과 적용
		// new Vector3(0.2f, 0.2f, 0f): x, y축으로 0.2만큼 펀치 효과
		// 0.3f: 효과 지속 시간, 10: 진동 횟수, 1f: 탄력성
		t.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 3f, 10, 0.5f).SetLoops(-1, LoopType.Restart);
	}

	public void StopTweenAni(Transform t)
	{
		t.DOKill();
	}

	public void SetReddot()
	{
		if(LocalDataManager.instance.GetReddotPlayer())
		{
			btnPlayer.transform.Find(Define.REDDOT).gameObject.SetActive(true);
			PunchScale(btnPlayer.transform);
		}
		if (LocalDataManager.instance.GetReddotMenu())
		{
			btnMenu.transform.Find(Define.REDDOT).gameObject.SetActive(true);
			PunchScale(btnMenu.transform);
		}
		if (LocalDataManager.instance.GetReddotPlay())
		{
			btnPlay.transform.Find(Define.REDDOT).gameObject.SetActive(true);
		}
	}

	public void OnClearADWork()
	{
		LocalDataManager.instance.ResetPlayADCount();
		SwapAdBtnToPlayBtn();
	}

	private void ActiveBtnPlay()
	{
		btnPlay.SetActive(true);
	}

	public void SwapAdBtnToPlayBtn()
	{
		btnTempAD.SetActive(false);
		ActiveBtnPlay();
	}

	public void SetCheat()
	{
		if(LocalDataManager.instance.GetCheatStage())
		{
			btnCheat.SetActive(true);
		}
	}

	public void SetClientVersion()
	{
		textVersion.text = Define.CLIENT_VERSION;
	}

	private IEnumerator PlayVideoWithFade()
	{
		// 타이틀 문구 끄기
		textTitle1.gameObject.SetActive(false);
		AdjustVideoScale();
		// 비디오 준비
		videoPlayer.Prepare();

		// 최대 7초 대기
		float timeout = 7f;
		float startTime = Time.time;

		while (!videoPlayer.isPrepared)
		{
			if (Time.time - startTime > timeout)
			{
				LogManager.LogError("Video preparation timed out.");
				LoadLevelSceneForced();
				yield break;
			}
			yield return null;
		}

		// 비디오 재생
		videoPlayer.Play();
		rawImage.texture = videoPlayer.texture;
		canvas.SetActive(true);

		// 비디오 재생이 끝날 때까지 대기
		while (videoPlayer.isPlaying)
		{
			if (Time.time - startTime > timeout)
			{
				LogManager.LogError("Video playback timed out.");
				LoadLevelSceneForced();
				yield break;
			}
			yield return null;
		}
		
		// 강제로 인게임신 전환
		LoadLevelSceneForced();
	}

	void AdjustVideoScale()
	{
		float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
		float multiflyValue = canvasWidth / (float)videoWidth;
		RectTransform rawImageRectTransform = rawImage.rectTransform;
		rawImageRectTransform.localScale = new Vector3(multiflyValue, multiflyValue, 1f);
	}

	public void CheckGameCenterInit()
	{
		GameCenterManager.CheckAuthInit();
	}

	public void ReqRankInfo()
	{
		if(GameCenterManager.GetRankInfoAll() == null)
		{
			GameCenterManager.RequestAllRank();
		}
	}

	public void ShowRankInfo()
	{
		ShowSingleRank(GameCenterManager.GetRankInfoAll());
	}

	public void ShowLeaderboardUI()
	{
		if (!LocalDataManager.instance.GetReddotRank())
		{
			btnAllRank.transform.Find(Define.REDDOT).gameObject.SetActive(false);
			LocalDataManager.instance.SetReddotRank();
		}		
		GameCenterManager.ShowLeaderboardUI();
	}

	public void ShowSingleRank(Define.RankInfo rankInfo)
	{
		if (rankInfo == null)
			return;

		btnAllRank.SetActive(true);
		if(!LocalDataManager.instance.GetReddotRank())
		{
			btnAllRank.transform.Find(Define.REDDOT).gameObject.SetActive(true);
		}
		TextMeshProUGUI buttonText = btnAllRank.GetComponentInChildren<TextMeshProUGUI>();
		buttonText.text = rankInfo.GetTextInfo();
	}

	public void ShowAchieveUI()
	{
		GameCenterManager.ShowAchieveUI();
	}
}

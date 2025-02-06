using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;
using static Define;
using System.Text;

public class LobbyScene : MonoBehaviour
{
	[SerializeField] private GameObject btnPlay;
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


	private Image buttonPlayImage;
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

	private void Awake()
	{
		buttonPlayImage = btnPlay.transform.Find("Image").GetComponent<Image>();
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
		TextMeshProUGUI buttonText = btnPlay.GetComponentInChildren<TextMeshProUGUI>();
		if (buttonText != null)
		{
			int level = LocalDataManager.instance.GetCurLevel();
			buttonText.text = level.ToString();
		}
		Color curGradeColor = LocalDataManager.instance.GetCurColor();
		buttonPlayImage.color = curGradeColor;
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
		AudioManager.instance.TickTockPlay();

		time = 0.0f;
	}

	void Update()
	{
		time += Time.deltaTime;
		float t = Mathf.PingPong(time / duration, 1.0f);
		cam.backgroundColor = Color.Lerp(bottomColor, topColor, t);
	}

	public void LoadLevelScene()
    {
		SceneManager.LoadScene("IngameScene");
	}

	public void OnClickShare()
	{
		popupPlayer.SetActive(true);
		if (LocalDataManager.instance.GetReddotPlayer())
		{
			LocalDataManager.instance.SetReddotPlayer(false);
			btnPlayer.transform.Find("Reddot").gameObject.SetActive(false);
		}
			
		SetGradeScrollValue();
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
			btnMenu.transform.Find("Reddot").gameObject.SetActive(false);
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
		string str = titlePrefix[randValue] + "TapTok";
		textTitle1.SetText(str);
		Debug.Log("SetTitleText : " + str);
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
		Debug.Log("SetGradeScrollInfo : " + scrollBarValue);
	}

	void SetGradeScrollValue()
	{
		scrollbar.GetComponent<Scrollbar>().value = scrollBarValue;
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
		markGameObject.GetComponent<Mark>().SetData(entity);
	}

	public void SetReddot()
	{
		if(LocalDataManager.instance.GetReddotPlayer())
		{
			btnPlayer.transform.Find("Reddot").gameObject.SetActive(true);
		}
		if (LocalDataManager.instance.GetReddotMenu())
		{
			btnMenu.transform.Find("Reddot").gameObject.SetActive(true);
		}
	}
}

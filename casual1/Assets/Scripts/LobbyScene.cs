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


	private Image buttonPlayImage;
	private Image buttonPlayerImage;
	private Image buttonMenuImage;
	private List<string> titlePrefix;

	public float scrollBarValue = 0;

	public Toggle soundOntoggle;
	public Toggle vibrateOntoggle;
	public Toggle guidelineOntoggle;
	private Sprite spriteInfinity;

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
	}

    public void LoadLevelScene()
    {
		SceneManager.LoadScene("IngameScene");
	}

	public void OnClickShare()
	{
		popupPlayer.SetActive(true);
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
		// Á¤º¸ ¸µÅ©
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
			if(entity.minLevel < LocalDataManager.instance.GetMaxLevel())
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
		GameObject gradeGameObject = null;
		GameObject createGrade = null;
		if (entity.id == LocalDataManager.instance.GetCurGrade())
		{
			createGrade = gradeCurrent;
		}
		else
		{
			createGrade = gradeEtc;
		}
		gradeGameObject = Instantiate(createGrade, Vector3.zero, Quaternion.identity);
		gradeGameObject.transform.SetParent(gradeScroll.transform);
		gradeGameObject.transform.localScale = Vector3.one;

		Transform iconTransform = gradeGameObject.transform.Find("Content/Icon/Mask/Image");
		Transform nameTransform = gradeGameObject.transform.Find("Content/NameText");
		Transform messageTransform = gradeGameObject.transform.Find("Content/MessageText");
		// ¾ÆÀÌÄÜ »ö»ó º¯°æ
		iconTransform.gameObject.GetComponent<Image>().color = LocalDataManager.instance.GetGradeColor(entity.id);
		nameTransform.gameObject.GetComponent<TextMeshProUGUI>().SetText(entity.grade);
		messageTransform.gameObject.GetComponent<TextMeshProUGUI>().SetText(GetGradeInfoStr(entity));

		/*
		Gimmick gameObjectGimmick = gimmickGameObject.GetComponent<Gimmick>();
		SetTargetStateByGimmickInit(gimmickType);
		bool isChecked = GetGimmickInitChecked(gimmickType, listGimmick);
		Color color = GetGimmickColor(gimmickType, hp, isChecked);
		gameObjectGimmick.SetGimmick(gimmickType, hp, color, angle, listGimmick, isChecked, targetCircle.gameObject);
		listGimmick.Add(gimmickGameObject);
		*/
	}

	public string GetGradeInfoStr(GradeDBEntity entity)
	{
		if(entity.id == 1)
		{
			return "Clear the Level to earn a Grade.";
		}

		StringBuilder sb = new StringBuilder();
		sb.Append(entity.minLevel - 1).Append(" Level Clear");
		if(entity.doubleDamage > 0)
		{
			sb.Append("\n").Append(Define.SKILL_DOUBLE_DAMAGE).Append(" : ").Append(entity.doubleDamage).Append("%");
		}
		if (entity.bonusShot > 0)
		{
			sb.Append("\n").Append(Define.SKILL_FINAL_SHOT).Append(" : ").Append(entity.bonusShot).Append("%");
		}
		return sb.ToString();
	}

	public void SetSetting()
	{
		bool soundOff = LocalDataManager.instance.GetSoundOff();
		if(soundOff)
		{
			// »ç¿îµå ²¨Áü
			soundOntoggle.isOn = false;
		}
		else
		{
			// »ç¿îµå ÄÑÁü
			soundOntoggle.isOn = true;
		}

		bool vibrateOff = LocalDataManager.instance.GetVibrateOff();
		if (vibrateOff)
		{
			// Áøµ¿ ²¨Áü
			vibrateOntoggle.isOn = false;
		}
		else
		{
			// Áøµ¿ ÄÑÁü
			vibrateOntoggle.isOn = true;
		}

		bool guidelineOff = LocalDataManager.instance.GetGuideLineOff();
		if (guidelineOff)
		{
			// °¡ÀÌµå¶óÀÎ ²¨Áü
			guidelineOntoggle.isOn = false;
		}
		else
		{
			// °¡ÀÌµå¶óÀÎ ÄÑÁü
			guidelineOntoggle.isOn = true;
		}
	}

	public void SoundOn(bool value)
	{
		soundOntoggle.isOn = value;
		LocalDataManager.instance.SetSoundOff(!value);
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
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;

public class LobbyScene : MonoBehaviour
{
	private bool isInit = false;
	[SerializeField] private GameObject btnPlay;
	[SerializeField] private GameObject btnPlayer;
	[SerializeField] private GameObject btnMenu;
	[SerializeField] private GameObject popupPlayer;
	[SerializeField] private GameObject popupMenu;
	[SerializeField] private GameObject popupCheat;
	[SerializeField] private TextMeshProUGUI textTitle1;

	private Image buttonPlayImage;
	private Image buttonPlayerImage;
	private Image buttonMenuImage;
	private List<string> titlePrefix;

	private void Awake()
	{
		buttonPlayImage = btnPlay.transform.Find("Image").GetComponent<Image>();
		buttonPlayerImage = btnPlayer.GetComponent<Image>();
		buttonMenuImage = btnMenu.GetComponent<Image>();
		titlePrefix = new List<string>();
		titlePrefix.Add("<rainb>");
		titlePrefix.Add("<pend>");
		titlePrefix.Add("<dangle>");
		titlePrefix.Add("<fade d=1>");
		titlePrefix.Add("<rot>");
		titlePrefix.Add("<bounce a=3>");
		titlePrefix.Add("<slide>");
		titlePrefix.Add("<swing>");
		titlePrefix.Add("<wave>");
		titlePrefix.Add("<incr a=1 f=4>");
		titlePrefix.Add("<shake a=3>");
		titlePrefix.Add("<wiggle>");
		titlePrefix.Add("");
	}

	void Start()
    {
		if (isInit == false)
		{
			// 60 FPS로 고정
			Application.targetFrameRate = 60;
			QualitySettings.vSyncCount = 0; // vSync를 비활성화하여 targetFrameRate가 적용되도록 설정
			Time.fixedDeltaTime = 0.01f; // 원하는 고정 시간 스텝 설정
			isInit = true;
		}
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
		SetTitleText();
	}

    public void LoadLevelScene()
    {
		SceneManager.LoadScene("IngameScene");
	}

	public void OnClickShare()
	{
		popupPlayer.SetActive(true);
		popupPlayer.transform.localScale = Vector3.zero;
		popupPlayer.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCirc);
	}

	public void OnCloseShare()
	{
		//popupShare.SetActive(false);
		popupPlayer.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => popupPlayer.SetActive(false));
	}

	public void OnClickMenu()
	{
		popupMenu.SetActive(true);
		popupMenu.transform.localScale = Vector3.zero;
		popupMenu.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCirc);
	}

	public void OnCloseMenu()
	{
		//popupShare.SetActive(false);
		popupMenu.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => popupMenu.SetActive(false));
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
}

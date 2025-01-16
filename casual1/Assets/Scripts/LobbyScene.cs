using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
	private bool isInit = false;
	[SerializeField] private GameObject btnPlay;
	[SerializeField] private GameObject btnPlayer;
	[SerializeField] private GameObject btnMenu;
	[SerializeField] private GameObject popupPlayer;
	[SerializeField] private GameObject popupMenu;
	[SerializeField] private GameObject popupCheat;

	private Image buttonPlayImage;
	private Image buttonPlayerImage;
	private Image buttonMenuImage;

	private void Awake()
	{
		buttonPlayImage = btnPlay.transform.Find("Image").GetComponent<Image>();
		buttonPlayerImage = btnPlayer.GetComponent<Image>();
		buttonMenuImage = btnMenu.GetComponent<Image>();
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
}

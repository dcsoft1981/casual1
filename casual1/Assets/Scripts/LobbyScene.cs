using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LobbyScene : MonoBehaviour
{
	private bool isInit = false;
	[SerializeField] private GameObject btnPlay;
	[SerializeField] private GameObject popupShare;
	[SerializeField] private GameObject popupMenu;
	[SerializeField] private GameObject popupCheat;

	void Start()
    {
		if (isInit == false)
		{
			// 60 FPS로 고정
			Application.targetFrameRate = 60;
			QualitySettings.vSyncCount = 0; // vSync를 비활성화하여 targetFrameRate가 적용되도록 설정
			isInit = true;
		}
		TextMeshProUGUI buttonText = btnPlay.GetComponentInChildren<TextMeshProUGUI>();
		if (buttonText != null)
		{
			int level = LocalDataManager.instance.GetCurLevel();
			buttonText.text = level.ToString();
		}
	}

    public void LoadLevelScene()
    {
		SceneManager.LoadScene("SampleScene");
	}

	public void OnClickShare()
	{
		popupShare.SetActive(true);
		popupShare.transform.localScale = Vector3.zero;
		popupShare.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCirc);
	}

	public void OnCloseShare()
	{
		//popupShare.SetActive(false);
		popupShare.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => popupShare.SetActive(false));
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
		popupCheat.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => popupCheat.SetActive(false));
	}
}

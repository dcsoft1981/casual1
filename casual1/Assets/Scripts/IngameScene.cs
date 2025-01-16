using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameScene : MonoBehaviour
{
	private bool isInit = false;
	[SerializeField] private GameObject popupIngameCheat;

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
	}

	public void OnClickIngameCheat()
	{
		popupIngameCheat.SetActive(true);
		popupIngameCheat.transform.localScale = Vector3.zero;
		popupIngameCheat.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutCirc);
	}

	public void OnCloseIngameCheat()
	{
		popupIngameCheat.SetActive(false);
	}

	public void OnClickHome()
	{
		SceneManager.LoadScene("LobbyScene");
	}
}

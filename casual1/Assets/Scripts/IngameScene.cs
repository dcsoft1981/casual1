using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameScene : MonoBehaviour
{
	[SerializeField] private GameObject btnCheat;
	[SerializeField] private GameObject popupIngameCheat;

	void Start() 
	{
		SetCheat();
	}

	public void SetCheat()
	{
		if (LocalDataManager.instance.GetCheatStage())
		{
			btnCheat.SetActive(true);
		}
	}

	public void OnClickIngameCheat()
	{
		if (!LocalDataManager.instance.GetCheatStage())
			return;

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

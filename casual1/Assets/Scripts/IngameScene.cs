using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameScene : MonoBehaviour
{
	[SerializeField] private GameObject popupIngameCheat;

	void Start() 
	{

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

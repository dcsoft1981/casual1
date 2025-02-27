using TMPro;
using UnityEngine;

public class PopupCheat : MonoBehaviour
{
    public TMP_InputField inputFieldLevel;
	public TMP_InputField inputFieldVibrate;
	public TMP_InputField inputFieldInfinityStage;
	bool initVibrate = false;

	public void OnClickCheatLevel()
    {
		if (!LocalDataManager.instance.GetCheatStage())
			return;
		int level = int.Parse(inputFieldLevel.text);
		LocalDataManager.instance.SetCurLevel(level);
	}

	public void OnClickCheatVibrate()
	{
		if(!initVibrate)
		{
			initVibrate = true;
		}
		int type = int.Parse(inputFieldVibrate.text);
		switch(type)
		{
			case 1: Taptic.Warning(); break;
			case 2: Taptic.Failure(); break;
			case 3: Taptic.Success(); break;
			case 4: Taptic.Light(); break;
			case 5: Taptic.Medium(); break;
			case 6: Taptic.Heavy(); break;
			case 7: Taptic.Default(); break;
			case 8: Taptic.Vibrate(); break;
			case 9: Taptic.Selection(); break;
		}
	}

	public void OnClickResetReview()
	{
		PlayerPrefs.SetInt("DoNotShowRatePopUp", 0);
	}

	public void OnClickCheaInfinityStage()
	{
		if (!LocalDataManager.instance.GetCheatStage())
			return;
		int stage = int.Parse(inputFieldInfinityStage.text);
		LocalDataManager.instance.SetInfinityStage(stage);
	}
}

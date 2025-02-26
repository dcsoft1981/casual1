using TMPro;
using UnityEngine;

public class PopupCheat : MonoBehaviour
{
    public TMP_InputField inputFieldStage;
	public TMP_InputField inputFieldVibrate;
	bool initVibrate = false;

	public void OnClickCheatStage()
    {
		if (!LocalDataManager.instance.GetCheatStage())
			return;
		int stage = int.Parse(inputFieldStage.text);
		LocalDataManager.instance.SetCurLevel(stage);
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
}

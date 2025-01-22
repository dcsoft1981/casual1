using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VibrationUtility;

public class PopupCheat : MonoBehaviour
{
    public TMP_InputField inputFieldStage;
	public TMP_InputField inputFieldVibrate;
	bool initVibrate = false;

	public void OnClickCheatStage()
    {
		int stage = int.Parse(inputFieldStage.text);
		LocalDataManager.instance.SetCurLevel(stage);
	}

	public void OnClickCheatVibrate()
	{
		if(!initVibrate)
		{
			initVibrate = true;
			VibrationUtil.Init();
		}
		int type = int.Parse(inputFieldVibrate.text);
		VibrationType vibrationType = VibrationType.Default;
		switch(type)
		{
			case 1: vibrationType = VibrationType.Default; break;
			case 2: vibrationType = VibrationType.Peek; break;
			case 3: vibrationType = VibrationType.Pop; break;
			case 4: vibrationType = VibrationType.Nope; break;
			case 5: vibrationType = VibrationType.Heavy; break;
			case 6: vibrationType = VibrationType.Medium; break;
			case 7: vibrationType = VibrationType.Light; break;
			case 8: vibrationType = VibrationType.Rigid; break;
			case 9: vibrationType = VibrationType.Soft; break;
			case 10: vibrationType = VibrationType.Error; break;
			case 11: vibrationType = VibrationType.Success; break;
			case 12: vibrationType = VibrationType.Warning; break;
		}
		VibrationUtil.Vibrate(vibrationType);
	}
}

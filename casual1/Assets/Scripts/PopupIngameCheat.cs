using TMPro;
using UnityEngine;

public class PopupIngameCheat : MonoBehaviour
{
	public TMP_InputField inputFieldRotation;
	public TMP_InputField inputFieldShot;

	public void OnClickIngameCheatRotation()
	{
		if (!LocalDataManager.instance.GetCheatStage())
			return;
		int rotation = int.Parse(inputFieldRotation.text);
		GameManager.instance.SetCheatRotation(rotation);
	}

	public void OnClickIngameCheatShot()
	{
		if (!LocalDataManager.instance.GetCheatStage())
			return;
		int shot = int.Parse(inputFieldShot.text);
		GameManager.instance.AddShot(shot);
	}
}

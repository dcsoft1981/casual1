using TMPro;
using UnityEngine;

public class PopupIngameCheat : MonoBehaviour
{
	public TMP_InputField inputFieldRotation;

	public void OnClickIngameCheatRotation()
	{
		int rotation = int.Parse(inputFieldRotation.text);
		GameManager.instance.SetCheatRotation(rotation);
	}
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCheat : MonoBehaviour
{
    public TMP_InputField inputFieldStage;

    public void OnClickCheatStage()
    {
		int stage = int.Parse(inputFieldStage.text);
		LocalDataManager.instance.SetCurLevel(stage);
	}
}

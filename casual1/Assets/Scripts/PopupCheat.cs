using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCheat : MonoBehaviour
{
    public TMP_InputField inputFieldStage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickCheatStage()
    {
		int stage = int.Parse(inputFieldStage.text);
		LocalDataManager.instance.SetCurLevel(stage);
	}
}

using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class PinLauncher : MonoBehaviour
{
	[SerializeField]
    private GameObject pinObject;

    private Pin currPin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		PreparePin();
	}

    // Update is called once per frame
    void Update()
    {
		if (currPin != null && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !EventSystem.current.IsPointerOverGameObject() && GameManager.instance.isGameOver == false)
        {
            currPin.Launch();
			GameManager.instance.DecreaseShot();
			currPin = null;
            Invoke("PreparePin", 0.10f);
		}
    }

	void PreparePin()
    {
		Invoke("CheckFailure", 0.2f);
		if (GameManager.instance.isGameOver == false)
        {
			GameObject pin = Instantiate(pinObject, transform.position, Quaternion.Euler(0, 0, 0));
			currPin = pin.GetComponent<Pin>();
			GameManager.instance.SetCreatedPin(currPin);
			GameManager.instance.ResetHitGimmick();
			GameManager.instance.CheckTriggerSkill(PassiveType.SHOT_DOUBLE_DAMAGE);
		}
	}

    void CheckFailure()
    {
		GameManager.instance.CheckFailure();
	}
}

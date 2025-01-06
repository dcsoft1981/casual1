using UnityEngine;
using UnityEngine.EventSystems;

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
            Invoke("PreparePin", 0.15f);
		}
    }

	void PreparePin()
    {
        GameManager.instance.CheckFailure();
        if(GameManager.instance.isGameOver == false)
        {
			GameObject pin = Instantiate(pinObject, transform.position, Quaternion.identity);
			currPin = pin.GetComponent<Pin>();
		}
	}
}

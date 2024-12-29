using UnityEngine;

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
        if (currPin != null && Input.GetMouseButtonDown(0) && GameManager.instance.isGameOver == false)
        {
            currPin.Launch();
            currPin = null;
            Invoke("PreparePin", 0.15f);

		}
    }

    void PreparePin()
    {
        if(GameManager.instance.isGameOver == false)
        {
			GameObject pin = Instantiate(pinObject, transform.position, Quaternion.identity);
			currPin = pin.GetComponent<Pin>();
		}
	}
}

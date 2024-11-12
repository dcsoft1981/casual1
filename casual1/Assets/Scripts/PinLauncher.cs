using UnityEngine;

public class PinLauncher : MonoBehaviour
{
    [SerializeField]
    private GameObject pinObject;

    private Pin currPin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PreparePing();

	}

    // Update is called once per frame
    void Update()
    {
        if (currPin != null && Input.GetMouseButtonDown(0))
        {
            currPin.Launch();
            currPin = null;
            Invoke("PreparePing", 0.1f);

		}
    }

    void PreparePing()
    {
        GameObject pin = Instantiate(pinObject, transform.position, Quaternion.identity);
        currPin = pin.GetComponent<Pin>();

	}
}

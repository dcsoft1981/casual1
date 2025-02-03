using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class PinLauncher : MonoBehaviour
{
	[SerializeField] private GameObject pinObject;
	[SerializeField] private ParticleSystem effectPrab;
	[SerializeField] private Transform effectGroup;

	private Pin currPin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		PreparePin();
	}

    // Update is called once per frame
    void Update()
    {
		bool noCheckPosition = false;
		if (Input.touchCount > 0)
		{
			// ù ��° ��ġ ��������
			Touch touch = Input.GetTouch(0);

			// ��ġ ���� �������� Ȯ��
			if (touch.phase == TouchPhase.Began)
			{
				// ��ġ ��ġ ��������
				Vector3 touchPosition = touch.position;

				// z ��ǥ ���� (ī�޶���� �Ÿ�)
				touchPosition.z = Camera.main.nearClipPlane;

				// ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ
				Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);

				// ��� ���
				Debug.Log("���� ��ǥ: " + worldPosition);
				if(worldPosition.x > -1f && worldPosition.x < 1f && worldPosition.y > -3f && worldPosition.y < -1f)
				{
					noCheckPosition = true;
				}
			}
		}

		if (currPin != null && 
			(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && 
			(!EventSystem.current.IsPointerOverGameObject() || noCheckPosition) && 
			GameManager.instance.isGameOver == false)
        {
			LaunchPin();
		}
    }

	public void LaunchPin()
	{
		currPin.Launch();
		LocalDataManager.instance.AddShotPlayData();
		GameManager.instance.TutorialButtonClick();
		GameManager.instance.DecreaseShot();
		currPin = null;
		Invoke("PreparePin", 0.1f);
	}

	void PreparePin()
    {
		if (GameManager.instance.isGameOver == false)
        {
			if (GameManager.instance.GetCheckFinalShot() && GameManager.instance.GetShotCount() == 0)
			{
				return;
			}

			GameObject pin = Instantiate(pinObject, transform.position, Quaternion.Euler(0, 0, 0));
			currPin = pin.GetComponent<Pin>();
			currPin.SetPinId(GameManager.instance.GetNextPinID());
			currPin.effect = Instantiate(effectPrab, effectGroup);
			GameManager.instance.SetCreatedPin(currPin);
			GameManager.instance.ResetHitGimmick();
			bool skillTriggered = GameManager.instance.CheckTriggerSkill(PassiveType.SHOT_DOUBLE_DAMAGE);
			if(!skillTriggered)
			{
				currPin.CreateScaleChange();
			}
		}
	}

    void CheckFinalShot()
    {
		GameManager.instance.CheckFinalShot();
	}
}

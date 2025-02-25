using UnityEngine;
using UnityEngine.EventSystems;
using static Define;
using System.Collections.Generic;

public class PinLauncher : MonoBehaviour
{
	[SerializeField] private GameObject pinObject;
	[SerializeField] private ParticleSystem effectPrab;
	[SerializeField] private Transform effectGroup;
	[SerializeField] private GameObject tempPinPrefab;

	private Pin currPin;
	float staffLength = 1.5f;
	float staffOffset = 0.5f;
	List<GameObject> staggLines = new List<GameObject>();
	Queue<TempPin> queueTempPIn = new Queue<TempPin>();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		PreparePin();
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		bool noCheckPosition = false;
		if (Input.touchCount > 0)
		{
			// 첫 번째 터치 가져오기
			Touch touch = Input.GetTouch(0);

			// 터치 시작 시점인지 확인
			if (touch.phase == TouchPhase.Began)
			{
				// 터치 위치 가져오기
				Vector3 touchPosition = touch.position;

				// z 좌표 설정 (카메라와의 거리)
				touchPosition.z = Camera.main.nearClipPlane;

				// 스크린 좌표를 월드 좌표로 변환
				Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);

				// 결과 출력
				LogManager.Log("월드 좌표: " + worldPosition);
				if(worldPosition.x > -5f && worldPosition.x < 5f && worldPosition.y > -4f && worldPosition.y < -1f)
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
		TempPinMove();
		Invoke("PreparePin", 0.1f);
	}

	void CreatePin()
	{
		GameObject pin = Instantiate(pinObject, transform.position, Quaternion.Euler(0, 0, 0));

		/*
		// 핀 색상 등급별 지정
		Transform childTransform = pin.transform.Find(Define.CHILD_SPRITE_OBJECT);
		SpriteRenderer spriteRenderer = childTransform.gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.color = LocalDataManager.instance.GetCurColor();
		*/

		currPin = pin.GetComponent<Pin>();
		currPin.ConnectorSetting(true);
		currPin.SetPinId(GameManager.instance.GetNextPinID());
		currPin.effect = Instantiate(effectPrab, effectGroup);
		GameManager.instance.SetCreatedPin(currPin);
		GameManager.instance.ResetHitGimmick();
		bool skillTriggered = GameManager.instance.CheckTriggerSkill(PassiveType.SHOT_DOUBLE_DAMAGE);
		if (!skillTriggered)
		{
			currPin.CreateScaleChange();
		}
	}

	void PreparePin()
    {
		if (GameManager.instance.isGameOver == false)
        {
			if (GameManager.instance.GetCheckFinalShot() && GameManager.instance.GetShotCount() == 0)
			{
				LogManager.Log("PreparePin No More Shot");
				return;
			}

			CreatePin();
		}
	}

	public void DrawStaff()
	{
		staggLines.Clear();
		for (int i = 0; i < 5; i++)
		{
			DrawStaff(i);
		}
	}

	private void DrawStaff(int index)
	{
		float alpha = 0.6f;
		Color color;
		if(LocalDataManager.instance.GetCurGrade() == 1)
		{
			color = new Color(Define.COLOR_TARGET_BASE.r, Define.COLOR_TARGET_BASE.g, Define.COLOR_TARGET_BASE.b, alpha);
		}
		else
		{
			Color gradeColor = LocalDataManager.instance.GetCurColor();
			color = new Color(gradeColor.r, gradeColor.g, gradeColor.b, alpha);
		}
		// LineRenderer 생성
		string objectName = "StaffLine" + index.ToString();
		GameObject lineObj = new GameObject(objectName);
		lineObj.transform.parent = gameObject.transform;
		LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.startColor = color;
		lineRenderer.endColor = color;
		lineRenderer.positionCount = 2;
		lineRenderer.useWorldSpace = false;
		lineRenderer.startWidth = 0.02f;
		lineRenderer.endWidth = 0.02f;
		lineRenderer.sortingOrder = -10;

		float staffIndexOffset = 0.15f;
		lineRenderer.SetPosition(0, new Vector3(-staffLength, -staffOffset-index* staffIndexOffset, 0));
		lineRenderer.SetPosition(1, new Vector3(staffLength, -staffOffset - index* staffIndexOffset, 0));
		lineObj.transform.localPosition = Vector3.zero;
		lineObj.transform.localScale = Vector3.one;
		LogManager.Log("DrawStaff radius :" + lineObj.transform.position);
		staggLines.Add(lineObj);
	}

	public void StopAni()
	{
		foreach(TempPin tempPin in queueTempPIn)
		{
			tempPin.StopAni();
		}
	}

	public void SetTempPin(int count)
	{
		queueTempPIn.Clear();
		AddTempPin(count);
	}

	public void AddTempPin(int count)
	{
		float positionX = staffLength - 0.2f;
		for (int i = 0; i < count; i++)
		{
			float x = UnityEngine.Random.Range(-positionX, positionX);
			float y = UnityEngine.Random.Range(this.transform.position.y - staffOffset - 0.4f, this.transform.position.y - staffOffset);
			GameObject tempPinObj = Instantiate(tempPinPrefab, new Vector3(x, y, 0f), Quaternion.Euler(0, 0, 0));
			TempPin tempPin = tempPinObj.GetComponent<TempPin>();
			tempPin.SetData(this.transform);
			queueTempPIn.Enqueue(tempPin);
		}
	}

	private void TempPinMove()
	{
		if (queueTempPIn.Count > 0)
		{
			TempPin tempPin = queueTempPIn.Dequeue();
			tempPin.SetMove();
		}
	}

	public void TempPinOff()
	{
		foreach(GameObject staff in staggLines)
		{
			staff.SetActive(false);
		}
		for (int i = 0; i < 100; i++)
		{
			if (queueTempPIn != null && queueTempPIn.Count > 0)
			{
				TempPin tempPin = queueTempPIn.Dequeue();
				tempPin.StopAni();
				tempPin.gameObject.SetActive(false);
			}
			else
			{
				break;
			}
		}
	}
}

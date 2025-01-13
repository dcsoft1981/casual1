using UnityEngine;

public class TargetCircle : MonoBehaviour
{
    //[SerializeField] public float m_RotateSpeed = 1f;
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private AnimationCurve curve;
	private float animationStartTime;
	private float animationDuration;

	private float elapsedTime = 0f;
	private float rotationSpeed = 0f; // ���� �ӵ�
	private float totalTime = 0f;

	private LevelDBEntity levelDBEntity;
	private LineRenderer lineRenderer;

	private SpriteRenderer spriteRenderer;
	private GameObject spriteObject;
	private float spriteScale = 1f;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();

		spriteObject = new GameObject("ChildObject");
		spriteObject.transform.SetParent(this.transform);

		// �ڽ� ������Ʈ�� ��ġ�� �θ�� �����ϰ� �����մϴ�.
		spriteObject.transform.localPosition = Vector3.zero;

		// SpriteRenderer ������Ʈ�� �߰��մϴ�.
		spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
	}

	public void SetSprite(float _spriteScale, Color _targetColor)
	{
		spriteScale = _spriteScale;
		spriteRenderer.sprite = Resources.Load<Sprite>("Circle");
		spriteRenderer.color = _targetColor;
		spriteObject.transform.localScale = new Vector3(spriteScale, spriteScale, 1);
	}

	public void StartAnimation(AnimationCurve animationCurve)
	{
		curve = animationCurve;
		animationStartTime = Time.time;

		// �ִϸ��̼� ���� ����
		animationDuration = curve[curve.length - 1].time;
	}

	public void SetLevelDB(LevelDBEntity levelDBEntity)
	{
		this.levelDBEntity = levelDBEntity;
		/*
		if (levelDBEntity.time0 > totalTime)
			totalTime = levelDBEntity.time0;
		if (levelDBEntity.time1 > totalTime)
			totalTime = levelDBEntity.time1;
		if (levelDBEntity.time2 > totalTime)
			totalTime = levelDBEntity.time2;
		if (levelDBEntity.time3 > totalTime)
			totalTime = levelDBEntity.time3;
		if (levelDBEntity.time4 > totalTime)
			totalTime = levelDBEntity.time4;
		*/
		totalTime = Define.ROTATE_SEC;
	}

	private float GetCurRotationSpeed(float curTime)
	{
		/*
		if (curTime < levelDBEntity.time0)
			return levelDBEntity.rotation0;
		else if (curTime < levelDBEntity.time1)
			return levelDBEntity.rotation1;
		else if (curTime < levelDBEntity.time2)
			return levelDBEntity.rotation2;
		else if (curTime < levelDBEntity.time3)
			return levelDBEntity.rotation3;
		else if (curTime < levelDBEntity.time4)
			return levelDBEntity.rotation4;
		else return 0f;
		*/

		int cheatRotation = GameManager.instance.GetCheatRotation();
		if (cheatRotation != 0)
			return cheatRotation;
		return levelDBEntity.rotation;
	}

    // Update is called once per frame
    void Update()
    {
		/*
        if(GameManager.instance.isGameOver == false )
            transform.Rotate(0, 0, m_RotateSpeed * Time.deltaTime);
		*/

		if (GameManager.instance.isGameOver == false && curve != null)
		{
			/*
			float elapsedTime = Time.time - animationStartTime;

			// WrapMode.Loop�� ���������Ƿ� �ð��� �ѱ�
			float rotationValue = curve.Evaluate(elapsedTime);

			transform.localEulerAngles = new Vector3(0, 0, rotationValue);
			*/

			elapsedTime += Time.deltaTime;
			float curTime = elapsedTime % totalTime;
			rotationSpeed = GameManager.instance.GetRotationValue(GetCurRotationSpeed(curTime));
			// ���� �ӵ��� ȸ�� ����
			transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
			//Debug.Log("TargetCircle : " + elapsedTime + " - " + curTime + " - " + rotationSpeed);
		}

	}

	public void DrawDamageLine(int _startAngle, int _endAngle)
	{
		float startAngle = _startAngle; // �߰� ���� ���� ���� ����
		float endAngle = _endAngle; // �߰� ���� ���� �� ����
		float segments = 50f; // ���׸�Ʈ ��
		float radius = 0.5f* spriteScale;

		lineRenderer.positionCount = (int)segments + 1;
		lineRenderer.useWorldSpace = false;
		lineRenderer.startWidth = 0.2f;
		lineRenderer.endWidth = 0.2f;

		float angle = startAngle;
		for (int i = 0; i <= segments; i++)
		{
			float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
			float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
			lineRenderer.SetPosition(i, new Vector3(x, y, 0));
			angle += (endAngle - startAngle) / segments;
		}
		Debug.Log("DrawDamageLine radius :" + radius);
	}
}

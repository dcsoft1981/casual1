using AllIn1SpriteShader;
using UnityEngine;

public class TargetCircle : MonoBehaviour
{
    //[SerializeField] public float m_RotateSpeed = 1f;
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private AnimationCurve curve;
	private float animationStartTime;
	private float animationDuration;

	private float elapsedTime = 0f;
	private float rotationSpeed = 0f; // 현재 속도
	private float totalTime = 0f;

	private LevelDBEntity levelDBEntity;
	private LineRenderer lineRenderer;

	private SpriteRenderer spriteRenderer;
	private GameObject spriteObject;
	private float spriteScale = 1f;
	private int damageLineCount = 0;

	[SerializeField] private Transform effectGroup;
	[SerializeField] private ParticleSystem effectPrab = null;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		spriteObject = transform.Find(Define.CHILD_SPRITE_OBJECT).gameObject;
		spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();

		effectPrab = Instantiate(effectPrab, effectGroup);

		/*
		spriteObject = new GameObject(Define.CHILD_SPRITE_OBJECT);
		spriteObject.transform.SetParent(this.transform);

		// 자식 오브젝트의 위치를 부모와 동일하게 설정합니다.
		spriteObject.transform.localPosition = Vector3.zero;

		// SpriteRenderer 컴포넌트를 추가합니다.
		spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
		Shader shader = Shader.Find("AllIn1SpriteShader/AllIn1SpriteShader");
		shader = null;
		if (shader != null)
		{
			spriteObject.AddComponent<AllIn1Shader>();
			spriteRenderer.material = new Material(shader);
			shaderLoaded = true;
			Debug.Log("AllIn1SpriteShader Shader Exist");
		}
		else
		{
			spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
			Debug.LogError("AllIn1SpriteShader Shader Not Exist");
		}
		*/
	}

	public void SetSprite(float _spriteScale, int _targetId)
	{
		spriteScale = _spriteScale;
		//spriteRenderer.sprite = Resources.Load<Sprite>("Circle");
		spriteRenderer.color = Define.COLOR_TARGET_BASE;
		spriteObject.transform.localScale = new Vector3(spriteScale, spriteScale, 1);
		Define.TargetType targetType = (Define.TargetType)_targetId;
		switch(targetType)
		{
			case Define.TargetType.SILVER:
				{
					spriteObject.transform.Find("Sprite1").gameObject.SetActive(true);
				}
				break;
			case Define.TargetType.GOLD:
				{
					spriteObject.transform.Find("Sprite2").gameObject.SetActive(true);
				}
				break;
		}
	}

	public void StartAnimation(AnimationCurve animationCurve)
	{
		curve = animationCurve;
		animationStartTime = Time.time;

		// 애니메이션 길이 설정
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

			// WrapMode.Loop로 설정했으므로 시간만 넘김
			float rotationValue = curve.Evaluate(elapsedTime);

			transform.localEulerAngles = new Vector3(0, 0, rotationValue);
			*/

			elapsedTime += Time.deltaTime;
			float curTime = elapsedTime % totalTime;
			rotationSpeed = GameManager.instance.GetRotationValue(GetCurRotationSpeed(curTime));
			// 현재 속도로 회전 적용
			transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
			//Debug.Log("TargetCircle : " + elapsedTime + " - " + curTime + " - " + rotationSpeed);
		}

	}

	public void DrawDamageLine(int _startAngle, int _endAngle)
	{
		float startAngle = _startAngle; // 추가 점수 범위 시작 각도
		float endAngle = _endAngle; // 추가 점수 범위 끝 각도
		float segments = 50f; // 세그먼트 수
		float radius = 0.5f * spriteScale;

		// LineRenderer 생성
		damageLineCount++;
		string objectName = "DamageLine" + damageLineCount.ToString();
		GameObject lineObj = new GameObject(objectName);
		lineObj.transform.parent = gameObject.transform;
		LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.startColor = Define.DAMAGE_LINE_COLOR;
		lineRenderer.endColor = Define.DAMAGE_LINE_COLOR;
		lineRenderer.numCapVertices = 10; // 끝부분을 둥글게 만들기 위해 추가할 버텍스 수
		lineRenderer.useWorldSpace = false;
		lineRenderer.positionCount = (int)segments + 1;
		lineRenderer.useWorldSpace = false;
		lineRenderer.startWidth = 0.1f;
		lineRenderer.endWidth = 0.1f;
		lineRenderer.sortingOrder = -10;

		float angle = startAngle;
		for (int i = 0; i <= segments; i++)
		{
			float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
			float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
			lineRenderer.SetPosition(i, new Vector3(x, y, 0));
			angle += (endAngle - startAngle) / segments;
		}
		lineObj.transform.localPosition = Vector3.zero;
		lineObj.transform.localScale = Vector3.one;
		Debug.Log("DrawDamageLine radius :" + radius + " , position : " + lineObj.transform.position);
	}

	public void ShieldColorON()
	{
		spriteRenderer.color = Define.COLOR_TARGET_SHIELD;
	}

	public void ShieldColorOFF()
	{
		spriteRenderer.color = Define.COLOR_TARGET_BASE;
	}

	public void EffectPlay()
	{
		float scale = 0.7f;
		effectPrab.transform.position = transform.position;
		effectPrab.transform.localScale = new Vector3(scale, scale, scale);
		Debug.Log("EffectPlay Target Scale : " + effectPrab.transform.localScale);
		effectPrab.GetComponent<Renderer>().sortingOrder = -10;
		effectPrab.Play();
	}
}

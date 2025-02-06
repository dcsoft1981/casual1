using AllIn1SpriteShader;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;
using Color = UnityEngine.Color;

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

	private SpriteRenderer spriteRenderer;
	private GameObject spriteObject;
	private float spriteScale = 1f;
	private int damageLineCount = 0;

	[SerializeField] private Transform effectGroup;
	[SerializeField] private ParticleSystem effectPrab = null;
	private Define.TargetColorType targetColorType;
	private Color gradeColor;
	private Define.ExpressionType expressionType;
	private float expressionLineWidth = 0f;
	private List<LineRenderer> listExpression = null;

	public float expressionAmplitude = 0.05f; // ������ ����
	public float expressionFrequency = 10f; // ������ ���ļ�
	public float expressionSpeed = 10f; // ������ �̵� �ӵ�
	public float expressionLength = 1f;
	public int expressionSegments = 0; // ���׸�Ʈ ��
	public LineRenderer expressionLineRenderer1 = null;
	public LineRenderer expressionLineRenderer2 = null;


	private void Awake()
	{
		spriteObject = transform.Find(Define.CHILD_SPRITE_OBJECT).gameObject;
		spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();

		effectPrab = Instantiate(effectPrab, effectGroup);
		listExpression = new List<LineRenderer>();

		/*
		spriteObject = new GameObject(Define.CHILD_SPRITE_OBJECT);
		spriteObject.transform.SetParent(this.transform);

		// �ڽ� ������Ʈ�� ��ġ�� �θ�� �����ϰ� �����մϴ�.
		spriteObject.transform.localPosition = Vector3.zero;

		// SpriteRenderer ������Ʈ�� �߰��մϴ�.
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

	private void Start()
	{
		SetColorType();
		SetExpressionType();
	}

	public void SetSprite(float _spriteScale, int _targetId)
	{
		spriteScale = _spriteScale;
		gradeColor = LocalDataManager.instance.GetCurColor();
		//spriteRenderer.sprite = Resources.Load<Sprite>("Circle");
		spriteRenderer.color = Define.COLOR_TARGET_BASE;
		spriteObject.transform.localScale = new Vector3(spriteScale, spriteScale, 1);
		Define.TargetType targetType = (Define.TargetType)_targetId;
		// ���� �ϴ� ǥ������ ó��
		/*
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
		*/
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

			if(expressionSegments != 0)
			{
				{
					Vector3[] positions1 = new Vector3[expressionSegments + 1];
					Vector3[] positions2 = new Vector3[expressionSegments + 1];
					for (int i = 0; i <= expressionSegments; i++)
					{
						float x = (expressionLength / expressionSegments) * i;
						float y = expressionAmplitude * Mathf.Sin((x * expressionFrequency) + (Time.time * expressionSpeed));
						positions1[i] = new Vector3(-x, y, 0);
						positions2[i] = new Vector3(x, y, 0);
					}
					expressionLineRenderer1.SetPositions(positions1);
					expressionLineRenderer2.SetPositions(positions2);
				}
			}
		}

	}

	public void DrawDamageLine(int _startAngle, int _endAngle)
	{
		float startAngle = _startAngle; // �߰� ���� ���� ���� ����
		float endAngle = _endAngle; // �߰� ���� ���� �� ����
		float segments = 50f; // ���׸�Ʈ ��
		float radius = 0.5f * spriteScale;

		// LineRenderer ����
		damageLineCount++;
		string objectName = "DamageLine" + damageLineCount.ToString();
		GameObject lineObj = new GameObject(objectName);
		lineObj.transform.parent = gameObject.transform;
		LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.startColor = Define.DAMAGE_LINE_COLOR;
		lineRenderer.endColor = Define.DAMAGE_LINE_COLOR;
		lineRenderer.numCapVertices = 10; // ���κ��� �ձ۰� ����� ���� �߰��� ���ؽ� ��
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

	public void DrawNoDamageLine(int _startAngle, int _endAngle)
	{
		float startAngle = _startAngle; // �߰� ���� ���� ���� ����
		float endAngle = _endAngle; // �߰� ���� ���� �� ����
		float segments = 50f; // ���׸�Ʈ ��
		float radius = 0.5f * spriteScale;

		// LineRenderer ����
		damageLineCount++;
		string objectName = "DamageLine" + damageLineCount.ToString();
		GameObject lineObj = new GameObject(objectName);
		lineObj.transform.parent = gameObject.transform;
		LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.startColor = Define.NODAMAGE_LINE_COLOR;
		lineRenderer.endColor = Define.NODAMAGE_LINE_COLOR;
		lineRenderer.numCapVertices = 10; // ���κ��� �ձ۰� ����� ���� �߰��� ���ؽ� ��
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
		Debug.Log("DrawNoDamageLine radius :" + radius + " , position : " + lineObj.transform.position);
	}

	private LineRenderer GetExpressionLineBase(string lineName, Vector3 position, Color color, int segments)
	{
		GameObject lineObj = new GameObject(lineName);
		LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.startColor = color;
		lineRenderer.endColor = color;
		//lineRenderer.numCapVertices = 20; // ���κ��� �ձ۰� ����� ���� �߰��� ���ؽ� ��
		lineRenderer.positionCount = segments;
		lineRenderer.useWorldSpace = false;
		lineRenderer.startWidth = GetExpressionLineWidth();
		lineRenderer.endWidth = GetExpressionLineWidth();
		lineRenderer.sortingOrder = 10;

		lineObj.transform.localPosition = position;
		lineObj.transform.localScale = Vector3.one;
		listExpression.Add(lineRenderer);
		return lineRenderer;
	}

	private void SetExpressionLine(string lineName, Color color, Define.ExpressionType type, float scale, int index)
	{
		Vector3 position = GetExpressionPosition(type, scale, index);
		switch (type)
		{
			case Define.ExpressionType.CIRCLE:
				{
					float radius = scale * 0.3f; // ���� ������
					float segments = 50f; // ���׸�Ʈ ��

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments + 1);
					float angle = 0f;
					for (int i = 0; i <= segments; i++)
					{
						float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
						float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

						lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
						angle += 360f / segments;
					}
					lineRenderer.loop = true;
				}
				break;
			case Define.ExpressionType.SMILE:
				{
					float length = scale * 0.25f; // �� ����
					float segments = 3f; // ���׸�Ʈ ��

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments);
					// '^' ����� �׸��� ���� �� ���� ��ǥ ����
					Vector3[] positions = new Vector3[(int)segments];
					positions[0] = new Vector3(-1f * length, -1f * length, 0f); // ���� �Ʒ� ��
					positions[1] = new Vector3(0f, 0f, 0f);    // ���� �߾� ��
					positions[2] = new Vector3(length, -1 * length, 0f); // ������ �Ʒ� ��

					// LineRenderer ����
					lineRenderer.positionCount = positions.Length;
					lineRenderer.SetPositions(positions);
				}
				break;
			case Define.ExpressionType.CLOSE:
				{
					float length = scale * 0.2f; // �� ����
					float segments = 3f; // ���׸�Ʈ ��

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments);
					Vector3[] positions = new Vector3[(int)segments];
					if (index == 0)
					{
						// '>' ��ȣ�� ��ǥ ����
						positions[0] = new Vector3(-1f * length, length, 0f);
						positions[1] = new Vector3(length, 0f, 0f);
						positions[2] = new Vector3(-1f * length, -1f * length, 0f);
					}
					else
					{
						// '<' ��ȣ�� ��ǥ ����
						positions[0] = new Vector3(length, length, 0f);
						positions[1] = new Vector3(-1f * length, 0f, 0f);
						positions[2] = new Vector3(length, -1f * length, 0f);
					}

					lineRenderer.SetPositions(positions);
				}
				break;
			case Define.ExpressionType.CRY:
				{
					float length = scale * 0.25f; // �� ����
					float segments = 2f; // ���׸�Ʈ ��

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments);
					Vector3[] positions = new Vector3[(int)segments];
					if (index == 0 || index == 2)
					{
						// ���μ� ��ǥ ����
						positions[0] = new Vector3(-length, 0f, 0f); // ���� ��
						positions[1] = new Vector3(length, 0f, 0f);  // ������ ��
					}
					else
					{
						// ���μ� ��ǥ ����
						positions[0] = new Vector3(0f, 0f, 0f);          // ���μ� �߰�
						positions[1] = new Vector3(0f, -length * 1.2f, 0f);         // �Ʒ��� ��
					}

					lineRenderer.positionCount = positions.Length;
					lineRenderer.SetPositions(positions);
				}
				break;
			case Define.ExpressionType.LINE:
				{
					float length = scale * 0.3f; // �� ����
					float segments = 2f; // ���׸�Ʈ ��

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments);
					Vector3[] positions = new Vector3[(int)segments];
					// ���μ� ��ǥ ����
					positions[0] = new Vector3(-length, 0f, 0f); // ���� ��
					positions[1] = new Vector3(length, 0f, 0f);  // ������ ��
					lineRenderer.positionCount = positions.Length;
					lineRenderer.SetPositions(positions);
				}
				break;
			case Define.ExpressionType.X:
				{
					float length = scale * 0.13f; // �� ����
					float segments = 2f; // ���׸�Ʈ ��

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments);
					Vector3[] positions = new Vector3[(int)segments];
					if (index == 0 || index == 2)
					{
						positions[0] = new Vector3(-length, length, 0f); // ���� ��
						positions[1] = new Vector3(length, -length, 0f); // ������ �Ʒ�
					}
					else
					{
						positions[0] = new Vector3(length, length, 0f); // ������ ��
						positions[1] = new Vector3(-length, -length, 0f); // ���� �Ʒ�
					}

					lineRenderer.positionCount = positions.Length;
					lineRenderer.SetPositions(positions);
				}
				break;
				/*
			case Define.ExpressionType.ONE_LINE:
				{
					float length = scale * 0.8f; // �� ����
					float segments = 2f; // ���׸�Ʈ ��

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments);
					Vector3[] positions = new Vector3[(int)segments];
					positions[0] = new Vector3(-length, 0f, 0f); // ���� ��
					positions[1] = new Vector3(length, 0f, 0f);  // ������ ��
					lineRenderer.positionCount = positions.Length;
					lineRenderer.SetPositions(positions);
				}
				break;
				*/
			case Define.ExpressionType.ONE_CAPSULE:
				{
					float length = scale * scale *0.7f; // �� ����
					float segments = 2f; // ���׸�Ʈ ��

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments);
					lineRenderer.numCapVertices = 30;
					Vector3[] positions = new Vector3[(int)segments];
					positions[0] = new Vector3(-length, 0f, 0f); // ���� ��
					positions[1] = new Vector3(length, 0f, 0f);  // ������ ��
					lineRenderer.positionCount = positions.Length;
					lineRenderer.SetPositions(positions);
				}
				break;
			case Define.ExpressionType.WAVE:
				{
					expressionLength = scale * scale * 0.7f; // �� ����
					position = GetExpressionPosition(type, scale, index);
					expressionSegments = 100;

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, expressionSegments + 1);
					if(index == 0)
						expressionLineRenderer1 = lineRenderer;
					else
						expressionLineRenderer2 = lineRenderer;
				}
				break;
			case Define.ExpressionType.STAR:
				{
					float length = scale * 0.35f; // �� ����
					float segments = 5f; // ���׸�Ʈ ��

					Vector3[] outerPoints = new Vector3[5];
					float angleStep = 2 * Mathf.PI / 5;
					// ���� ���ʺ��� �����ϱ� ���� -90���� offset (���� ��ȯ)
					float startAngle = -Mathf.PI / 2;

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments);

					for (int i = 0; i < 5; i++)
					{
						float currentAngle = startAngle + i * angleStep;
						float x = Mathf.Cos(currentAngle) * length;
						float y = Mathf.Sin(currentAngle) * length;
						outerPoints[i] = new Vector3(x, y, 0);
					}
					Vector3[] positions = new Vector3[(int)segments];
					positions[0] = outerPoints[0];
					positions[1] = outerPoints[2];
					positions[2] = outerPoints[4];
					positions[3] = outerPoints[1];
					positions[4] = outerPoints[3];

					lineRenderer.numCornerVertices = 10;
					lineRenderer.loop = true;
					lineRenderer.positionCount = positions.Length;
					lineRenderer.SetPositions(positions);
				}
				break;
			case Define.ExpressionType.HEART:
				{
					float length = scale * 0.015f; // �� ����
					float segments = 50f; // ���׸�Ʈ ��

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments+1);
					// 0 ~ 2*PI ������ t���� ��ȭ��Ű�� ��Ʈ ��� �� ���
					for (int i = 0; i <= segments; i++)
					{
						// ���� t (���� ����)
						float t = Mathf.PI * 2 * i / segments;
						// ��Ʈ�� parametric �� (������ ��Ʈ � ������)
						// x = 16 * sin��(t)
						// y = 13 * cos(t) - 5*cos(2*t) - 2*cos(3*t) - cos(4*t)
						float x = 16 * Mathf.Pow(Mathf.Sin(t), 3);
						float y = 13 * Mathf.Cos(t) - 5 * Mathf.Cos(2 * t) - 2 * Mathf.Cos(3 * t) - Mathf.Cos(4 * t);
						// ���� ��ǥ�� ������ ���� (z�� 0)
						lineRenderer.SetPosition(i, new Vector3(x, y, 0) * length);
					}
				}
				break;
		}
	}

	public void ClearExpressionLines()
	{
		// ���� ���η����� off
		foreach (LineRenderer line in listExpression)
		{
			if(!line.gameObject.IsDestroyed())
				line.gameObject.SetActive(false);
		}
		listExpression.Clear();
	}

	private Vector3 GetExpressionPosition(Define.ExpressionType type, float scale, int index)
	{
		switch (type)
		{
			case Define.ExpressionType.CIRCLE:
			case Define.ExpressionType.CLOSE:
			case Define.ExpressionType.STAR:
			case Define.ExpressionType.HEART:
				{
					float x = 0.6f;
					if (scale == 1f)
					{
						x = 0.6f;
					}
					else if (scale == 0.8f)
					{
						x = 0.5f;
					}
					else if (scale == 0.5f)
					{
						x = 0.35f;
					}

					float y = 2f;
					if (scale == 1f)
					{
						y = 1.9f;
					}
					else if (scale == 0.8f)
					{
						y = 1.8f;
					}
					else if (scale == 0.5f)
					{
						y = 1.7f;
					}
					if(index == 0)
					{
						x = x * -1f;
					}
					return new Vector3 (x, y, 0f);
				}
			case Define.ExpressionType.SMILE:
			case Define.ExpressionType.LINE:
				{
					float x = 0.6f;
					if (scale == 1f)
					{
						x = 0.6f;
					}
					else if (scale == 0.8f)
					{
						x = 0.5f;
					}
					else if (scale == 0.5f)
					{
						x = 0.35f;
					}
					float y = 2f;
					if (scale == 1f)
					{
						y = 2f;
					}
					else if (scale == 0.8f)
					{
						y = 1.9f;
					}
					else if (scale == 0.5f)
					{
						y = 1.8f;
					}
					if (index == 0)
					{
						x = x * -1f;
					}
					return new Vector3(x, y, 0f);
				}
			case Define.ExpressionType.WAVE:
				{
					float x = 0.6f;
					if (scale == 1f)
					{
						x = 0.4f;
					}
					else if (scale == 0.8f)
					{
						x = 0.3f;
					}
					else if (scale == 0.5f)
					{
						x = 0.25f;
					}
					float y = 2f;
					if (scale == 1f)
					{
						y = 2f;
					}
					else if (scale == 0.8f)
					{
						y = 1.9f;
					}
					else if (scale == 0.5f)
					{
						y = 1.8f;
					}
					if (index == 0)
					{
						x = x * -1f;
					}
					return new Vector3(x, y, 0f);
				}
			case Define.ExpressionType.CRY:
				{
					float x = 0.6f;
					if (scale == 1f)
					{
						x = 0.6f;
					}
					else if (scale == 0.8f)
					{
						x = 0.5f;
					}
					else if (scale == 0.5f)
					{
						x = 0.4f;
					}
					float y = 2f;
					if (scale == 1f)
					{
						y = 2f;
					}
					else if (scale == 0.8f)
					{
						y = 1.9f;
					}
					else if (scale == 0.5f)
					{
						y = 1.8f;
					}
					if (index == 0 || index == 1)
					{
						x = x * -1f;
					}
					return new Vector3(x, y, 0f);
				}
			
			case Define.ExpressionType.X:
				{
					float x = 0.6f;
					if (scale == 1f)
					{
						x = 0.6f;
					}
					else if (scale == 0.8f)
					{
						x = 0.5f;
					}
					else if (scale == 0.5f)
					{
						x = 0.4f;
					}
					float y = 2f;
					if (scale == 1f)
					{
						y = 2f;
					}
					else if (scale == 0.8f)
					{
						y = 1.9f;
					}
					else if (scale == 0.5f)
					{
						y = 1.8f;
					}
					if (index == 0 || index == 1)
					{
						x = x * -1f;
					}
					return new Vector3(x, y, 0f);
				}
			//case Define.ExpressionType.ONE_LINE:
			case Define.ExpressionType.ONE_CAPSULE:
				{
					float y = 2f;
					if (scale == 1f)
					{
						y = 2.1f;
					}
					else if (scale == 0.8f)
					{
						y = 2f;
					}
					else if (scale == 0.5f)
					{
						y = 1.9f;
					}
					return new Vector3(0f, y, 0f);
				}
				/*
			case Define.ExpressionType.WAVE:
				{
					float y = 2f;
					if (scale == 1f)
					{
						y = 2.1f;
					}
					else if (scale == 0.8f)
					{
						y = 2f;
					}
					else if (scale == 0.5f)
					{
						y = 1.9f;
					}
					return new Vector3(-expressionLength/2f, y, 0f);
				}
				*/
		}
		return Vector3.zero;
	}

	public void DrawExpression(int maxHp, int hp)
	{
		ClearExpressionLines();

		switch (expressionType)
		{
			//case Define.ExpressionType.ONE_LINE:
			case Define.ExpressionType.ONE_CAPSULE:
			{
					SetExpressionLine("ExpressionLine0", gradeColor, expressionType, spriteScale, 0);
				}
				break;

			case Define.ExpressionType.CIRCLE:
			case Define.ExpressionType.SMILE:
			case Define.ExpressionType.CLOSE:
			case Define.ExpressionType.LINE:
			case Define.ExpressionType.STAR:
			case Define.ExpressionType.HEART:
			case Define.ExpressionType.WAVE:
				{
					SetExpressionLine("ExpressionLine0", gradeColor, expressionType, spriteScale, 0);
					SetExpressionLine("ExpressionLine1", gradeColor, expressionType, spriteScale, 1);
				}
				break;
			case Define.ExpressionType.CRY:
			case Define.ExpressionType.X:
				{
					SetExpressionLine("ExpressionLine0", gradeColor, expressionType, spriteScale, 0);
					SetExpressionLine("ExpressionLine1", gradeColor, expressionType, spriteScale, 1);
					SetExpressionLine("ExpressionLine2", gradeColor, expressionType, spriteScale, 2);
					SetExpressionLine("ExpressionLine3", gradeColor, expressionType, spriteScale, 3);
				}
				break;
		}
	}

	public void SetExpressionLineScaleUp()
	{
		foreach (LineRenderer line in listExpression)
		{
			if (!line.gameObject.IsDestroyed())
			{
				line.startWidth = GetExpressionLineWidth() * 1.5f;
				line.endWidth = GetExpressionLineWidth() * 1.5f;
			}
		}
	}

	public void SetExpressionLineScaleNormal()
	{
		foreach (LineRenderer line in listExpression)
		{
			if (!line.gameObject.IsDestroyed())
			{
				line.startWidth = GetExpressionLineWidth();
				line.endWidth = GetExpressionLineWidth();
			}
		}
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

	private void SetColorType()
	{
		targetColorType = Define.GetRandomEnumValue<TargetColorType>();
	}

	private void SetExpressionType()
	{
		// ExpressionType ����
		int curLevel = LocalDataManager.instance.GetCurLevel();
		if(curLevel <= 10)
		{
			expressionType = (Define.ExpressionType)curLevel;
		}
		else
		{
			expressionType = Define.GetRandomEnumValue<Define.ExpressionType>();
		}
		//expressionType = Define.ExpressionType.HEART;

		expressionLineWidth = 0.1f;
		switch(expressionType)
		{
			//case Define.ExpressionType.ONE_LINE:
			case Define.ExpressionType.ONE_CAPSULE:
				{
					expressionLineWidth = 0.35f;
				}
				break;
			case Define.ExpressionType.HEART:
				{
					expressionLineWidth = 0.05f;
				}
				break;
		}
		if(spriteScale == 0.5f)
		{
			expressionLineWidth = expressionLineWidth * 0.25f;
		}
	}

	public float GetExpressionLineWidth()
	{
		return expressionLineWidth;
	}

	public float GetColorValueByTargetHp(float maxHp, float curHp)
	{
		return (40f * curHp / maxHp);
	}

	public void GetColorByTargetHp(float maxHp, float curHp)
	{
		float r = 40;
		float g = 40;
		float b = 40;
		float curValue = GetColorValueByTargetHp(maxHp, curHp);
		switch (targetColorType)
		{
			case TargetColorType.RED:
				{
					r = curValue;
				}
				break;
			case TargetColorType.GREEN:
				{
					g = curValue;
				}
				break;
			case TargetColorType.BLUE:
				{
					b = curValue;
				}
				break;
			case TargetColorType.GREEN_BLUE:
				{
					g = curValue;
					b = curValue;
				}
				break;
			case TargetColorType.RED_BLUE:
				{
					r = curValue;
					b = curValue;
				}
				break;
			case TargetColorType.RED_GREEN:
				{
					r = curValue;
					g = curValue;
				}
				break;
			case TargetColorType.ALL:
				{
					r = curValue;
					g = curValue;
					b = curValue;
				}
				break;
			case TargetColorType.NONE:
				{

				}
				break;
		}
		if(GameManager.instance.IsInShield())
		{
			ShieldColorON();
		}
		else
		{
			spriteRenderer.color = Define.GetRGBColor(r, g, b);
		}
	}
}

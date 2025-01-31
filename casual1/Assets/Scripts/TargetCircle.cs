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
	private Define.ExpressionType expressionType;
	private List<LineRenderer> listExpression = null;
	private Color gradeColor;

	private void Awake()
	{
		spriteObject = transform.Find(Define.CHILD_SPRITE_OBJECT).gameObject;
		spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();

		effectPrab = Instantiate(effectPrab, effectGroup);
		SetColorType();
		SetExpressionType();
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
		lineRenderer.startWidth = 0.1f;
		lineRenderer.endWidth = 0.1f;
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

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments+1);
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
					positions[0] = new Vector3(-1f*length, -1f*length, 0f); // ���� �Ʒ� ��
					positions[1] = new Vector3(0f, 0f, 0f);    // ���� �߾� ��
					positions[2] = new Vector3(length, -1*length, 0f); // ������ �Ʒ� ��

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
						positions[0] = new Vector3(-1f* length, length, 0f);
						positions[1] = new Vector3(length, 0f, 0f);
						positions[2] = new Vector3(-1f* length, -1f* length, 0f);
					}
					else
					{
						// '<' ��ȣ�� ��ǥ ����
						positions[0] = new Vector3(length, length, 0f);
						positions[1] = new Vector3(-1f*length, 0f, 0f);
						positions[2] = new Vector3(length, -1f* length, 0f);
					}

					lineRenderer.SetPositions(positions);
				}
				break;
			case Define.ExpressionType.CRY:
				{
					float length = scale * 0.25f; // �� ����
					float segments = 4f; // ���׸�Ʈ ��

					LineRenderer lineRenderer = GetExpressionLineBase(lineName, position, color, (int)segments);
					Vector3[] positions = new Vector3[(int)segments];
					// ���μ� ��ǥ ����
					positions[0] = new Vector3(-length, 0f, 0f); // ���� ��
					positions[1] = new Vector3(length, 0f, 0f);  // ������ ��

					// ���μ� ��ǥ ����
					positions[2] = new Vector3(0f, 0f, 0f);          // ���μ� �߰�
					positions[3] = new Vector3(0f, -length*1.5f, 0f);         // �Ʒ��� ��

					// 'T' ����� �׸��� ���� ���μ��� ���μ��� ����
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
					if (index == 0)
					{
						x = x * -1f;
					}
					return new Vector3(x, y, 0f);
				}
			case Define.ExpressionType.CLOSE:
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
					if (index == 0)
					{
						x = x * -1f;
					}
					return new Vector3(x, y, 0f);
				}
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
					if (index == 0)
					{
						x = x * -1f;
					}
					return new Vector3(x, y, 0f);
				}
		}
		return Vector3.zero;
	}

	public void DrawExpression(int maxHp, int hp)
	{
		ClearExpressionLines();

		switch (expressionType)
		{
			case Define.ExpressionType.CIRCLE:
			case Define.ExpressionType.SMILE:
			case Define.ExpressionType.CRY:
			case Define.ExpressionType.CLOSE:
			case Define.ExpressionType.LINE:
				{
					SetExpressionLine("ExpressionLine0", gradeColor, expressionType, spriteScale, 0);
					SetExpressionLine("ExpressionLine1", gradeColor, expressionType, spriteScale, 1);
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
				line.startWidth = 0.18f;
				line.endWidth = 0.18f;
			}
		}
	}

	public void SetExpressionLineScaleNormal()
	{
		foreach (LineRenderer line in listExpression)
		{
			if (!line.gameObject.IsDestroyed())
			{
				line.startWidth = 0.1f;
				line.endWidth = 0.1f;
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
		expressionType = Define.GetRandomEnumValue<Define.ExpressionType>();
		//expressionType = Define.ExpressionType.LINE;
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
		spriteRenderer.color = Define.GetRGBColor(r, g, b);
	}
}

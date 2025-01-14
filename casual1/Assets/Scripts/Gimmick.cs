using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gimmick : MonoBehaviour
{
    public Define.GimmickType gimmickType;
    public int hp;
    private SpriteRenderer spriteRenderer;
	private GameObject spriteObject;
	private float objectScale = 1f;
	private float spriteScale = 1f;
	private bool isChecked = false;
	private List<GameObject> listGimmick = null;

	private LineRenderer lineRenderer1;
	private LineRenderer lineRenderer2;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake()
    {
		// ���ο� ���� ������Ʈ�� �����ϰ�, ���� ������Ʈ�� �ڽ����� �����մϴ�.
		spriteObject = new GameObject("ChildObject");
		spriteObject.transform.SetParent(this.transform);

		// �ڽ� ������Ʈ�� ��ġ�� �θ�� �����ϰ� �����մϴ�.
		spriteObject.transform.localPosition = Vector3.zero;

		// SpriteRenderer ������Ʈ�� �߰��մϴ�.
		spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetGimmickSprite(int index)
	{
		GimmickDBEntity gimmickInfo = GameManager.instance.GetGimmickInfo(gimmickType);
		// Sprite Load
		Sprite gimmickSprite = null;
		if(index == 1)
		{
			gimmickSprite = GameManager.instance.GetGimmickSprite(gimmickType);
		}
		else if(index == 2)
		{
			gimmickSprite = GameManager.instance.GetGimmickSprite2(gimmickType);
		}
		else if (index == 3)
		{
			gimmickSprite = GameManager.instance.GetGimmickSprite3(gimmickType);
		}

		// ��������Ʈ�� ���������� �ε�Ǿ����� Ȯ���մϴ�.
		if (gimmickSprite != null)
		{
			// SpriteRenderer�� ��������Ʈ�� �����մϴ�.
			spriteRenderer.sprite = gimmickSprite;
		}
		else
		{
			Debug.LogError($"{gimmickType} ��������Ʈ�� �ε��� �� �����ϴ�. ��θ� Ȯ���ϼ���.");
		}

		//spriteRenderer.material = Resources.Load<Material>("Materials/ice");
	}

    public void SetGimmick(Define.GimmickType _type, int _hp, Color _color, int _inputAngle, List<GameObject> _listGimmick, bool _isChecked, GameObject targetCircle)
    {
		this.hp = _hp;
		if (this.hp == 0)
			this.hp = 1;
		this.gimmickType = _type;
		this.listGimmick = _listGimmick;
		this.isChecked = _isChecked;

		SetColor(_color);
		SetGimmickSprite(GetSpriteNoAtInit(_type, _isChecked));
		GimmickDBEntity gimmickInfo = GameManager.instance.GetGimmickInfo(gimmickType);
		// ���ϴ� ũ�⿡ �°� ���� �������� �����մϴ�.
		spriteScale = gimmickInfo.spritescale;
		spriteObject.transform.localScale = new Vector3(spriteScale, spriteScale, 1);

		// �Է� ������ 0~359���� ����
		float angle = (_inputAngle + 270)%360;

		// ���� ȸ�� ���� ������
		Vector3 currentRotation = transform.eulerAngles;

		// ���ο� ȸ�� ���� ���� (Z�� ����)
		currentRotation.z = angle;

		// ������Ʈ�� ȸ�� ����
		spriteObject.transform.eulerAngles = currentRotation;

		// ��� ������ ����
		objectScale = gimmickInfo.objectscale*Define.BASE_GIMMICK_SCALE;
		transform.localScale = new Vector3(objectScale, objectScale, 1);

		//���� �׸���
		DrawGuideLine(targetCircle);
	}

    public void SetColor(Color _color)
    {
		spriteRenderer.color = _color;
	}

	public void SetChecked()
	{
		isChecked = true;
	}

	public void SetUnChecked()
	{
		isChecked = false;
	}

	public bool GetChecked()
	{
		return isChecked;
	}

	public List<GameObject> GetListGimmick()
	{
		return this.listGimmick;
	}

	public int GetSpriteNoAtInit(Define.GimmickType _type, bool _checked)
	{
		switch(_type)
		{
			case Define.GimmickType.ONOFF_ON:
			case Define.GimmickType.ONOFF_OFF:
				{
					if (_checked)
						return 2;
				}
				break;
		}
		return 1;
	}

	void DrawGuideLine(GameObject targetCircle)
	{
		float lineLength = 0.35f; // ������ ����
								 // LineRenderer ������Ʈ �ʱ�ȭ
		lineRenderer1 = CreateLineRenderer("NormalLine1", targetCircle);
		lineRenderer2 = CreateLineRenderer("NormalLine2", targetCircle);

		Vector3 bigCenter = targetCircle.transform.position;
		Vector3 smallCenter = transform.position;

		// TargetCircle�� �߽ɿ��� SmallCircle�� �߽������� ����
		Vector3 direction = (smallCenter - bigCenter).normalized;

		// SmallCircle�� ������ (����: SpriteRenderer�� bounds�� ���)
		float smallRadius = GetComponent<SpriteRenderer>().bounds.extents.x*1.20f;
		Debug.Log($"DrawGuideLine smallRadius : {smallRadius}");

		// ���� ���
		Vector3 tangentPoint1 = smallCenter + Quaternion.Euler(0, 0, 90) * direction * smallRadius;
		Vector3 tangentPoint2 = smallCenter + Quaternion.Euler(0, 0, -90) * direction * smallRadius;

		// ���� ���� ���
		Vector3 normalEndPoint1 = tangentPoint1 + direction * lineLength;
		Vector3 normalEndPoint2 = tangentPoint2 + direction * lineLength;

		// LineRenderer�� ����Ͽ� �� �׸���
		lineRenderer1.SetPosition(0, bigCenter);
		lineRenderer1.SetPosition(1, normalEndPoint1);

		lineRenderer2.SetPosition(0, bigCenter);
		lineRenderer2.SetPosition(1, normalEndPoint2);

		lineRenderer1.sortingOrder = -10;
		lineRenderer2.sortingOrder = -10;
	}

	LineRenderer CreateLineRenderer(string name, GameObject targetCircle)
	{
		GameObject lineObj = new GameObject(name);
		lineObj.transform.parent = targetCircle.transform;
		LineRenderer lr = lineObj.AddComponent<LineRenderer>();
		lr.startWidth = 0.03f;
		lr.endWidth = 0.03f;
		lr.material = new Material(Shader.Find("Sprites/Default"));
		lr.positionCount = 2;
		lr.startColor = Define.COLOR_BLACK;
		lr.endColor = Define.COLOR_BLACK_ALPHA10;
		lr.numCapVertices = 10; // ���κ��� �ձ۰� ����� ���� �߰��� ���ؽ� ��
		lr.useWorldSpace = false;
		//lineObj.transform.SetParent(targetCircle.transform);
		return lr;
	}

	public void DisableGuideLine()
	{
		if(lineRenderer1 != null)
			lineRenderer1.enabled = false;
		if (lineRenderer2 != null)
			lineRenderer2.enabled = false;
	}
}

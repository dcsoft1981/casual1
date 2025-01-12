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

    public void SetGimmick(Define.GimmickType _type, int _hp, Color _color, int _inputAngle, List<GameObject> _listGimmick, bool _isChecked)
    {
		this.hp = _hp;
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
}

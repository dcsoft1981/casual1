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

    public void SetGimmick(Define.GimmickType _type, int _hp, Color _color, int inputAngle)
    {
		this.hp = _hp;
		this.gimmickType = _type;
        spriteRenderer.color = _color;

		GimmickDBEntity gimmickInfo = GameManager.instance.GetGimmickInfo(_type);
		// Sprite Load
		Sprite heartSprite = GameManager.instance.GetGimmickSprite(_type);

		// ��������Ʈ�� ���������� �ε�Ǿ����� Ȯ���մϴ�.
		if (heartSprite != null)
		{
			// SpriteRenderer�� ��������Ʈ�� �����մϴ�.
			spriteRenderer.sprite = heartSprite;
		}
		else
		{
			Debug.LogError($"{_type} ��������Ʈ�� �ε��� �� �����ϴ�. ��θ� Ȯ���ϼ���.");
		}

		// ���ϴ� ũ�⿡ �°� ���� �������� �����մϴ�.
		spriteScale = gimmickInfo.spritescale;
		spriteObject.transform.localScale = new Vector3(spriteScale, spriteScale, 1);

		// �Է� ������ 0~359���� ����
		float angle = (inputAngle+270)%360;

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
}

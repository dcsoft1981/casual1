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
		// 새로운 게임 오브젝트를 생성하고, 현재 오브젝트의 자식으로 설정합니다.
		spriteObject = new GameObject("ChildObject");
		spriteObject.transform.SetParent(this.transform);

		// 자식 오브젝트의 위치를 부모와 동일하게 설정합니다.
		spriteObject.transform.localPosition = Vector3.zero;

		// SpriteRenderer 컴포넌트를 추가합니다.
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

		// 스프라이트가 성공적으로 로드되었는지 확인합니다.
		if (heartSprite != null)
		{
			// SpriteRenderer의 스프라이트를 설정합니다.
			spriteRenderer.sprite = heartSprite;
		}
		else
		{
			Debug.LogError($"{_type} 스프라이트를 로드할 수 없습니다. 경로를 확인하세요.");
		}

		// 원하는 크기에 맞게 로컬 스케일을 조정합니다.
		spriteScale = gimmickInfo.spritescale;
		spriteObject.transform.localScale = new Vector3(spriteScale, spriteScale, 1);

		// 입력 각도를 0~359도로 제한
		float angle = (inputAngle+270)%360;

		// 현재 회전 값을 가져옴
		Vector3 currentRotation = transform.eulerAngles;

		// 새로운 회전 값을 설정 (Z축 기준)
		currentRotation.z = angle;

		// 오브젝트의 회전 적용
		spriteObject.transform.eulerAngles = currentRotation;

		// 기믹 스케일 변경
		objectScale = gimmickInfo.objectscale*Define.BASE_GIMMICK_SCALE;
		transform.localScale = new Vector3(objectScale, objectScale, 1);
	}

    public void SetColor(Color _color)
    {
		spriteRenderer.color = _color;
	}
}

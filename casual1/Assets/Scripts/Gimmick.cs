using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Define;

public class Gimmick : MonoBehaviour
{
    public Define.GimmickType gimmickType;
    public int hp;
    private SpriteRenderer spriteRenderer;
	private CircleCollider2D circleCollider;
	private GameObject spriteObject;
	private float objectScale = 1f;
	private float spriteScale = 1f;
	private bool isChecked = false;
	private List<GameObject> listGimmick = null;

	private LineRenderer lineRenderer1;
	private LineRenderer lineRenderer2;
	private List<int> listWorkedPin = null;
	public ParticleSystem effect = null;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake()
    {
		// 새로운 게임 오브젝트를 생성하고, 현재 오브젝트의 자식으로 설정합니다.
		spriteObject = new GameObject(Define.CHILD_SPRITE_OBJECT);
		spriteObject.transform.SetParent(this.transform);

		// 자식 오브젝트의 위치를 부모와 동일하게 설정합니다.
		spriteObject.transform.localPosition = Vector3.zero;

		// SpriteRenderer 컴포넌트를 추가합니다.
		spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
		circleCollider = GetComponent<CircleCollider2D>();
		listWorkedPin = new List<int>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetGimmickSprite(int index)
	{
		GimmickDBEntity gimmickInfo = LocalDataManager.instance.GetGimmickInfo(gimmickType);
		// Sprite Load
		Sprite gimmickSprite = null;
		if(index == 1)
		{
			gimmickSprite = LocalDataManager.instance.GetGimmickSprite(gimmickType);
		}
		else if(index == 2)
		{
			gimmickSprite = LocalDataManager.instance.GetGimmickSprite2(gimmickType);
		}
		else if (index == 3)
		{
			gimmickSprite = LocalDataManager.instance.GetGimmickSprite3(gimmickType);
		}

		// 스프라이트가 성공적으로 로드되었는지 확인합니다.
		if (gimmickSprite != null)
		{
			// SpriteRenderer의 스프라이트를 설정합니다.
			spriteRenderer.sprite = gimmickSprite;
		}
		else
		{
			Debug.LogError($"{gimmickType} 스프라이트를 로드할 수 없습니다. 경로를 확인하세요.");
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
		GimmickDBEntity gimmickInfo = LocalDataManager.instance.GetGimmickInfo(gimmickType);
		// 원하는 크기에 맞게 로컬 스케일을 조정합니다.
		spriteScale = gimmickInfo.spritescale;
		spriteObject.transform.localScale = new Vector3(spriteScale, spriteScale, 1);

		// 입력 각도를 0~359도로 제한
		float angle = (_inputAngle + 270)%360;

		// 현재 회전 값을 가져옴
		Vector3 currentRotation = transform.eulerAngles;

		// 새로운 회전 값을 설정 (Z축 기준)
		currentRotation.z = angle;

		// 오브젝트의 회전 적용
		spriteObject.transform.eulerAngles = currentRotation;

		// 기믹 스케일 변경
		objectScale = gimmickInfo.objectscale*Define.BASE_GIMMICK_SCALE;
		transform.localScale = new Vector3(objectScale, objectScale, 1);

		// 타겟 히트형 기믹 충돌영역 살짝 늘리기
		GimmickCathegory cathegory = Define.GetGimmickCathegory(_type);
		if(cathegory == GimmickCathegory.TargetHit)
		{
			//circleCollider.radius = 0.52f;
		}

		//가이드라인 그리기
		if (!LocalDataManager.instance.GetGuideLineOff())
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
		float lineLength = 0.35f; // 법선의 길이
								 // LineRenderer 컴포넌트 초기화
		lineRenderer1 = CreateLineRenderer("NormalLine1", targetCircle);
		lineRenderer2 = CreateLineRenderer("NormalLine2", targetCircle);

		Vector3 bigCenter = targetCircle.transform.position;
		Vector3 smallCenter = transform.position;

		// TargetCircle의 중심에서 SmallCircle의 중심으로의 벡터
		Vector3 direction = (smallCenter - bigCenter).normalized;

		// SmallCircle의 반지름 (가정: SpriteRenderer의 bounds를 사용)
		float smallRadius = GetComponent<SpriteRenderer>().bounds.extents.x*1.20f;
		Debug.Log($"DrawGuideLine smallRadius : {smallRadius}");

		// 접점 계산
		Vector3 tangentPoint1 = smallCenter + Quaternion.Euler(0, 0, 90) * direction * smallRadius;
		Vector3 tangentPoint2 = smallCenter + Quaternion.Euler(0, 0, -90) * direction * smallRadius;

		// 법선 끝점 계산
		Vector3 normalEndPoint1 = tangentPoint1 + direction * lineLength;
		Vector3 normalEndPoint2 = tangentPoint2 + direction * lineLength;

		// LineRenderer를 사용하여 선 그리기
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
		lr.numCapVertices = 10; // 끝부분을 둥글게 만들기 위해 추가할 버텍스 수
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

	public bool PinWork(int _pinID)
	{
		// 이미 처리한 핀인지 확인
		foreach(int pinID in listWorkedPin)
		{
			if (pinID == _pinID) return false;
		}
		// 처리한 핀에 추가
		listWorkedPin.Add(_pinID);
		return true;
	}

	public void EffectPlay()
	{
		float scale = 0.05f;
		effect.transform.position = transform.position;
		effect.transform.localScale = new Vector3(scale, scale, scale);
		Debug.Log("EffectPlay Gimmick Scale : " + effect.transform.localScale);
		effect.Play();
	}
}

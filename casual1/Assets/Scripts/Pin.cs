using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Pin : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 10f;
	[SerializeField] private float rotateSpeed = 10f;

	private bool isPinned = false;
	private bool isLaunched = false;
	private bool isReflecteded = false;
	private Vector3 reflectVec = Vector3.zero;
	private bool isUpgraded = false;
	private bool isWorked = false;
	//private SpriteRenderer spriteRenderer;
	private float reflectRotateSpeed = 0f;
	private int pinId = 0;
	public ParticleSystem effect = null;
	private GameObject spriteObject;
	private Vector3 spriteOriginalScale;
	private Tweener scaleTween;
	private GameObject objConnector;
	//private GameObject objJoy;
	private bool connectorAlive = false;
	private float rotationSpeed = 100f;

	private void Awake()
	{
		//spriteRenderer = GetComponent<SpriteRenderer>();
		spriteObject = gameObject.transform.Find(Define.CHILD_SPRITE_OBJECT).gameObject;
		spriteOriginalScale = spriteObject.transform.localScale;
		objConnector = spriteObject.transform.Find("Connector").gameObject;
		//objJoy = spriteObject.transform.Find("Joy").gameObject;
	}
	void Start()
    {
		//SpriteRenderer objJoySprite = objJoy.GetComponent<SpriteRenderer>();
		//objJoySprite.color = LocalDataManager.instance.GetCurColor();
	}

	// Update is called once per frame
	void FixedUpdate()
    {
        if (isPinned == false)
        {
			if(isLaunched)
			{
				if (isReflecteded)
				{
					transform.position += reflectVec * moveSpeed * Time.deltaTime;
					// 회전
					transform.Rotate(0, 0, reflectRotateSpeed * Time.deltaTime);
				}
				else
				{
					transform.position += Vector3.up * moveSpeed * Time.deltaTime;
				}
			}
		}

		GameManager.instance.CheckUpgradePin(false);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// 이미 결과처리된(고정된, 반사된) 발사체는 아무 동작을 하지 않는다.
		if (isPinned || isReflecteded)
			return;

		bool listGimmickWork = true;
		if(collision.gameObject.tag == "Target")
		{
			if(GameManager.instance.IsInShield())
			{
				// 데미지 없음
				AudioManager.instance.PlaySfx(AudioManager.Sfx.iron_hit);
				ReflectPin(collision);
				GameManager.instance.ResetCombo();
				LogManager.Log("OnTriggerEnter2D Target InShield");
			}
			else
			{
				// 접착된 지점의 각도
				float angle = collision.gameObject.transform.rotation.eulerAngles.z;
				if (angle < 0f)
				{
					angle = 360f - angle;
				}
				angle = 360f - angle;
				if (angle >= 90f)
					angle -= 90f;
				else
					angle += 270f;
				int damage = GameManager.instance.GetHpAmountByTargetAngle((int)angle, isUpgraded); // 데미지 영역 계산
				if(damage == 0)
				{
					AudioManager.instance.PlaySfx(AudioManager.Sfx.iron_hit);
					ReflectPin(collision);
					GameManager.instance.ResetCombo();
				}
				else
				{
					isPinned = true;
					ConnectorSetting(false);
					// 위치 조정
					transform.position = GameManager.instance.GetTargetPinnedPosition();

					// 부착
					transform.SetParent(collision.gameObject.transform);

					GameManager.instance.DecreaseHP(damage);
					GameManager.instance.AddPinnedShot(this.gameObject);
					GameManager.instance.AddCombo(transform.position);

					LogManager.Log("OnTriggerEnter2D Target HIT Angle : " + angle + " , DAMAGE : " + damage + " , Position : " + transform.position);
				}
			}
			this.SetDefaultSpriteScale();
			isWorked = true;
		}
		else if(collision.gameObject.tag == "Pin")
		{
			Pin pin = collision.gameObject.GetComponent<Pin>();
			if(pin == null)
			{
				LogManager.LogError("OnTriggerEnter2D No Pin");
				return;
			}

			if(pin.GetPinned())
			{
				// 이미 고정된 핀
				GameManager.instance.ResetCombo();
				AudioManager.instance.PlaySfx(AudioManager.Sfx.shot_failure);
				ReflectPin(collision);
				LogManager.Log("OnTriggerEnter2D to Pinned -> ReflectPin");
				isWorked = true;
			}
			else
			{
				// 고정되지 않은 핀(생성되거나 움직이는핀)과의 충돌은 무시한다.
				return;
			}
		}
		else if (collision.gameObject.tag == "Gimmick")
		{
			Define.ShotGimmickHitResult shotGimmickHitResult = GameManager.instance.ShotGimmickHit(this, collision.gameObject);
			LogManager.Log("OnTriggerEnter2D Gimmick : " + this.GetPinID() + " , " + shotGimmickHitResult);
			switch (shotGimmickHitResult)
			{
				case Define.ShotGimmickHitResult.NONE:
					{
						return;
					}
				case Define.ShotGimmickHitResult.ALEADY_COMPLETED:
					{
						return;
					}
				case Define.ShotGimmickHitResult.HIT_THROUTH:
					{
						//GameManager.instance.ResetCombo();
						listGimmickWork = false; // 타겟 히트할떼 동작함
					}
					break;
				case Define.ShotGimmickHitResult.HIT_REFLECT:
					{
						AudioManager.instance.PlaySfx(AudioManager.Sfx.shot_gimmick);
						ReflectPin(collision);
						//DisapearPin();
						GameManager.instance.ResetCombo();
					}
					break;
				case Define.ShotGimmickHitResult.HIT_PAIR_REFLECT:
					{
						AudioManager.instance.PlaySfx(AudioManager.Sfx.shot_gimmick);
						ReflectPin(collision);
						//DisapearPin();
						GameManager.instance.ResetCombo();
					}
					break;
				case Define.ShotGimmickHitResult.HIT_IRON_REFLECT:
					{
						AudioManager.instance.PlaySfx(AudioManager.Sfx.iron_hit);
						ReflectPin(collision);
						GameManager.instance.ResetCombo();
					}
					break;
			}
			isWorked = true;
		}
		else
		{
			GameManager.instance.ResetCombo();
			LogManager.Log("OnTriggerEnter2D STRANGE NO WORK");
		}

		// 모든 기믹들 상태 변환 체크
		if(listGimmickWork)
		{
			GameManager.instance.CheckListGimmickStatus();
		}
	}

	public void Launch()
	{
		isLaunched = true;
	}

	private void DisapearPin()
	{
		isReflecteded = true;
		Invoke("DestroyPin", 0.3f);
	}

	private void ReflectPin(Collider2D collision)
	{
		Pin collisionPin = collision.gameObject.GetComponent<Pin>();
		if(collisionPin != null)
		{
			if(collisionPin.isReflected())
			{
				// 반사 시키지 않는다.
				LogManager.Log("튕기지 않음");
				return;
			}
		}
		// 스파라이트 스케일 줄이기
		DisappearScaleChange();

		Vector3 colliderPos = collision.transform.position;
		reflectVec = (collision.transform.position - this.transform.position).normalized;
		reflectVec = Vector3.Reflect(Vector3.down, reflectVec);
		float reflectX = -reflectVec.x;
		if(reflectX > 0)
		{
			if (reflectX < 0.01f)
			{
				reflectX = 0.01f;
			}
			else if (reflectX < 0.3f)
			{
				reflectX = 0.3f;
			}
			else if (reflectX > 0.8f)
			{
				reflectX = 0.8f;
			}
		}
		else
		{
			if (reflectX > -0.01f)
			{
				reflectX = -0.01f;
			}
			else if (reflectX > -0.3f)
			{
				reflectX = -0.3f;
			}
			else if (reflectX < -0.8f)
			{
				reflectX = -0.8f;
			}
		}

		reflectVec = new Vector3(reflectX, -1f, 0f);
		reflectRotateSpeed = reflectX*rotateSpeed*20;
		LogManager.Log($"튕김 방향: {reflectVec} , {reflectRotateSpeed}");

		isReflecteded = true;
		Invoke("DestroyPin", 0.3f);
	}

	public void StopAni()
	{
		spriteObject.transform.DOKill();
		ConnectorSetting(false);
	}

	private void DestroyPin()
	{
		StopAni();
		Destroy(this.gameObject);
	}

	public bool isReflected()
	{
		return isReflecteded;
	}

	public bool GetPinned()
	{
		return isPinned;
	}

	public bool isAbleUpgrade()
	{
		if(!isPinned && !isLaunched && !isReflecteded)
		{
			return true;
		}

		return false;
	}

	public void Upgrade()
	{
		isUpgraded = true;
		transform.Find(Define.CHILD_SPRITE_OBJECT).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
	}

	public int GetPinID()
	{
		return pinId;
	}

	public void SetPinId(int id)
	{
		pinId = id;
	}

	public bool GetWorked()
	{
		return isWorked;
	}

	public void EffectPlay()
	{
		float scale = 0.035f;
		effect.transform.position = transform.position;
		effect.transform.localScale = new Vector3(scale, scale, scale);
		LogManager.Log("EffectPlay Pin Scale : " + effect.transform.localScale);
		effect.Play();
	}

	public void ScaleShake()
	{
		if (spriteObject.IsDestroyed())
			return;
		spriteObject.transform.localScale = spriteOriginalScale;
		scaleTween = spriteObject.transform.DOScale(spriteOriginalScale * 2, 0.1f).SetLoops(2, LoopType.Yoyo);
	}

	public void CreateScaleChange()
	{
		if (spriteObject.IsDestroyed())
			return;
		spriteObject.transform.localScale = Vector3.zero;
		scaleTween = spriteObject.transform.DOScale(spriteOriginalScale, 0.1f).SetEase(Ease.OutCirc).OnComplete(() =>
		{
			spriteObject.transform.localScale = spriteOriginalScale;
		});
	}

	public void DisappearScaleChange()
	{
		if (spriteObject.IsDestroyed())
			return;
		spriteObject.transform.localScale = spriteOriginalScale;
		spriteObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutCirc);
	}

	public void SetDefaultSpriteScale()
	{
		if (scaleTween != null)
		{
			scaleTween.Kill();
			scaleTween = null;
		}
		spriteObject.transform.localScale = spriteOriginalScale;
	}

	public void ConnectorSetting(bool alive)
	{
		if (spriteObject.IsDestroyed())
			return;
		objConnector.SetActive(alive);
		connectorAlive = alive;
		SpriteRenderer objConnectorSprite = objConnector.GetComponent<SpriteRenderer>();
		//SpriteRenderer objJoySprite = objJoy.GetComponent<SpriteRenderer>();
		if (alive)
		{
			objConnectorSprite.DOFade(0.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
			//objJoySprite.DOFade(0.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
		}
		else
		{
			objConnectorSprite.DOKill();
			objConnector.SetActive(false);
			/*
			objJoySprite.DOKill();
			Color currentColor = objJoySprite.color;
			currentColor.a = 1f;
			objJoySprite.color = currentColor;
			*/
		}
	}
}
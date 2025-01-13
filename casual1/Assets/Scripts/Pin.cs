using DG.Tweening;
using Microsoft.SqlServer.Server;
using UnityEngine;

public class Pin : MonoBehaviour
{
	[SerializeField]
	private float moveSpeed = 10f;

	private bool isPinned = false;
	private bool isLaunched = false;
	private bool isReflecteded = false;
	private Vector3 reflectVec = Vector3.zero;
	private bool isUpgraded = false;
	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	void Start()
    {
        
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
				}
				else
				{
					transform.position += Vector3.up * moveSpeed * Time.deltaTime;
				}
			}
		}

		GameManager.instance.CheckUpgradePin();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// 이미 결과처리된(고정된, 반사된) 발사체는 아무 동작을 하지 않는다.
		if (isPinned || isReflecteded)
			return;

		if(collision.gameObject.tag == "Target")
		{
			if(GameManager.instance.IsInShield())
			{
				// 데미지 없음
				AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_failure);
				ReflectPin(collision);
				GameManager.instance.ResetCombo();
				Debug.Log("OnTriggerEnter2D Target InShield");
			}
			else
			{
				isPinned = true;
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

				GameObject childObject = transform.Find("Square").gameObject;
				//GameObject childObject = transform.GetChild(0).gameObject;
				//SpriteRenderer childSprite = childObject.GetComponent<SpriteRenderer>();
				//childSprite.enabled = true;
				// 부착
				transform.SetParent(collision.gameObject.transform);
				AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_good);
				int damage = GameManager.instance.GetHpAmountByTargetAngle((int)angle, isUpgraded);
				GameManager.instance.DecreaseHP(damage);
				GameManager.instance.AddPinnedShot(this.gameObject);
				GameManager.instance.AddCombo();

				Debug.Log("OnTriggerEnter2D Target HIT Angle : " + angle + " , DAMAGE : " + damage);
			}
		}
		else if(collision.gameObject.tag == "Pin")
		{
			Pin pin = collision.gameObject.GetComponent<Pin>();
			if(pin == null)
			{
				Debug.LogError("OnTriggerEnter2D No Pin");
				return;
			}

			if(pin.GetPinned())
			{
				// 이미 고정된 핀
				GameManager.instance.ResetCombo();
				AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_failure);
				ReflectPin(collision);
				Debug.Log("OnTriggerEnter2D to Pinned -> ReflectPin");
			}
			else
			{
				// 고정되지 않은 핀(생성되거나 움직이는핀)과의 충돌은 무시한다.
				return;
			}
		}
		else if (collision.gameObject.tag == "Gimmick")
		{
			AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_good);
			bool destroyPin = GameManager.instance.GimmickHitWork(collision.gameObject);
			if (destroyPin)
			{
				ReflectPin(collision);
				Debug.Log("OnTriggerEnter2D Gimmick ReflectPin");
			}
			else
			{
				Debug.Log("OnTriggerEnter2D Gimmick Hit");
			}
			GameManager.instance.ResetCombo();
		}
		else
		{
			GameManager.instance.ResetCombo();
			Debug.Log("OnTriggerEnter2D STRANGE NO WORK");
		}

		GameManager.instance.CheckListGimmickStatus();
	}

	public void Launch()
	{
		isLaunched = true;
	}

	private void ReflectPin(Collider2D collision)
	{
		Pin collisionPin = collision.gameObject.GetComponent<Pin>();
		if(collisionPin != null)
		{
			if(collisionPin.isReflected())
			{
				// 반사 시키지 않는다.
				Debug.Log("튕기지 않음");
				return;
			}
		}

		Vector3 colliderPos = collision.transform.position;
		reflectVec = (collision.transform.position - this.transform.position).normalized;
		reflectVec = Vector3.Reflect(Vector3.down, reflectVec);
		float reflectX = reflectVec.x;
		if(reflectX > 0)
		{
			if (reflectX < 0.01f)
				reflectX = 0.01f;
			else if (reflectX > 0.6f)
				reflectX = 0.6f;
		}
		else
		{
			if (reflectX > -0.01f)
				reflectX = -0.01f;
			else if (reflectX < -0.3f)
				reflectX = -0.3f;
		}

		reflectVec = new Vector3(reflectX, -1f, 0f);
		Debug.Log($"튕김 방향: {reflectVec}");

		isReflecteded = true;
		Invoke("DestroyPin", 0.3f);
	}

	private void DestroyPin()
	{
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
		spriteRenderer.color = Color.red;
	}
}
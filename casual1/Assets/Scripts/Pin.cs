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
	// Start is called once before the first execution of Update after the MonoBehaviour is created
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
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		bool checkGimmickStatus = true;
		if(collision.gameObject.tag == "Target")
		{
			if(GameManager.instance.IsInShield())
			{
				// 데미지 없음
				AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_failure);
				ReflectPin(collision);
				Debug.Log("OnTriggerEnter2D Target InShield");
			}
			else
			{
				isPinned = true;
				GameObject childObject = transform.Find("Square").gameObject;
				//GameObject childObject = transform.GetChild(0).gameObject;
				//SpriteRenderer childSprite = childObject.GetComponent<SpriteRenderer>();
				//childSprite.enabled = true;
				// 부착
				transform.SetParent(collision.gameObject.transform);
				AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_good);
				GameManager.instance.DecreaseHP();
				GameManager.instance.AddPinnedShot(this.gameObject);
				Debug.Log("OnTriggerEnter2D Target HIT");
			}
		}
		else if(collision.gameObject.tag == "Pin")
		{
			if(isPinned)
			{
				// 이미 고정된 핀
				Debug.Log("OnTriggerEnter2D Pin Pinned");
			}
			else
			{
				//AudioManager.instance.StopBgm();
				AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_failure);
				//GameManager.instance.SetGameOver(false);
				ReflectPin(collision);
				Debug.Log("OnTriggerEnter2D Pin ReflectPin");
				checkGimmickStatus = false; // 움직이거나, 새로 생성된 발사체와 충돌시 체크하지 않는다.
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
		}
		else
		{
			Debug.Log("OnTriggerEnter2D STRANGE NO WORK");
		}
		
		if(checkGimmickStatus)
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
}
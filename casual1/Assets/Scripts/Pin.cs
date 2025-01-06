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
		if(collision.gameObject.tag == "Target")
		{
			isPinned = true;
			GameObject childObject = transform.Find("Square").gameObject;
			//GameObject childObject = transform.GetChild(0).gameObject;
			//SpriteRenderer childSprite = childObject.GetComponent<SpriteRenderer>();
			//childSprite.enabled = true;
			// ∫Œ¬¯
			transform.SetParent(collision.gameObject.transform);
			AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_good);
			GameManager.instance.DecreaseHP();
		}
		else if(collision.gameObject.tag == "Pin")
		{
			if(isPinned)
			{
				// ¿ÃπÃ ∞Ì¡§µ» «…
			}
			else
			{
				//AudioManager.instance.StopBgm();
				AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_failure);
				//GameManager.instance.SetGameOver(false);
				ReflectPin(collision);
			}
			Debug.Log("OnTriggerEnter2D : " + isPinned);
		}
		else if (collision.gameObject.tag == "Gimmick")
		{
			AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_good);
			bool destroyPin = GameManager.instance.GimmickHitWork(collision.gameObject);
			if (destroyPin)
			{
				ReflectPin(collision);
			}
		}
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
				// π›ªÁ Ω√≈∞¡ˆ æ ¥¬¥Ÿ.
				Debug.Log("∆®±‚¡ˆ æ ¿Ω");
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
		Debug.Log($"∆®±Ë πÊ«‚: {reflectVec}");

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
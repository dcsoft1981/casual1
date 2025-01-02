using UnityEngine;

public class Pin : MonoBehaviour
{
	[SerializeField]
	private float moveSpeed = 10f;

	private bool isPinned = false;
	private bool isLaunched = false;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPinned == false && isLaunched == true)
        {
			transform.position += Vector3.up * moveSpeed * Time.deltaTime;
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
				Destroy(this.gameObject);
			}
			Debug.Log("OnTriggerEnter2D : " + isPinned);
		}
		else if (collision.gameObject.tag == "Gimmick")
		{
			AudioManager.instance.PlaySfx(AudioManager.Sfx.shoot_good);
			bool destroyPin = GameManager.instance.GimmickHitWork(collision.gameObject);
			if(destroyPin)
				Destroy(this.gameObject);
		}
	}

	public void Launch()
	{
		isLaunched = true;
	}
}
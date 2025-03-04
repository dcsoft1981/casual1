using System.Runtime.CompilerServices;
using UnityEngine;

public class Background : MonoBehaviour
{
    private float offsetX = 5.5f;
	private float offsetY = 4.5f;

	public float speed = 1f;
	private Vector3 targetPosition;

	void Start()
    {
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = LocalDataManager.instance.GetBG();
		// 시작 위치를 랜덤하게 설정
		float startX = Random.Range(-offsetX, offsetX);
		float startY = Random.Range(-offsetY, offsetY);
		transform.position = new Vector3(startX, startY, 0f);

		// 첫 번째 목표 지점 설정
		SetNewRandomTarget();
	}

    // Update is called once per frame
    void Update()
    {
		MoveToTarget();
	}

	void SetNewRandomTarget()
	{
		float randomX = Random.Range(-offsetX, offsetX);
		float randomY = Random.Range(-offsetY, offsetY);
		targetPosition = new Vector3(randomX, randomY, 0f);
	}

	void MoveToTarget()
	{
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

		if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
		{
			SetNewRandomTarget();
		}
	}
}

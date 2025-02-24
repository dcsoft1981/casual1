using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class TempPin : MonoBehaviour
{
	private Transform targetPosition;
	private SpriteRenderer spriteRenderer;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	public void SetData(Transform _targetPosition)
	{
		if (spriteRenderer.gameObject.IsDestroyed())
			return;
		float blinkDuration = UnityEngine.Random.Range(0.5f, 2f);
		spriteRenderer.DOFade(0f, blinkDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
		targetPosition = _targetPosition;
	}

	public void SetMove()
	{

		// 목표 위치로 이동 (1초 동안 선형 이동)
		transform.DOMove(targetPosition.position, 0.08f)
				 .SetEase(Ease.Linear).OnComplete(() => { StopAni(); this.gameObject.SetActive(false); });
	}

	public void StopAni()
	{
		transform.DOKill();
		if (spriteRenderer.gameObject.IsDestroyed())
			return;
		spriteRenderer.DOKill();
	}
}

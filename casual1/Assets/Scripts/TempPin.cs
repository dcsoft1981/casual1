using UnityEditor;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class TempPin : MonoBehaviour
{
	private Transform targetPosition;
	private Sequence jumpSequence;
	private SpriteRenderer spriteRenderer;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	public void SetData(Transform _targetPosition)
	{
		/*
		float jump = UnityEngine.Random.Range(0.05f,0.2f);
		float jumpTime = UnityEngine.Random.Range(0.3f, 0.5f);
		// 제자리 점프 애니메이션 생성 (Y축으로 0.5 만큼 상승 후 하강)
		jumpSequence = DOTween.Sequence();
		jumpSequence.Append(transform.DOMoveY(transform.position.y + jump, jumpTime)
									 .SetEase(Ease.OutQuad));
		jumpSequence.Append(transform.DOMoveY(transform.position.y, jumpTime)
									 .SetEase(Ease.InQuad));
		jumpSequence.SetLoops(-1); // 무한 반복
		*/

		float blinkDuration = UnityEngine.Random.Range(0.5f, 2f);
		spriteRenderer.DOFade(0f, blinkDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
		targetPosition = _targetPosition;
	}

	public void SetMove()
	{
		// 점프 애니메이션 정지
		jumpSequence.Kill();

		// 목표 위치로 이동 (1초 동안 선형 이동)
		transform.DOMove(targetPosition.position, 0.08f)
				 .SetEase(Ease.Linear).OnComplete(() => this.gameObject.SetActive(false));
	}
}

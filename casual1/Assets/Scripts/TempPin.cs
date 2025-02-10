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
		// ���ڸ� ���� �ִϸ��̼� ���� (Y������ 0.5 ��ŭ ��� �� �ϰ�)
		jumpSequence = DOTween.Sequence();
		jumpSequence.Append(transform.DOMoveY(transform.position.y + jump, jumpTime)
									 .SetEase(Ease.OutQuad));
		jumpSequence.Append(transform.DOMoveY(transform.position.y, jumpTime)
									 .SetEase(Ease.InQuad));
		jumpSequence.SetLoops(-1); // ���� �ݺ�
		*/

		float blinkDuration = UnityEngine.Random.Range(0.5f, 2f);
		spriteRenderer.DOFade(0f, blinkDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
		targetPosition = _targetPosition;
	}

	public void SetMove()
	{
		// ���� �ִϸ��̼� ����
		jumpSequence.Kill();

		// ��ǥ ��ġ�� �̵� (1�� ���� ���� �̵�)
		transform.DOMove(targetPosition.position, 0.08f)
				 .SetEase(Ease.Linear).OnComplete(() => this.gameObject.SetActive(false));
	}
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Mark : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private GameObject imageObj;
	private Image image;
	private RectTransform imageRectTransform;

	private void Awake()
	{
		imageObj = transform.Find("Image").gameObject;
		image = imageObj.GetComponent<Image>();		
	}
	
	public void SetData(GradeDBEntity entity)
	{
		Vector3 position = transform.localPosition;
		Color color = LocalDataManager.instance.GetGradeColor(entity.id);
		image.color = color;
		imageRectTransform = imageObj.GetComponent<RectTransform>();
		//image.DOFade(0f, 1.3f).SetLoops(-1, LoopType.Yoyo);

		float useTime = 0.5f - entity.id * 0.02f;
		// ������Ʈ�� ���� ��ġ�� �������� ���� �ִϸ��̼� ����
		imageRectTransform.DOAnchorPosY(
			endValue: imageRectTransform.anchoredPosition.y + 100f, // ���� ����
			duration: useTime                                     // ������ �ҿ�Ǵ� �ð�
		)
		.SetLoops(-1, LoopType.Yoyo)   // ���� �ݺ� ����
		.SetEase(Ease.OutQuad);         // ������ �ӵ��� �ִϸ��̼� ����
	}

	public void StopJump()
	{
		imageRectTransform.DOKill();
	}
}

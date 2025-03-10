using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Mark : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private GameObject imageObj;
	private Image image;
	private RectTransform imageRectTransform;
	//private RectTransform joyRectTransform;
	//private CanvasGroup joyCanvasGroup;

	private void Awake()
	{
		imageObj = transform.Find("Image").gameObject;
		image = imageObj.GetComponent<Image>();
		imageRectTransform = imageObj.GetComponent<RectTransform>();
		/*
		GameObject joyObj = imageObj.transform.Find("Joy").gameObject;
		Image imageJoy = joyObj.GetComponent<Image>();
		joyRectTransform = imageJoy.GetComponent<RectTransform>();
		joyCanvasGroup = joyObj.AddComponent<CanvasGroup>();
		*/
	}
	
	public void SetData(GradeDBEntity entity)
	{
		Vector3 position = transform.localPosition;
		Color color = LocalDataManager.instance.GetGradeColor(entity.id);
		image.color = color;
		//image.DOFade(0f, 1.3f).SetLoops(-1, LoopType.Yoyo);

		float useTime = 0.5f - entity.id * 0.02f;
		// 오브젝트의 현재 위치를 기준으로 점프 애니메이션 설정
		imageRectTransform.DOAnchorPosY(
			endValue: imageRectTransform.anchoredPosition.y + 100f, // 점프 높이
			duration: useTime                                     // 점프에 소요되는 시간
		)
		.SetLoops(-1, LoopType.Yoyo)   // 무한 반복 설정
		.SetEase(Ease.OutQuad);         // 일정한 속도로 애니메이션 진행

		//joyCanvasGroup.DOFade(0.2f, useTime).SetLoops(-1, LoopType.Yoyo);
	}

	public void StopJump()
	{
		imageRectTransform.DOKill();
		//joyCanvasGroup.DOKill();
	}
}

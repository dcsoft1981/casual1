using UnityEngine;

public class Define
{
	public const string CUR_LEVEL = "CUR_LEVEL";
	public const string LOCALE_RETRY = "RETRY";
	public const string LOCALE_NEXT = "NEXT";

	//public const int MAX_LEVEL_KEYFRAME_COUNT = 5; // 회전값 복수개 사용시 사용 현재 미사용

	public const int ROTATE_SEC = 10;
	public const float TARGET_BASE_SCALE = 3f;

	// 타겟
	// 10000 자리 값 : 외형. 일단 색으로 구분(1-GREEN, 2-BLUE,3-RED)
	// 1 자리 값 : 반지름 사이즈% 
	// ex) 10100 - BLACK 100% 사이즈 , 10050 - BLACK 50% 사이즈 , 30050 - RED 50% 사이즈
	public enum TargetType
	{
		None = 0,
		BLACK = 10000,
		BLUE = 20000,
		RED = 30000,
	}

	// 기믹
	// 10000 자리 값 : 형( 1 - 기믹히트형, 2 - 타겟히트형, 3 - 기믹 통과형(
	// 100 자리 값 : 기믹 타입( GimmickType )
	// 1 자리 값 : HP , N배 ...
	// ex) 10101 : 나무 1단계 , 10103 : 나무 3단계 , 10201 : 강철 1단계 , 20101 : 사과 1단계 , 30102 : 데미지2배 구역

	public enum GimmickType
	{
		NONE = 0,
		BOX = 10100,
		IRON_BOX = 10200,
		APPLE = 20100,
		DAMAGE_AREA = 30100,
	}
}

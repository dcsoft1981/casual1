using UnityEngine;

public class Define
{
	public const string CHILD_SPRITE_OBJECT = "ChildSpriteObject";
	public const string CUR_LEVEL = "CUR_LEVEL";
	public const string OPTION_SOUND_OFF = "OPTION_SOUND_OFF";
	public const string OPTION_VIBRATE_OFF = "OPTION_VIBRATE_OFF";
	public const string LOCALE_RETRY = "RETRY";
	public const string LOCALE_NEXT = "NEXT";

	public const string SKILL_DOUBLE_DAMAGE = "Double Damage";
	public const string SKILL_FINAL_SHOT = "Final Shot";

	//public const int MAX_LEVEL_KEYFRAME_COUNT = 5; // 회전값 복수개 사용시 사용 현재 미사용

	public const int ROTATE_SEC = 10;
	public const float TARGET_BASE_SCALE = 3f;

	public const float BASE_GIMMICK_SCALE = 1 / 3f;
	public const int MAX_COMBO = 5;
	public static Color COLOR_TARGET_BASE = new Color(40f/255f, 40f / 255f, 40f / 255f, 1f);
	public static Color COLOR_BLACK = new Color(0f, 0f, 0f, 1f);
	public static Color COLOR_BLACK_ALPHA10 = new Color(0f, 0f, 0f, 0.4f);
	public static Color GREEN2 = new Color(0f, 0.8f, 0f, 1f);
	public static Color HP_OVER4 = new Color(0.9f, 0f, 0f);
	public static Color HP_3 = new Color(0.7f, 0f, 0f);
	public static Color HP_2 = new Color(0.4f, 0f, 0f);
	public static Color HP_1 = new Color(0.1f, 0f, 0f);

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
		// 기믹 히트형
		NONE = 0,
		SHIELD = 10100, // 방패
		SUPER_SHIELD = 10200, // 무적 방패
		SEQUENCE = 10300, // 순서 ( 지정된 제거 순서가 존재,제거 순서가 아닌 경우 제거 불가)
		KEY_CHAIN = 10400, // 모든 고리 기믹을 제거해야 타겟 히트 가능
		TARGET_RECOVER = 10500, // 히트시 타겟 회복
		CONTINUE_HIT = 10600, // 연속해서 히트하지 못할 경우, HP가 원복됨 (=로얄매치 두더지)
		REMOVE_SHOT = 10700, // 소거 시, 타겟에 붙어있는 모든 발사체 소거 (도움형 기믹)
		DAMAGE_N = 10800, // 소거 시, 타겟에 데미지 2 적용
		ONOFF_ON = 10900, // 턴마다 제거 가능 여부가 바뀜. 히트가능 시작 (로얄매치의 나비넥타이)
		ONOFF_OFF = 11000, // 턴마다 제거 가능 여부가 바뀜. 히트불가 시작 (로얄매치의 나비넥타이)


		// 타겟 히트형
		ROTATION_UP = 20100, // 히트 시 회전속도 증가
		ROTATION_DOWN = 20200, // 히트 시 회전속도 감소
		ADD_SHOT = 20300, // 히트 시 발사체 수량 N 증가
		WARP = 20400, // 히트 시 히트 위치의 180도 반대쪽에 발사체가 꽂힘(보류)


		// 기믹 통과형
		DAMAGE_AREA = 30100, //해당 영역을 통과해서 히트 시, 데미지가 N배로 들어감 (N=2~5?)

		// alex notebook commit
	}

	// 효과가 너무 좋지 않은것으로 타입들 잡아야됨
	public enum PassiveType
	{
		NONE = 0,
		SHOT_DOUBLE_DAMAGE = 1, // 2배 데미지
		FAILURE_BONUS_SHOT = 2, // 실패 시 추가 발사체

		// 보류
		SHOT_ROTATION_DOWN = 101, // 히트 시 회전속도 감소
	}
}

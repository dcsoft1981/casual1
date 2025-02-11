using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Define
{
	public const int STABLEUSER_LEVEL = 51;
	// Local Save Data
	public const string CHILD_SPRITE_OBJECT = "ChildSpriteObject";
	public const string CUR_LEVEL = "CUR_LEVEL";
	public const string OPTION_SOUND_OFF = "OPTION_SOUND_OFF";
	public const string OPTION_VIBRATE_OFF = "OPTION_VIBRATE_OFF";
	public const string OPTION_GUIDELINE_OFF = "OPTION_GUIDELINE_OFF";
	public const string INFINITY_STAGE = "INFINITY_STAGE";
	public const string LEVEL_PLAY_DATA_STARTSEC = "LEVEL_PLAY_DATA_STARTSEC";
	public const string LEVEL_PLAY_DATA_TRYCOUNT = "LEVEL_PLAY_DATA_TRYCOUNT";
	public const string LEVEL_PLAY_DATA_SHOTCOUNT = "LEVEL_PLAY_DATA_SHOTCOUNT";
	public const string REDDOT_PLAYER = "REDDOT_PLAYER";
	public const string REDDOT_MENU = "REDDOT_MENU";
	public const string REDDOT_PLAY = "REDDOT_PLAY";


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
	public static Color GREEN2 = new Color(0f, 0.5f, 0f);
	public static Color HP_OVER4 = new Color(0.9f, 0f, 0f);
	public static Color HP_3 = new Color(0.9f, 0f, 0f);
	public static Color HP_2 = new Color(0.7f, 0f, 0f);
	public static Color HP_1 = new Color(0.3f, 0f, 0f);
	public static Color BASE_YELLOW = new Color(255f/255f, 254f / 255f, 213f / 255f);
	public static Color DARK_RED = new Color(75f / 255f, 25f / 255f, 25f / 255f);
	public static Color COLOR_TARGET_SHIELD = Color.gray;
	public static Color DAMAGE_LINE_COLOR = new Color(183f / 255f, 88f / 255f, 255f / 255f, 200f/255f);
	public static Color NODAMAGE_LINE_COLOR = new Color(127f / 255f, 127f / 255f, 127f / 255f, 200f / 255f);
	public static Color GIMMICKHIT_DEBUFF = new Color(0.1f, 0f, 0f);
	public static Color GIMMICKHIT_BUFF = new Color(0.9f, 0f, 0f);
	public static Color TARGETHIT_DEBUFF = new Color(0f, 0f, 0.3f);
	public static Color TARGETHIT_BUFF = new Color(0f, 0f, 1f);

	// 타겟
	// 10000 자리 값 : 외형. 일단 색으로 구분(1-GREEN, 2-BLUE,3-RED)
	// 1 자리 값 : 반지름 사이즈% 
	// ex) 10100 - BLACK 100% 사이즈 , 10050 - BLACK 50% 사이즈 , 30050 - RED 50% 사이즈
	public enum TargetType
	{
		None = 0,
		BLACK = 10000,
		SILVER = 20000,
		GOLD = 30000,
	}

	public enum GimmickCathegory
	{
		NONE = 0,
		GimmickHit = 1,
		TargetHit = 2,
		GimmickPass = 3,
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
		NODAMAGE_AREA = 30200, //해당 영역을 통과해서 히트 시, 데미지가 0

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

	// 인게임 모드
	public enum IngameType
	{
		NONE = 0,
		NORMAL = 1,
		INFINITY = 2,
	}

	// 발사체 기믹 히트 결과 타입
	public enum ShotGimmickHitResult
	{
		NONE = 0,
		ALEADY_COMPLETED = 1, // 이미 해당 핀에 대한 히트 처리 완료
		HIT_THROUTH =  2, // 히트하고 진행
		HIT_REFLECT = 3, // 히트하고 반사
		HIT_PAIR_REFLECT = 4, // 페어기믹 히트하고 반사
		HIT_IRON_REFLECT = 5, // 무적기믹히트하고 반사
	}

	// 흔들기 타입
	public enum ShakeType
	{
		NONE = 0,
		NORMAL = 1, 
		LONG = 2, // 김(REMOVE_SHOT 타입)
		IMPACK = 3,
	}

	// 타겟 색 변경 타입
	public enum TargetColorType
	{
		RED = 0,
		GREEN = 1,
		BLUE = 2,
		RED_GREEN = 3,
		RED_BLUE = 4,
		GREEN_BLUE = 5,
		ALL = 6,
		NONE = 7
	}

	// 표정 타입
	public enum ExpressionType
	{
		//NONE = 0,
		CRY = 1, // T T
		CIRCLE = 2, // O O
		CLOSE = 3, // > <
		LINE = 4, // - -
		X = 5, // X X
		WAVE = 6,
		ONE_CAPSULE = 7, 
		STAR = 8,
		HEART = 9,
		SMILE = 10, //  ^ ^
		//ONE_LINE = 11,
	}


	// Static 함수들
	public static GimmickCathegory GetGimmickCathegory(GimmickType type)
	{
		int typeValue = (int)type;
		if (typeValue >= 10000 && typeValue < 20000)
			return GimmickCathegory.GimmickHit;
		else if (typeValue >= 20000 && typeValue < 30000)
			return GimmickCathegory.TargetHit;
		else if (typeValue >= 30000 && typeValue < 40000)
			return GimmickCathegory.GimmickPass;
		return GimmickCathegory.NONE;
	}

	public static int GetCurrentUnixTimestamp()
	{
		return (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
	}

	public static string ConvertSecondsToTimeString(long totalSeconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
		return timeSpan.ToString(@"hh\:mm\:ss");
	}

	public static T GetRandomEnumValue<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        System.Random random = new System.Random();
        return (T)values.GetValue(random.Next(values.Length));
    }

	public static Color GetRGBColor(float r , float g , float b )
	{
		return new Color(r / 255f, g / 255f, b / 255f);
	}

	public static ExpressionType GetStartRandomExpression()
	{
		List<ExpressionType> list = new List<ExpressionType>();
		list.Add(ExpressionType.CRY);
		list.Add(ExpressionType.CIRCLE);
		list.Add(ExpressionType.CLOSE);
		list.Add(ExpressionType.LINE);
		list.Add(ExpressionType.X);
		list.Add(ExpressionType.WAVE);
		list.Add(ExpressionType.ONE_CAPSULE);
		list.Add(ExpressionType.STAR);

		List<ExpressionType> list2Shuffle= list.OrderBy(x => Guid.NewGuid()).ToList();
		return list2Shuffle[0];
	}
}

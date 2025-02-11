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

	//public const int MAX_LEVEL_KEYFRAME_COUNT = 5; // ȸ���� ������ ���� ��� ���� �̻��

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

	// Ÿ��
	// 10000 �ڸ� �� : ����. �ϴ� ������ ����(1-GREEN, 2-BLUE,3-RED)
	// 1 �ڸ� �� : ������ ������% 
	// ex) 10100 - BLACK 100% ������ , 10050 - BLACK 50% ������ , 30050 - RED 50% ������
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

	// ���
	// 10000 �ڸ� �� : ��( 1 - �����Ʈ��, 2 - Ÿ����Ʈ��, 3 - ��� �����(
	// 100 �ڸ� �� : ��� Ÿ��( GimmickType )
	// 1 �ڸ� �� : HP , N�� ...
	// ex) 10101 : ���� 1�ܰ� , 10103 : ���� 3�ܰ� , 10201 : ��ö 1�ܰ� , 20101 : ��� 1�ܰ� , 30102 : ������2�� ����

	public enum GimmickType
	{
		// ��� ��Ʈ��
		NONE = 0,
		SHIELD = 10100, // ����
		SUPER_SHIELD = 10200, // ���� ����
		SEQUENCE = 10300, // ���� ( ������ ���� ������ ����,���� ������ �ƴ� ��� ���� �Ұ�)
		KEY_CHAIN = 10400, // ��� �� ����� �����ؾ� Ÿ�� ��Ʈ ����
		TARGET_RECOVER = 10500, // ��Ʈ�� Ÿ�� ȸ��
		CONTINUE_HIT = 10600, // �����ؼ� ��Ʈ���� ���� ���, HP�� ������ (=�ξ��ġ �δ���)
		REMOVE_SHOT = 10700, // �Ұ� ��, Ÿ�ٿ� �پ��ִ� ��� �߻�ü �Ұ� (������ ���)
		DAMAGE_N = 10800, // �Ұ� ��, Ÿ�ٿ� ������ 2 ����
		ONOFF_ON = 10900, // �ϸ��� ���� ���� ���ΰ� �ٲ�. ��Ʈ���� ���� (�ξ��ġ�� �����Ÿ��)
		ONOFF_OFF = 11000, // �ϸ��� ���� ���� ���ΰ� �ٲ�. ��Ʈ�Ұ� ���� (�ξ��ġ�� �����Ÿ��)


		// Ÿ�� ��Ʈ��
		ROTATION_UP = 20100, // ��Ʈ �� ȸ���ӵ� ����
		ROTATION_DOWN = 20200, // ��Ʈ �� ȸ���ӵ� ����
		ADD_SHOT = 20300, // ��Ʈ �� �߻�ü ���� N ����
		WARP = 20400, // ��Ʈ �� ��Ʈ ��ġ�� 180�� �ݴ��ʿ� �߻�ü�� ����(����)


		// ��� �����
		DAMAGE_AREA = 30100, //�ش� ������ ����ؼ� ��Ʈ ��, �������� N��� �� (N=2~5?)
		NODAMAGE_AREA = 30200, //�ش� ������ ����ؼ� ��Ʈ ��, �������� 0

		// alex notebook commit
	}

	// ȿ���� �ʹ� ���� ���������� Ÿ�Ե� ��ƾߵ�
	public enum PassiveType
	{
		NONE = 0,
		SHOT_DOUBLE_DAMAGE = 1, // 2�� ������
		FAILURE_BONUS_SHOT = 2, // ���� �� �߰� �߻�ü

		// ����
		SHOT_ROTATION_DOWN = 101, // ��Ʈ �� ȸ���ӵ� ����
	}

	// �ΰ��� ���
	public enum IngameType
	{
		NONE = 0,
		NORMAL = 1,
		INFINITY = 2,
	}

	// �߻�ü ��� ��Ʈ ��� Ÿ��
	public enum ShotGimmickHitResult
	{
		NONE = 0,
		ALEADY_COMPLETED = 1, // �̹� �ش� �ɿ� ���� ��Ʈ ó�� �Ϸ�
		HIT_THROUTH =  2, // ��Ʈ�ϰ� ����
		HIT_REFLECT = 3, // ��Ʈ�ϰ� �ݻ�
		HIT_PAIR_REFLECT = 4, // ����� ��Ʈ�ϰ� �ݻ�
		HIT_IRON_REFLECT = 5, // ���������Ʈ�ϰ� �ݻ�
	}

	// ���� Ÿ��
	public enum ShakeType
	{
		NONE = 0,
		NORMAL = 1, 
		LONG = 2, // ��(REMOVE_SHOT Ÿ��)
		IMPACK = 3,
	}

	// Ÿ�� �� ���� Ÿ��
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

	// ǥ�� Ÿ��
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


	// Static �Լ���
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

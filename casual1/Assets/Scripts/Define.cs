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

	//public const int MAX_LEVEL_KEYFRAME_COUNT = 5; // ȸ���� ������ ���� ��� ���� �̻��

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

	// Ÿ��
	// 10000 �ڸ� �� : ����. �ϴ� ������ ����(1-GREEN, 2-BLUE,3-RED)
	// 1 �ڸ� �� : ������ ������% 
	// ex) 10100 - BLACK 100% ������ , 10050 - BLACK 50% ������ , 30050 - RED 50% ������
	public enum TargetType
	{
		None = 0,
		BLACK = 10000,
		BLUE = 20000,
		RED = 30000,
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
}

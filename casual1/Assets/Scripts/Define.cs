using UnityEngine;

public class Define
{
	public const string CUR_LEVEL = "CUR_LEVEL";
	public const string LOCALE_RETRY = "RETRY";
	public const string LOCALE_NEXT = "NEXT";

	//public const int MAX_LEVEL_KEYFRAME_COUNT = 5; // ȸ���� ������ ���� ��� ���� �̻��

	public const int ROTATE_SEC = 10;
	public const float TARGET_BASE_SCALE = 3f;

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
		NONE = 0,
		BOX = 10100,
		IRON_BOX = 10200,
		APPLE = 20100,
		DAMAGE_AREA = 30100,
	}
}

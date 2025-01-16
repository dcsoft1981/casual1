using System.Collections.Generic;
using UnityEngine;
using static Define;

public class LocalDataManager : MonoBehaviour
{
	public static LocalDataManager instance = null;
	private int curLevel = 0;

	[SerializeField] private LevelDB levelDB;
	[SerializeField] private GimmickDB gimmickDB;
	[SerializeField] private GradeDB gradeDB;

	private Dictionary<int, GimmickDBEntity> dic_gimmicks;
	private Dictionary<int, Sprite> dic_gimmickSprites;
	private Dictionary<int, Sprite> dic_gimmickSprites2;
	private Dictionary<int, Sprite> dic_gimmickSprites3;
	private Dictionary<int, GradeDBEntity> dic_grades;
	private Dictionary<int, Color> dic_gradeColor;
	private Dictionary<int, int> dic_levelGrade;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			curLevel = PlayerPrefs.GetInt(Define.CUR_LEVEL, 1);

			dic_gimmicks = new Dictionary<int, GimmickDBEntity>();
			dic_gimmickSprites = new Dictionary<int, Sprite>();
			dic_gimmickSprites2 = new Dictionary<int, Sprite>();
			dic_gimmickSprites3 = new Dictionary<int, Sprite>();
			dic_grades = new Dictionary<int, GradeDBEntity>();
			dic_gradeColor = new Dictionary<int, Color>();
			dic_levelGrade = new Dictionary<int, int>();
			foreach (GimmickDBEntity gimmickDbEntity in gimmickDB.gimmicks)
			{
				dic_gimmicks.Add(gimmickDbEntity.id, gimmickDbEntity);
				if (gimmickDbEntity.sprite.Length > 0)
					dic_gimmickSprites.Add(gimmickDbEntity.id, Resources.Load<Sprite>(gimmickDbEntity.sprite));
				if (gimmickDbEntity.sprite2.Length > 0)
					dic_gimmickSprites2.Add(gimmickDbEntity.id, Resources.Load<Sprite>(gimmickDbEntity.sprite2));
				if (gimmickDbEntity.sprite3.Length > 0)
					dic_gimmickSprites3.Add(gimmickDbEntity.id, Resources.Load<Sprite>(gimmickDbEntity.sprite3));
			}
			foreach(GradeDBEntity gradeDBEntity in gradeDB.grades)
			{
				dic_grades.Add(gradeDBEntity.id, gradeDBEntity);
				dic_gradeColor.Add(gradeDBEntity.id, GetColorFromRGB(gradeDBEntity.color));
			}
			foreach(LevelDBEntity levelDBEntity in levelDB.levels)
			{
				int grade = GetGradeFromLevel(levelDBEntity.id);
				if( grade != 0)
				{
					dic_levelGrade.Add(levelDBEntity.id, grade);
				}
			}
		}
	}

	public int GetCurLevel()
	{
		return curLevel;
	}

	public void SetCurLevel(int level)
	{
		PlayerPrefs.SetInt(Define.CUR_LEVEL, level);
		curLevel = level;
	}

	public LevelDBEntity GetLevelDBEntity()
	{
		int level = GetCurLevel();
		return levelDB.levels[level - 1];
	}

	public GimmickDBEntity GetGimmickInfo(GimmickType type)
	{
		if (dic_gimmicks.TryGetValue((int)type, out GimmickDBEntity gimmick))
			return gimmick;
		return null;
	}

	public Sprite GetGimmickSprite(GimmickType type)
	{
		if (dic_gimmickSprites.TryGetValue((int)type, out Sprite sprite))
			return sprite;
		return null;
	}

	public Sprite GetGimmickSprite2(GimmickType type)
	{
		if (dic_gimmickSprites2.TryGetValue((int)type, out Sprite sprite))
			return sprite;
		return null;
	}

	public Sprite GetGimmickSprite3(GimmickType type)
	{
		if (dic_gimmickSprites3.TryGetValue((int)type, out Sprite sprite))
			return sprite;
		return null;
	}

	public Color GetColorFromRGB(string rgb)
	{
		string[] strInfo = rgb.Split(":");
		float[] rgbInfo = new float[strInfo.Length];
		for (int i = 0; i < rgbInfo.Length; i++)
		{
			rgbInfo[i] = float.Parse(strInfo[i]);
		}

		return new Color(rgbInfo[0]/255f, rgbInfo[1]/255f, rgbInfo[2]/255f);
	}

	private int GetGradeFromLevel(int level)
	{
		foreach (GradeDBEntity gradeDBEntity in gradeDB.grades)
		{
			if (level >= gradeDBEntity.minLevel && level <= gradeDBEntity.maxLevel)
				return gradeDBEntity.id;
		}

		return 0;
	}

	public int GetCurGrade()
	{
		if(dic_levelGrade.TryGetValue(GetCurLevel(), out int grade))
		{  return grade; }

		return 0;
	}

	public GradeDBEntity GetCurGradeDBEntity()
	{
		int grade = GetCurGrade();
		if (grade == 0)
			return null;

		if (dic_grades.TryGetValue(grade, out GradeDBEntity entity))
			return entity;
		return null;
	}

	public Color GetCurColor()
	{
		int grade = GetCurGrade();
		if (grade == 0)
			return Color.black;

		if (dic_gradeColor.TryGetValue(grade, out Color color))
			return color;
		return Color.black;
	}
}

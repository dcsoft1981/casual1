using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;

public class LocalDataManager : MonoBehaviour
{
	public static LocalDataManager instance = null;
	private int curLevel = 0;
	private int optionSoundOff = 0;
	private int optionVibrateOff = 0;
	private int optionGuideLineOff = 0;
	private int infinityStage = 0;
	private int levelPlayDataStartSec = 0;
	private int levelPlayDataTryCount = 0;
	private int levelPlayDataShotCount = 0;

	[SerializeField] private LevelDB levelDB;
	[SerializeField] private GimmickDB gimmickDB;
	[SerializeField] private GradeDB gradeDB;

	private Dictionary<int, GimmickDBEntity> dic_gimmicks;
	private Dictionary<int, Sprite> dic_gimmickSprites;
	private Dictionary<int, Sprite> dic_gimmickSprites2;
	private Dictionary<int, Sprite> dic_gimmickSprites3;
	private Dictionary<int, GradeDBEntity> dic_grades;
	private Dictionary<int, UnityEngine.Color> dic_gradeColor;
	private Dictionary<int, int> dic_levelGrade;
	private List<int> listStage;

	private int maxLevel = 0;

	public Sprite spriteQuestion = null;

	private void Awake()
	{
		if (instance == null)
		{
			// 시스템 설정
			// 60 FPS로 고정
			Application.targetFrameRate = 60;
			QualitySettings.vSyncCount = 0; // vSync를 비활성화하여 targetFrameRate가 적용되도록 설정
			Time.fixedDeltaTime = 0.01f; // 원하는 고정 시간 스텝 설정

			instance = this;

			// 로컬 저장 데이터
			curLevel = PlayerPrefs.GetInt(Define.CUR_LEVEL, 1);
			optionSoundOff = PlayerPrefs.GetInt(Define.OPTION_SOUND_OFF, 0);
			optionVibrateOff = PlayerPrefs.GetInt(Define.OPTION_VIBRATE_OFF, 0);
			optionGuideLineOff = PlayerPrefs.GetInt(Define.OPTION_GUIDELINE_OFF, 1);
			infinityStage = PlayerPrefs.GetInt(Define.INFINITY_STAGE, 0);
			levelPlayDataStartSec = PlayerPrefs.GetInt(Define.LEVEL_PLAY_DATA_STARTSEC, 0);
			levelPlayDataTryCount = PlayerPrefs.GetInt(Define.LEVEL_PLAY_DATA_TRYCOUNT, 0);
			levelPlayDataShotCount = PlayerPrefs.GetInt(Define.LEVEL_PLAY_DATA_SHOTCOUNT, 0);


			dic_gimmicks = new Dictionary<int, GimmickDBEntity>();
			dic_gimmickSprites = new Dictionary<int, Sprite>();
			dic_gimmickSprites2 = new Dictionary<int, Sprite>();
			dic_gimmickSprites3 = new Dictionary<int, Sprite>();
			dic_grades = new Dictionary<int, GradeDBEntity>();
			dic_gradeColor = new Dictionary<int, Color>();
			dic_levelGrade = new Dictionary<int, int>();
			listStage = new List<int>();
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
			foreach (GradeDBEntity gradeDBEntity in gradeDB.grades)
			{
				dic_grades.Add(gradeDBEntity.id, gradeDBEntity);
				dic_gradeColor.Add(gradeDBEntity.id, GetColorFromRGB(gradeDBEntity.color));
			}
			foreach (LevelDBEntity levelDBEntity in levelDB.levels)
			{
				int grade = GetGradeFromLevel(levelDBEntity.id);
				if (grade != 0)
				{
					dic_levelGrade.Add(levelDBEntity.id, grade);
				}
				if (levelDBEntity.id > maxLevel)
					maxLevel = levelDBEntity.id;
				listStage.Add(levelDBEntity.id);
			}
			// 올클 상황 등급 추가
			{
				int grade = GetGradeFromLevel(maxLevel + 1);
				if (grade != 0)
				{
					dic_levelGrade.Add(maxLevel + 1, grade);
				}
			}

			ListShuffler.ShuffleList(listStage, maxLevel);
			LocalDataManager.instance.CheckInfinityStage();

			spriteQuestion = Resources.Load<Sprite>("Help");
		}
	}

	public int GetCurLevel()
	{
		return curLevel;
	}

	public void SetCurLevel(int level)
	{
		PlayerPrefs.SetInt(Define.CUR_LEVEL, level);
		PlayerPrefs.Save();
		curLevel = level;
	}

	public LevelDBEntity GetLevelDBEntity(int level)
	{
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

		return Define.GetRGBColor(rgbInfo[0], rgbInfo[1], rgbInfo[2]);
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
		int curLevel = GetCurLevel();
		if (curLevel > GetMaxLevel())
		{
			curLevel = GetMaxLevel() + 1;
		}
		if (dic_levelGrade.TryGetValue(curLevel, out int grade))
		{ return grade; }

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
		return GetGradeColor(GetCurGrade());
	}

	public Color GetGradeColor(int grade)
	{
		if (grade == 0)
			return Color.black;

		if (dic_gradeColor.TryGetValue(grade, out Color color))
			return color;
		return Color.black;
	}

	public int GetMaxLevel()
	{
		return maxLevel;
	}

	public List<GradeDBEntity> GetGradeEntity()
	{
		return gradeDB.grades;
	}

	public List<GimmickDBEntity> GetGimmickEntity()
	{
		return gimmickDB.gimmicks;
	}

	public bool GetSoundOff()
	{
		if (optionSoundOff == 0)
			return false;
		else
			return true;
	}

	public void SetSoundOff(bool soundOff)
	{
		int value = 0;
		if (soundOff)
		{
			value = 1;
		}
		optionSoundOff = value;
		PlayerPrefs.SetInt(Define.OPTION_SOUND_OFF, optionSoundOff);
		PlayerPrefs.Save();
	}

	public bool GetVibrateOff()
	{
		if (optionVibrateOff == 0)
			return false;
		else
			return true;
	}

	public void SetVibrateOff(bool off)
	{
		int value = 0;
		if (off)
		{
			value = 1;
		}
		optionVibrateOff = value;
		PlayerPrefs.SetInt(Define.OPTION_VIBRATE_OFF, optionVibrateOff);
		PlayerPrefs.Save();
	}

	public bool GetGuideLineOff()
	{
		if (optionGuideLineOff == 0)
			return false;
		else
			return true;
	}

	public void SetGuideLineOff(bool off)
	{
		int value = 0;
		if (off)
		{
			value = 1;
		}
		optionGuideLineOff = value;
		PlayerPrefs.SetInt(Define.OPTION_GUIDELINE_OFF, optionGuideLineOff);
		PlayerPrefs.Save();
	}

	public IngameType GetIngameType()
	{
		int level = GetCurLevel();
		if (level > GetMaxLevel())
			return IngameType.INFINITY;

		return IngameType.NORMAL;
	}

	public int GetInfinityStage()
	{
		return infinityStage;
	}

	public void SetInfinityStage(int _infinityStage)
	{
		PlayerPrefs.SetInt(Define.INFINITY_STAGE, _infinityStage);
		PlayerPrefs.Save();
		infinityStage = _infinityStage;
	}

	public void CheckInfinityStage()
	{
		int level = GetCurLevel();
		if (level > GetMaxLevel())
		{
			// 올클 상황
			if (infinityStage == 0)
			{
				SetInfinityStage(1);
			}
		}
		else
		{
			// 올클 전 상황
			if (infinityStage != 0)
			{
				SetInfinityStage(0);
			}
		}
	}

	public int GetInfinityStageLevel()
	{
		int stage = GetInfinityStage();
		return GetInfinityStageLevel(stage);
	}

	public int GetInfinityStageLevel(int stage)
	{
		int index = (stage - 1) % GetMaxLevel();
		return listStage[index];
	}

	public void ClearLevelPlayData()
	{
		PlayerPrefs.SetInt(Define.LEVEL_PLAY_DATA_STARTSEC, 0);
		levelPlayDataStartSec = 0;
		PlayerPrefs.SetInt(Define.LEVEL_PLAY_DATA_TRYCOUNT, 0);
		levelPlayDataTryCount = 0;
		PlayerPrefs.SetInt(Define.LEVEL_PLAY_DATA_SHOTCOUNT, 0);
		PlayerPrefs.Save();
		levelPlayDataShotCount = 0;
	}

	public void StartLevelPlayData()
	{
		if (levelPlayDataStartSec == 0)
		{
			levelPlayDataStartSec = Define.GetCurrentUnixTimestamp();
			PlayerPrefs.SetInt(Define.LEVEL_PLAY_DATA_STARTSEC, levelPlayDataStartSec);
		}
		levelPlayDataTryCount++;
		PlayerPrefs.SetInt(Define.LEVEL_PLAY_DATA_TRYCOUNT, levelPlayDataTryCount);
	}

	public void AddShotPlayData()
	{
		levelPlayDataShotCount++;
		PlayerPrefs.SetInt(Define.LEVEL_PLAY_DATA_SHOTCOUNT, levelPlayDataShotCount);
	}

	public string GetLevelPlayDataText()
	{
		int startTime = levelPlayDataStartSec;
		int curTime = Define.GetCurrentUnixTimestamp();
		int sec = curTime - startTime;
		if (sec < 0)
			sec = 0;
		string timeText = Define.ConvertSecondsToTimeString(sec);
		StringBuilder sb = new StringBuilder();
		sb.Append(timeText).Append(" Time Taken\\nTryCount : ").Append(levelPlayDataTryCount).Append(" ShotCount : ").Append(levelPlayDataShotCount);
		return sb.ToString();
	}

	public GameObject CreateGradeInfo(GradeDBEntity entity, GameObject createGrade, Transform parentTransform)
	{
		GameObject gradeGameObject = Instantiate(createGrade, Vector3.zero, Quaternion.identity);
		gradeGameObject.transform.SetParent(parentTransform);
		gradeGameObject.transform.localScale = Vector3.one;

		Transform iconTransform = gradeGameObject.transform.Find("Content/Icon/Mask/Image");
		Transform nameTransform = gradeGameObject.transform.Find("Content/NameText");
		Transform messageTransform = gradeGameObject.transform.Find("Content/MessageText");
		// 아이콘 색상 변경
		iconTransform.gameObject.GetComponent<Image>().color = LocalDataManager.instance.GetGradeColor(entity.id);
		nameTransform.gameObject.GetComponent<TextMeshProUGUI>().SetText(entity.grade);
		messageTransform.gameObject.GetComponent<TextMeshProUGUI>().SetText(GetGradeInfoStr(entity));
		return gradeGameObject;
	}

	public void CreateGimmickInfo(GimmickDBEntity entity, GameObject createGrade, Transform parentTransform)
	{
		int gimmickOpenLevel = GetGimmickOpenLevel(entity.id);
		if (gimmickOpenLevel == 0)
			return;

		int curLevel = GetCurLevel();

		GameObject gradeGameObject = Instantiate(createGrade, Vector3.zero, Quaternion.identity);
		gradeGameObject.transform.SetParent(parentTransform);
		gradeGameObject.transform.localScale = Vector3.one;

		Transform iconTransform = gradeGameObject.transform.Find("Content/Icon/Mask/Image");
		Transform nameTransform = gradeGameObject.transform.Find("Content/NameText");
		Transform messageTransform = gradeGameObject.transform.Find("Content/MessageText");
		// 아이콘 색상 변경
		Image image = iconTransform.gameObject.GetComponent<Image>();
		GimmickType gimmickType = (GimmickType)entity.id;
		if (gimmickType == GimmickType.ONOFF_OFF)
		{
			if (dic_gimmickSprites2.TryGetValue(entity.id, out Sprite sprite))
			{
				image.sprite = sprite;
				image.color = GetGimmickColor(gimmickType, 1, false);
			}
		}
		else if (dic_gimmickSprites.TryGetValue(entity.id, out Sprite sprite))
		{
			image.sprite = sprite;
			image.color = GetGimmickColor(gimmickType, 1, false);
		}
		else
		{
			if (gimmickType == GimmickType.DAMAGE_AREA)
			{
				image.color = Define.DAMAGE_LINE_COLOR;
			}
			else if (gimmickType == GimmickType.NODAMAGE_AREA)
			{
				image.color = Define.NODAMAGE_LINE_COLOR;
			}
		}

		// 텍스트 설정
		nameTransform.gameObject.GetComponent<TextMeshProUGUI>().SetText(entity.gimmick);
		if(curLevel < gimmickOpenLevel)
		{
			// 오픈전
			image.sprite = spriteQuestion;
			string message = "Level " + gimmickOpenLevel + " Required";
			messageTransform.gameObject.GetComponent<TextMeshProUGUI>().SetText(message);
		}
		else
		{
			// 오픈 후
			messageTransform.gameObject.GetComponent<TextMeshProUGUI>().SetText(entity.textinfo);
		}
	
	}

	public string GetGradeInfoStr(GradeDBEntity entity)
	{
		if (entity.id == 1)
		{
			return "Clear the Level to earn a Tier.";
		}

		StringBuilder sb = new StringBuilder();
		sb.Append(entity.minLevel - 1).Append(" Level Clear");
		if (entity.doubleDamage > 0)
		{
			sb.Append("\n").Append(Define.SKILL_DOUBLE_DAMAGE).Append(" : ").Append(entity.doubleDamage).Append("%");
		}
		if (entity.bonusShot > 0)
		{
			sb.Append("\n").Append(Define.SKILL_FINAL_SHOT).Append(" : ").Append(entity.bonusShot).Append("%");
		}
		return sb.ToString();
	}

	public Color GetGimmickColor(GimmickType type, int hp, bool isChecked)
	{
		switch (type)
		{
			case GimmickType.SUPER_SHIELD:
				return Define.COLOR_TARGET_SHIELD;
			case GimmickType.TARGET_RECOVER:
				return Define.GIMMICKHIT_DEBUFF;
			case GimmickType.DAMAGE_N:
			case GimmickType.REMOVE_SHOT:
				return Define.GIMMICKHIT_BUFF;
			case GimmickType.ROTATION_DOWN:
				return Define.TARGETHIT_BUFF;
			case GimmickType.ROTATION_UP:
				return Define.TARGETHIT_DEBUFF;
			case GimmickType.ADD_SHOT:
				return Define.TARGETHIT_BUFF;
			case GimmickType.SEQUENCE:
				{
					if (isChecked)
						return Define.GREEN2;
					else
						return Color.black;
				}
			case GimmickType.KEY_CHAIN:
				{
					if (isChecked)
						return Define.GREEN2;
					else
						return Color.black;
				}
		}

		if (hp > 3)
			return Define.HP_OVER4;
		if (hp == 3)
			return Define.HP_3;
		else if (hp == 2)
			return Define.HP_2;
		else if (hp == 1)
			return Define.HP_1;

		return Color.black;
	}

	public int GetGimmickOpenLevel(int gimmickId)
	{
		foreach (LevelDBEntity levelDBEntity in levelDB.levels)
		{
			if (LevelDataGimmickExist(levelDBEntity.gimmick1, gimmickId)) return levelDBEntity.id;
			if (LevelDataGimmickExist(levelDBEntity.gimmick2, gimmickId)) return levelDBEntity.id;
			if (LevelDataGimmickExist(levelDBEntity.gimmick3, gimmickId)) return levelDBEntity.id;
			if (LevelDataGimmickExist(levelDBEntity.gimmick4, gimmickId)) return levelDBEntity.id;
			if (LevelDataGimmickExist(levelDBEntity.gimmick5, gimmickId)) return levelDBEntity.id;
			if (LevelDataGimmickExist(levelDBEntity.gimmick6, gimmickId)) return levelDBEntity.id;
			if (LevelDataGimmickExist(levelDBEntity.gimmick7, gimmickId)) return levelDBEntity.id;
		}

		return 0;
	}

	public bool LevelDataGimmickExist(string gimmick, int gimmickId)
	{
		if (gimmick.Length == 0)
			return false;

		string[] strInfo = gimmick.Split(":");
		int[] numInfo = new int[strInfo.Length];
		for (int i = 0; i < numInfo.Length; i++)
		{
			numInfo[i] = int.Parse(strInfo[i]);
		}

		int gimmickValue = numInfo[0];
		int hp = gimmickValue % 100;
		if (gimmickId == (gimmickValue - hp))
			return true;
		else
			return false;
	}
}

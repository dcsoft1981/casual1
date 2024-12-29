using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameDataManager
{
	public Dictionary<int,LevelData> dicLevel = null;

	public GameDataManager()
	{
		dicLevel = new Dictionary<int, LevelData>();
		//SetTempData();
	}

	private void SetTempData()
	{
		for (int i = 1; i < 100; i++)
		{
			LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
			dicLevel.Add(i, levelData);
			levelData.level = i;
			levelData.goal = GetGoal(levelData.level);
			levelData.datas = new List<LevelRotateData>();

			{
				float fValue = (float)i;
				LevelRotateData levelRotateData0 = new LevelRotateData();
				levelRotateData0.rotate = 150f + fValue;
				levelRotateData0.time = 0f;
				levelData.datas.Add(levelRotateData0);

				LevelRotateData levelRotateData1 = new LevelRotateData();
				levelRotateData1.rotate = 100f + fValue / 2;
				levelRotateData1.time = 1f + fValue * 0.01f;
				levelData.datas.Add(levelRotateData1);

				LevelRotateData levelRotateData2 = new LevelRotateData();
				levelRotateData2.rotate = 10f - fValue / 4;
				levelRotateData2.time = 1.5f + fValue * 0.011f;
				levelData.datas.Add(levelRotateData2);

				LevelRotateData levelRotateData3 = new LevelRotateData();
				levelRotateData3.rotate = 60f - fValue / 4;
				levelRotateData3.time = 2f + fValue * 0.012f;
				levelData.datas.Add(levelRotateData3);
			}
		}
	}

	private LevelData GetLevelData(int level)
	{
		return dicLevel[level];
	}

	private int GetGoal(int level)
	{
		if (level < 10)
			return 14;
		else if (level < 20)
			return 15;
		else if (level < 30)
			return 16;
		else if (level < 40)
			return 17;
		else if (level < 50)
			return 18;
		else if (level < 60)
			return 19;
		else
			return 20;
	}
}

using UnityEngine;

public class LocalDataManager : MonoBehaviour
{
	public static LocalDataManager instance = null;
	private int curLevel = 0;	

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			curLevel = PlayerPrefs.GetInt(Define.CUR_LEVEL, 1);
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
}

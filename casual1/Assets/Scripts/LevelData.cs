using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public int level;
	public int hp;
	public int shot;
	public int target;
	public List<LevelRotateData> datas;
}

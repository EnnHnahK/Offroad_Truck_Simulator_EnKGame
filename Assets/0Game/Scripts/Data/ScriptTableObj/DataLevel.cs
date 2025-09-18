using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_Data", menuName = "Data/Level_Data")]
public class DataLevel : ScriptableObject
{
	[System.Serializable]
	public class Level
	{
		public string name;
		public int id;
		public int packID;
		public Sprite levelSprite;
		public float baseFuel;
		public float basePerLevel = 15;
		public int[] fragmentsReward = new int[3];
	}

	public int maxLv;
	public int maxPack;
	public List<Level> levels = new ();

}

using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIChoseLevel : BasePanel
{
	[SerializeField] private GameObject packParent;
	[SerializeField] private LevelItem levelItem;
	[SerializeField] private LevelPack levelPack;
	
	private List<LevelItem> _levelItems = new ();
	private List<LevelPack> _levelPacks = new ();

	[SerializeField] private ScrollRect levelScroll;
	
	protected override void Awake()
	{
		base.Awake();
		LoadLevel();
	}

	private void LoadLevel()
	{
		for (int i = 0; i < DataController.CountLevel; i++)
		{
			var dataLevel = DataController.Instance.dataLevel.levels[i];
			if (dataLevel.packID >= _levelPacks.Count)
			{
				var pack =  Instantiate(levelPack, packParent.transform);
				_levelPacks.Add(pack);
			}
			_levelPacks[dataLevel.packID].Init(dataLevel.packID, dataLevel);
		}
	}
	
	public void ChoseLevel(int index, float baseFuel)
	{
		if(!canTarget) return;

		var anotherLevel = DataController.CurLevel != index;
		
		//Update Fuel
		UIController.Instance.uIGamePlay.SetBaseFuel(baseFuel);


		ShowAfterHide(UIController.Instance.uIGarage, false, 3f);
		
		PrefData.CurLevel = index;
		
		//Load Upgrade Data
		UIController.Instance.uIGarage.LoadData();

		if(anotherLevel) DOVirtual.DelayedCall(1f, () => _MainLevelLoader.Instance.LoadLevel(index)) ;
	}

	public void UnlockLevel(int index)
	{
		if (_levelItems.Count == 0)
		{
			return;
		}
		
		_levelItems[index].UnlockMap();
	}
	
	public override void Hide()
	{
		ShowAfterHide(UIController.Instance.uIGarage, false, 3f);
	}
	
	private void OnDisable()
	{
		levelScroll.verticalNormalizedPosition = 1f;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPack : MonoBehaviour
{
   [SerializeField] private List<LevelItem> levelItems;

   [SerializeField] private GameObject levelParent;
   [SerializeField] private LevelItem levelItem;

   private int _packID;
   
   public void Init(int packID, DataLevel.Level data)
   {
      _packID = packID;
      var levelIns = Instantiate(levelItem, levelParent.transform);
      levelIns.Init(data, packID);
      levelItems.Add(levelIns);
   }

}

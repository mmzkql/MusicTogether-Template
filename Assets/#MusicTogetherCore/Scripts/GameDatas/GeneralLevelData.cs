using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CollectionData
{
    public string CollectionID;
    public GameObject CollectionPrefab;
}


[CreateAssetMenu(menuName = "MusicTogether/GeneralLevelData", fileName = "GeneralLevelData")]
public class GeneralLevelData : ScriptableObject
{
    public static GeneralLevelData Instance;
    public List<string> LevelIDs = new List<string>();
    public List<string> CollectionIDs = new List<string>();

    public void OnValidate()
    {
        Instance = this;
    }
}
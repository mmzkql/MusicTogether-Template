using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicTogether/LevelData", fileName = "NewLevelData")]
public class LevelData:ScriptableObject
{
    [ValueDropdown("GetLevelIDList")]
    public string LevelID;
    public string AuthorID;
    public string SceneName;
    public AudioClip LevelMusic;
    public float LevelMusicDelay;


    public string LevelName;
    public float MaxPercentage;
    [ValueDropdown("GetCollectionIDList")]
    public List<string> Collections;

    private List<string> GetLevelIDList()
    {
        return GeneralLevelData.Instance.LevelIDs;
    }
    private List<string> GetCollectionIDList()
    {
        return GeneralLevelData.Instance.CollectionIDs;
    }
}
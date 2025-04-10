using System;
using MusicTogether.General;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;


namespace MusicTogether.DancingBall
{
    [ExecuteAlways,CustomEditor(typeof(MapMaker))]
    public class MapMakerEditor : OdinEditor
    {
        private void OnSceneGUI()
        {
            MapMaker.Instance = (MapMaker)target;
        }

        public void UpdateValue()
        {
            MapMaker maker = (MapMaker)target;
            maker.UpdateValue(0);            
        }   

        public void CreateManager()
        {
            MapMaker maker = (MapMaker)target;

            GameObject newManager = new GameObject($"RoadManager{maker.roadManagers.Length-1}");
            newManager.transform.SetParent(maker.transform);
            newManager.AddComponent<RoadMaker>();
            
            maker.roadManagers.Add(newManager.GetComponent<RoadMaker>());
            newManager.UpdateValue(maker);
        }
    }
}
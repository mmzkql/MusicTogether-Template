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
        new void OnEnable()
        {
            MapMaker maker = (MapMaker)target;
            MapMaker.Instance = maker;
        }

        public void UpdateManager()
        {
            MapMaker maker = (MapMaker)target;
            maker.UpdateManager(0);            
        }   

        /*public void CreateManager()
        {
            MapMaker maker = (MapMaker)target;

            GameObject newRoad = new GameObject($"RoadManager{maker.roadManagers.Count-1}");
            newRoad.transform.SetParent(maker.transform);
            newRoad.AddComponent<RoadMaker>();
            
            maker.roadManagers.Add(newRoad.GetComponent<RoadMaker>());
            newRoad.GetComponent<RoadMaker>().GetData();
        }*/
    }
}
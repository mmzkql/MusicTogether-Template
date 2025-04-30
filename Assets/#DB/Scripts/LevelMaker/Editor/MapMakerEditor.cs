using MusicTogether.General;
using UnityEngine;
using UnityEditor;
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

        void OnValidate()
        {
            MapMaker maker = (MapMaker)target;
            //nothing;
        }
    }
}
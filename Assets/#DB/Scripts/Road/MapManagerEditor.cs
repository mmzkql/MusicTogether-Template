using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[ExecuteAlways,CustomEditor(typeof(MapManager))]
namespace MusicTogether.DancingBall
{
    public class MapManager : OdinEditor
    {
        public void UpdateClickNote()
        {
            MapManager manager = (MapManager)target;
            NoteType fromType = manager.inputData.noteType, toType = manager.noteProcessType;
            manager.defaultClickNotes.Clear();
            manager.defaultClickNoteData.NoteIndex.ForEach(item=>manager.defaultClickNotes.Add(NoteConverter.convertNoteIndex(item,fromType,toType)));
            manager.defaultClickNotes.Sort((a, b) => a.CompareTo(b));
        }
        public void UpdateValue()
        {
            MapManager manager = (MapManager)target;
            manager.UpdateValue(0);            
        }   

        public void CreateManager()
        {
            MapManager manager = (MapManager)target;

            Gameobject newManager = new Gameobject($"RoadManager{manager.roadManagers.Length-1}");
            newManager.transform.SetParent(manager.transform);
            newManager.AddComponent<RoadManager>();
            
            manager.roadManagers.Add(newManager.GetComponent<RoadManager>());
            newManager.UpdateValue(manager);
        }
    }
}
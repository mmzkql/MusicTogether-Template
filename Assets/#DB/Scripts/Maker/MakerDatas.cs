using UnityEngine;

namespace MusicTogether.DancingBall
{
    [Serializable]
    public class ManagerData
    {
        public InputNoteDatas clickNoteData;
        public int bpm;
        public NoteType noteProcessType;
        public List<int> clickNotes

        public Color normalColor,turnColor,styleColor,turnAndStyleColor;
        
    }
    [Serializable]
    public class BlockData
    {
        public GameObject blockPrefab,tapPlacer;
        public RoadPlacementStyle placementStyle;
        public float scale;
        public Vector2 anchor;
        public FacingDirecton tileDirection = FacingDirection.Down;
        public bool 
        public StyleData styleData;
    }
}
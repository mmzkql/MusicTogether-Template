using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace MusicTogether.DancingBall
{
    public class MapManager : MonoBehaviour
    {
        [Title("Necessities")]
        public PlayableDirector director;
        //public InputNoteDatas defaultClickNoteData;
        [Title("DefaultDatas")]
        //Manager
        //       --Data
        public ManagerData defaultManagerData;
        //public int defaultBPM;
        //public NoteType defaultNoteProcessType;
        //public List<int> defaultClickNotes;
        //       --Resource
        //public GameObject defaultBlockPrefab;
        //       --Debug
        public bool drawTrack,drawBlockMask;
        //public Color normalColor,turnColor,styleColor,turnAndStyleColor;
        //Block
        public BlockData defaultBlockData;
        //     --Data
        //public float defaultScale;
        //public RoadPlacementStyle defaultPlacementStyle;
        //public FacingDirecton defaultTileDirection = FacingDirection.Down;
        //public Vector2 defaultAnchor;

        //     --Resource
        //public GameObject defaultBlockPrefab,defaultTapPlacer;
        //public StyleData defaultStyleData;
        [Title("Manager")]
        public RoadManager[] roadManagers;
    }
}

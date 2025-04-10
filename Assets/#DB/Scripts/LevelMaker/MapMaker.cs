using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace MusicTogether.DancingBall
{
    public class MapMaker : MonoBehaviour
    {
        [Title("Necessities")]
        public PlayableDirector director;
        public static MapMaker Instance;
        [Title("DefaultDatas")]
        public bool drawTrack,drawBlockMask;
        
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public MusicData mapMusicData;
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public BlockStyleData mapStyleData;
        
        [Title("Debug")]
        public Color normalMaskColor,turnMaskColor,styleMaskColor,turnAndStyleMaskColor;
        [Title("Manager")]
        public RoadMaker[] roadManagers;
    }
}

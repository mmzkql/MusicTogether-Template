using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MusicTogether.DancingBall
{
    public class MapMaker : MonoBehaviour
    {
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
        public List<RoadMaker> roadManagers;
        public void UpdateManager(int beginIndex)
        {
            for (int i = beginIndex; i < roadManagers.Count; i++)
            {
                roadManagers[i].UpdateData(i);
            }
        }
    }
}
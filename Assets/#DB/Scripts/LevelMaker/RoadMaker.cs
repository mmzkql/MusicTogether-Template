using System;
using System.Collections.Generic;
using MusicTogether.General;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace MusicTogether.DancingBall
{
    public class RoadMaker : MonoBehaviour
    {
        //Dependencies
        public MapMaker mapMaker=>MapMaker.Instance;
        
        [EnumToggleButtons]public MusicDataInheritOption managerDataOption;
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public MusicData customMusicData;
        [FormerlySerializedAs("defaultRoadStyle")] [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public Block defaultRoad;
        //basicConfig
        public int noteBegin, noteEnd;
        public bool justifyPlacement;
        
        [Title("Manager")]
        public List<BlockMaker> nodes;
        
        
        
        
        
        
        
        
        
        [Button]
        public void UpdateValue()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].ResetValue(this, i);
                nodes[i].name = $"Block {i}";
                nodes[i].transform.SetSiblingIndex(i);
            }
        }
        public void UpdateBlockStyle(int begin)
        {
            if (childCount != transform.childCount)
            {
                UpdateValue();
            }
            for (int i = begin; i < nodes.Count; i++)
            {
                if (i < 0)
                    i = 0;
                nodes[i].UpdateStyle();
                
            }
        }
        public void UpdateBlockPosAndStyle(int begin)
        {
            if (childCount != transform.childCount)
            {
                UpdateValue();
            }
            for (int i = begin; i < nodes.Count; i++)
            {
                if (i < 0)
                    i = 0;
                nodes[i].UpdateStyle();
                
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                float length = Scale;//Mathf.Ceil(Nodes[i+1].nodeIndex - Nodes[i].nodeIndex) * Scale
                nodes[i+1].UpdatePosition(i, length);
            }
        }
    }
}


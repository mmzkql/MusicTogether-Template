using System;
using System.Collections.Generic;
using System.Linq;
using MusicTogether.General;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace MusicTogether.DancingBall
{
    public class RoadMaker : MonoBehaviour
    {
        //Dependencies
        private static MapMaker MapMaker=>MapMaker.Instance;
        public RoadHolder roadHolder;
        
        [EnumToggleButtons]public MusicDataInheritOption managerDataOption;

        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        [LabelText("customMusicData")]
        [ShowIf("@managerDataOption == MusicDataInheritOption.Custom")]
        private MusicData musicData
        {
            get
            {
                return roadHolder.musicData;
            }
            set
            {
                roadHolder.musicData = value;
            }
        }
        
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public BlockHolder defaultRoad;
        //basicConfig
        private int NoteBegin => roadHolder.noteBegin;
        private int NoteEnd=>roadHolder.noteEnd;
        public bool manageChildren ;
        
        [Title("Manager")]
        public List<BlockMaker> blockMakers;
        
        //MakingProcess
        public void UpdateData(int index)
        {
            int selfIndex = index;
            //int targetIndex=0;
            switch (managerDataOption)
            {
                case MusicDataInheritOption.Custom:
                    break;
                case MusicDataInheritOption.Last:
                    if (selfIndex > 0)
                    {
                        musicData = MapMaker.roadManagers[selfIndex - 1].musicData;
                    }
                    else
                    {
                        managerDataOption = MusicDataInheritOption.Map;
                        goto case MusicDataInheritOption.Map;
                    }
                    break;
                case MusicDataInheritOption.Map:
                    musicData = MapMaker.mapMusicData;
                    break;
            }

            UpdateBlockManagement();
        }
        public void UpdateData(int index,RoadHolder targetRoadHolder)
        {
            roadHolder = targetRoadHolder;
            UpdateData(index);
        }
        public void UpdateBlockManagement()
        {
            blockMakers.Clear();
            blockMakers = GetComponentsInChildren<BlockMaker>().ToList();
            //GetComponentsInChildren<BlockMaker>().ForEach(blockMaker => nodes.Add(blockMaker));
            blockMakers.Sort((a, b) => a.targetBlockHolder.nodeIndex.CompareTo(b.targetBlockHolder.nodeIndex));
            roadHolder.blockHodlers.Clear();
            blockMakers.ForEach(x => roadHolder.blockHodlers.Add(x.targetBlockHolder));
        }

        public void UpdateBlockData(int begin)
        {
            for (int i = begin; i < blockMakers.Count; i++)
            {
                blockMakers[i].UpdateBlockData(this,i);
            }
        }

        public void UpdateBlockPosAndStyle(int begin)
        {
            for (int i = begin; i < blockMakers.Count; i++)
            {
                blockMakers[i].UpdateBlockPosition(i);
                blockMakers[i].UpdateBlockStyle(i);
            }
        }
        
        
        
        
        
        
        /*[Button]
        public void UpdateValue()
        {
            for (int i = 0; i < blockMakers.Count; i++)
            {
                blockMakers[i].ResetValue(this, i);
                blockMakers[i].name = $"Block {i}";
                blockMakers[i].transform.SetSiblingIndex(i);
            }
        }
        public void UpdateBlockStyle(int begin)
        {
            if (childCount != transform.childCount)
            {
                UpdateValue();
            }
            for (int i = begin; i < blockMakers.Count; i++)
            {
                if (i < 0)
                    i = 0;
                blockMakers[i].UpdateStyle();
                
            }
        }
        public void UpdateBlockPosAndStyle(int begin)
        {
            if (childCount != transform.childCount)
            {
                UpdateValue();
            }
            for (int i = begin; i < blockMakers.Count; i++)
            {
                if (i < 0)
                    i = 0;
                blockMakers[i].UpdateStyle();
                
            }
            for (int i = 0; i < blockMakers.Count; i++)
            {
                float length = Scale;//Mathf.Ceil(Nodes[i+1].nodeIndex - Nodes[i].nodeIndex) * Scale
                blockMakers[i+1].UpdatePosition(i, length);
            }
        }*/
    }
}


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

        public int RoadIndex
        {
            get => roadHolder.roadIndex;
            set => roadHolder.roadIndex = value;
        }
        [EnumToggleButtons]public MusicDataInheritOption musicDataOption;
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

        public bool customRoadBlockHolder;
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public GameObject roadBlockHolderPrefab;
        //basicConfig
        private int NoteBegin => roadHolder.noteBegin;
        private int NoteEnd=>roadHolder.noteEnd;
        public bool manageChildren ;
        
        [Title("Manager")]
        public List<BlockMaker> blockMakers;
        
        //MakingProcess
        public BlockMaker CreateNewBlock(int targetIndex)
        {
            GameObject newBlock = new GameObject($"RoadHolder ({targetIndex})");
            newBlock.transform.SetParent(transform);
            newBlock.transform.localPosition = Vector3.zero;
            newBlock.transform.localRotation = Quaternion.identity;
            newBlock.transform.localScale = Vector3.one;
            
            BlockMaker maker = newBlock.AddComponent<BlockMaker>();
            BlockHolder holder = newBlock.transform.GetComponentInChildren<BlockHolder>();
            maker.UpdateBlockData(this,holder, targetIndex);
            
            return maker;
        }

        public void UpdateBlockList()
        {
            blockMakers.Clear();
            roadHolder.blockHodlers.Clear();
            
            blockMakers = GetComponentsInChildren<BlockMaker>().ToList();
            blockMakers.Sort((a,b)=>a.BlockIndex.CompareTo(b.BlockIndex));

            foreach (BlockMaker blockMaker in blockMakers)
            {
                blockMaker.gameObject.name = $"Block ({blockMaker.BlockIndex})";
                roadHolder.blockHodlers.Add(blockMaker.targetBlockHolder);
            }
        }
        
        public void UpdateData()
        {
            switch (musicDataOption)
            {
                case MusicDataInheritOption.Custom:
                    break;
                case MusicDataInheritOption.Last:
                    if (RoadIndex > 0)
                    {
                        musicData = MapMaker.roadMakers[RoadIndex - 1].musicData;
                    }
                    else
                    {
                        musicDataOption = MusicDataInheritOption.Map;
                        goto case MusicDataInheritOption.Map;
                    }
                    break;
                case MusicDataInheritOption.Map:
                    musicData = MapMaker.mapMusicData;
                    break;
            }

            if (!customRoadBlockHolder)
            {
                roadBlockHolderPrefab = MapMaker.mapBlockHolderPrefab;
            }
            UpdateBlockList();
        }
        public void UpdateData(int index)
        {
            RoadIndex = index;
            UpdateData();
        }
        public void UpdateData(int index,RoadHolder targetRoadHolder)
        {
            roadHolder = targetRoadHolder;
            UpdateData(index);
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
                blockMakers[i].UpdateBlockPosition();
                blockMakers[i].UpdateBlockStyle();
            }
        }

        [ButtonGroup("MakerTools")]
        public void UpdateBlockListTool()
        {
            UpdateBlockList();
        }
        [ButtonGroup("MakerTools")]
        public void UpdateBlockDataTool()
        {
            UpdateBlockData(0);
        }

        [ButtonGroup("MakerTools")]
        public void UpdateBlockPosAndStyleTool()
        {
            UpdateBlockPosAndStyle(0);
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


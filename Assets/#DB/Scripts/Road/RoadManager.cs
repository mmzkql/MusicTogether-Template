using System;
using System.Collections.Generic;
using MusicTogether.General;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MusicTogether.DancingBall
{
    public class RoadManager : MonoBehaviour
    {
        [Title("Generator")]
        public InputNoteDatas inputData;
        public NoteType noteProcessType;
        public int begin, end;

        [Title("Data")] public int BPM;
        public float Scale;
        public bool enableJustify = false;
        public GameObject RoadUnitPrefab;
        
        [Title("Manager")]
        public List<BlockNode> Nodes;
        public List<int> TurnIndexes;
        public List<int> StyleIndexes;
        //RuntimeData
        public MapManager mapManager;
        [HideInInspector]public int childCount;
        //public MoveBlock[] brickUnits;
        

        [Button]
        public void UpdateValue()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].ResetValue(this, i);
                Nodes[i].name = $"Block {i}";
                Nodes[i].transform.SetSiblingIndex(i);
            }
        }
        public void UpdateBlockStyle(int begin)
        {
            if (childCount != transform.childCount)
            {
                UpdateValue();
            }
            for (int i = begin; i < Nodes.Count; i++)
            {
                if (i < 0)
                    i = 0;
                Nodes[i].UpdateStyle();
                
            }
        }
        public void UpdateBlockPosAndStyle(int begin)
        {
            if (childCount != transform.childCount)
            {
                UpdateValue();
            }
            for (int i = begin; i < Nodes.Count; i++)
            {
                if (i < 0)
                    i = 0;
                Nodes[i].UpdateStyle();
                
            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                float Length = Scale;//Mathf.Ceil(Nodes[i+1].nodeIndex - Nodes[i].nodeIndex) * Scale
                Nodes[i+1].UpdatePosition(i, Length);
            }
        }
    }
}


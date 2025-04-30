using System;
using MusicTogether.General;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace MusicTogether.DancingBall
{
    public class Player : MonoBehaviour
    {
        public float ballScale;
        public LineRenderer lineRenderer;

        public MapHolder MapHolder=>MapHolder.Instance;
        public RoadHolder targetRoadHolder;
        public BlockHolder targetBlockHolder;
        public static Player Instance;
        
        private PlayableDirector Director=>MapHolder.director;
        private float NowTime => (float)MapHolder.NowTime;
        
        
        public int targetRoadIndex;
        public int targetBlockIndex;
        private int _nextRoadIndex;
        private int _nextBlockIndex;
        private int _targetNodeIndex;
        
        //RoadData
        private int _bpm;
        private float _unitTime,_scale;
        private NoteType _noteType;
        private int _managerNoteBegin;
        
        //BlockData
        private bool _teleportToNext;
        private bool _nextIsTurn;
        private Transform[] _nodes;
        
        //Value
        private TimeRange _moveTimeRange;
        private Vector3 _beginPoint = Vector3.zero;
        private Vector3 _displacementVector=Vector3.zero;

        
        void Move()
        {
            transform.position = _beginPoint + _displacementVector*_moveTimeRange.GetProgress(NowTime);
            //transform.position = _beginPoint + Direction * (speed * Mathf.Abs((_nowTime - beginTime)));
        }
        
        void GetNodeData()
        {
            _displacementVector = targetBlockHolder.nodes[_targetNodeIndex].position - _beginPoint;
            
            float beginTime = NowTime;
            float endTime = beginTime;
            if (targetBlockHolder.nodes.Count > 1)
            {
                endTime = (float)InputNoteDatas.GetTime(_bpm, _noteType, targetBlockHolder.nodeIndex + _managerNoteBegin) +
                                _unitTime * ((_targetNodeIndex - 0.5f) * 0.2f);
                //((_targetNodeIndex+0.5f)/targetBlock.nodes.Count-0.5f);
            }
            else
            {
                endTime = (float)InputNoteDatas.GetTime(_bpm, _noteType, targetBlockHolder.nodeIndex + _managerNoteBegin);
            }
            _moveTimeRange = new TimeRange(beginTime,endTime);
            
            if(false)
            {
                for (int i = 10; i > 1; i--)
                {
                    lineRenderer.SetPosition(i, lineRenderer.GetPosition(i - 1));
                }

                lineRenderer.SetPosition(1, _beginPoint);
                lineRenderer.SetPosition(0, _beginPoint + _displacementVector);
            }
        }
        void GetBlockData()
        {
            targetBlockHolder =targetRoadHolder.blockHodlers[targetBlockIndex];
            _teleportToNext = targetBlockHolder.teleport;
            GetNodeData();
        }

        void GetRoadData()
        {
            targetRoadHolder = MapHolder.roadHolders[targetRoadIndex];

            _bpm = targetRoadHolder.musicData.bpm;
            _noteType = targetRoadHolder.musicData.noteProcessType;
            _unitTime = (float)InputNoteDatas.GetTime(_bpm,_noteType,1);
            _managerNoteBegin = targetRoadHolder.noteBegin;
            
            GetBlockData();
        }

        private void Start()
        {
            targetRoadIndex = 0;
            targetBlockIndex = 0;
            _targetNodeIndex = 0;
            GetRoadData();
        }

        void Update()
        {
            if (targetBlockHolder.isTurn && _targetNodeIndex==0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    targetBlockHolder.tapPlacer.Tap();
                    GetNextNode(1);
                }
            }
            else if (_nextIsTurn)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    GetNextBlock(1);
                }
            }
            /*else if (targetManager.Nodes.Count > _targetBlockIndex+3 && targetManager.Nodes[_targetBlockIndex+2].isTurnNode)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    GetNextBlock(3);
                }
            }*/
            else if (NowTime > _moveTimeRange.endTime)
            {
                GetNextNode(1);
            }
            
            Move();
        }

        void GetNextNode(int step)
        {
            _targetNodeIndex+=step;
            DataManagement();
        }

        void GetNextBlock(int step)
        {
            _targetNodeIndex = 0;
            _nextBlockIndex = targetBlockIndex + step;
            DataManagement();
        }
        void DataManagement()
        {
            if(_targetNodeIndex<0)
                _targetNodeIndex=0;
            if (_targetNodeIndex > _nodes.Length - 1)
            {
                _targetNodeIndex=0;
                GetNextBlock(1);
                return;
            }

            if (!MapHolder.CorrectBlockIndex(_nextRoadIndex, _nextBlockIndex, out _nextRoadIndex,
                    out _nextBlockIndex))
            {
                return;
            }

            if (_nextRoadIndex != targetRoadIndex)
            {
                GetRoadData();
                targetRoadIndex = _nextRoadIndex;
                return;
            }

            if (_nextBlockIndex != targetBlockIndex)
            {
                GetBlockData();
                targetBlockIndex = _nextBlockIndex;
                return;
            }
            GetNodeData();
        }
    }
}

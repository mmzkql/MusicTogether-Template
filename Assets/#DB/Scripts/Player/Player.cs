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
        public MapManager mapManager;
        public float ballScale;
        public LineRenderer lineRenderer;
        //Level
        public PlayableDirector director;
        //manager
        public RoadManager targetManager;
        private int _targetManagerIndex;
        private int _bpm;
        private float _unitTime,_scale;
        private NoteType _noteType;
        private int _managerBegin;
        
        //block
        public BlockNode targetBlock;
        private int _targetBlockIndex;
        
        private bool _teleport;
        //node
        private Transform[] _nodes;
        private int _targetNodeIndex;
        //public float speed;
        //Value
        private TimeRange _timeRange;
        private Vector3 _beginPoint = Vector3.zero,_displacementVector=Vector3.zero;
        //private Vector3 _beginPoint= Vector3.zero,_endPoint= Vector3.zero;
        //private Vector3 Direction=>transform.forward;
        
        private float _nowTime=>(float)director.time;
        // Start is called before the first frame update
        void Move()
        {
            //Debug.Log($"{_beginPoint}+{Direction}*({_nowTime}-{beginTime})/{_unitTime}");
            transform.position = _beginPoint + _displacementVector*_timeRange.GetProgress(_nowTime);
            //transform.position = _beginPoint + Direction * (speed * Mathf.Abs((_nowTime - beginTime)));
        }

        void GetNodeData()
        {
            if(_teleport)
            {
                _beginPoint = targetBlock.nodes[0].position+ballScale*targetBlock.nodes[0].up;
            }
            else
            {
                _beginPoint = transform.position;
            }
            _displacementVector = targetBlock.nodes[_targetNodeIndex].position - _beginPoint;
            //_endPoint = targetBlock.nodes[_targetNodeIndex].position+ballScale*targetBlock.nodes[_targetNodeIndex].up;
            
            float beginTime = _nowTime;
            float endTime = (float)InputNoteDatas.GetTime(_bpm, _noteType, targetBlock.nodeIndex + _managerBegin) +
                      _unitTime * ((_targetNodeIndex - 0.5f) * 0.2f);//((_targetNodeIndex+0.5f)/targetBlock.nodes.Count-0.5f);
            _timeRange = new TimeRange(beginTime,endTime);

            //transform.LookAt(_endPoint);
            //speed = (_beginPoint-_endPoint).magnitude/(endTime-beginTime);
            
            for (int i = 10; i > 1; i--)
            {
                lineRenderer.SetPosition(i,lineRenderer.GetPosition(i-1));
            }
            lineRenderer.SetPosition(1,_beginPoint);
            lineRenderer.SetPosition(0,_beginPoint + _displacementVector);
        }
        void GetBlockData()
        {
            targetBlock =targetManager.Nodes[_targetBlockIndex];
            _teleport = targetBlock.isTeleportNode;
            GetNodeData();
        }

        void GetManagerData()
        {
            targetManager = mapManager.roadManagers[_targetManagerIndex];

            _bpm = targetManager.BPM;
            _noteType = targetManager.noteProcessType;
            _unitTime = (float)InputNoteDatas.GetTime(_bpm,_noteType,1);
            _scale= targetManager.Scale;
            _managerBegin = targetManager.begin;
            
            GetBlockData();
        }

        private void Start()
        {
            _targetManagerIndex = 0;
            _targetBlockIndex = 0;
            _targetNodeIndex = 0;
            GetManagerData();
            GetBlockData();
            GetNodeData();
        }

        void Update()
        {
            if (targetBlock.isTurnNode&&_targetNodeIndex == 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    targetBlock.tapPlacer.tap();
                    GetNextNode(1);
                }
            }
            /*else if (targetManager.Nodes.Count > _targetBlockIndex+2 && targetManager.Nodes[_targetBlockIndex+1].isTurnNode)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    GetNextBlock(2);
                }
            }*/
            /*else if (targetManager.Nodes.Count > _targetBlockIndex+3 && targetManager.Nodes[_targetBlockIndex+2].isTurnNode)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    GetNextBlock(3);
                }
            }*/
            else if (_nowTime > endTime)
            {
                GetNextNode(1);
            }

            //DataManagement();
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
            _targetBlockIndex += step;
            DataManagement();
        }
        void DataManagement()
        {
            if(_targetNodeIndex<0)
                _targetNodeIndex=0;
            if(_targetBlockIndex<0)
                _targetBlockIndex=0;
            if(_targetManagerIndex<0)
                _targetManagerIndex=0;
            if(targetBlock.nodes.Count > _targetNodeIndex)//if next node exists
            { 
                GetNodeData();
                return;
            }

            _targetNodeIndex = 0;
            _targetBlockIndex++;
            if (targetManager.Nodes.Count > _targetBlockIndex)//if next block exists
            {
                GetBlockData();
                //DataManagement();
                return;
            }
            _targetBlockIndex = 0;
            _targetManagerIndex++;
            if (mapManager.roadManagers.Length > _targetManagerIndex)
            {
                GetManagerData();
            }
            else
            {
                _targetManagerIndex = mapManager.roadManagers.Length - 1;
                GetManagerData();
            }
        }
    }
}

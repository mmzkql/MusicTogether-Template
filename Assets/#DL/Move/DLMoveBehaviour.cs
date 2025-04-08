using UnityEngine;
using UnityEngine.Playables;
using MusicTogether.TimelineKit;
using System.Collections.Generic;

namespace MusicTogether.DancingLine
{
    public class DLMoveBehaviour : BaseBehaviour
    {
        private DLController _player;
        private int _currentIndex = 0;
        private DLMoveClipAsset _data;
        private List<TurnData> turnDatas;
        private TurnData currentTurnData
        {
            get
            {
                return turnDatas[_currentIndex];
            }
        }

        private Vector3 getPosition(float speed,TurnData turnData,float NowTime)
        {
            Vector3 Forward = Quaternion.Euler(turnData.NowRotation) * Vector3.forward;
            //Debug.Log($"当前TimeLine时间{GlobalTime}-上次点击时间{turnData.ClickTime}*{Forward * speed}=线位置{ _player.transform.localPosition}");
            return (turnData.StartPoint + speed * (NowTime - turnData.ClickTime) * Forward);
        }
        private Vector3 getStartPosition(float speed, TurnData prevData, TurnData nowData)
        {
            Vector3 Forward = Quaternion.Euler(prevData.NowRotation) * Vector3.forward;
            //Debug.Log($"当前TimeLine时间{GlobalTime}-上次点击时间{prevData.ClickTime}*{Forward * speed}=线位置{ _player.transform.localPosition}");
            return (prevData.StartPoint + speed * (nowData.ClickTime - prevData.ClickTime) * Forward);
        }

        protected override void OnStart(object binding)
        {
            _player = binding as DLController;
            _data = GetData<DLMoveClipAsset>();
            if (_player == null || !_data)//应该只发生在倒退回起点的情况下
            {
                //Debug.LogError("DLPlayer binding is null!");
                return;
            }
            _player.moveBehaviour = this;
            _data.dlController = _player;
            turnDatas = _data.turnDatas;
            // 获取片段数据
            _currentIndex = 0;
            if (turnDatas == null || turnDatas.Count == 0)
            {
                //Debug.LogError("DLClipAsset data is invalid!");
                return;
            }

            // 初始化物体位置
            _player.transform.localPosition = turnDatas[0].StartPoint;
        }

        protected override void OnUpdate(object binding, float deltaTime)
        {
            if (_player == null)
                return;

            // 获取当前时间
            float currentTime = GlobalTime;

            // 获取片段数据
            if (_data == null || turnDatas == null)
                return;

            // 设置物体位置
            _player.transform.localPosition = getPosition(_data.Speed,currentTurnData,GlobalTime);

            // 检查是否需要转向
            while (_currentIndex < turnDatas.Count - 1 && currentTime >= turnDatas[_currentIndex + 1].ClickTime)
            {
                _currentIndex++;
                /*if (_data.CalculateBeginPoint)
                {
                    turnDatas[_currentIndex].StartPoint = getStartPosition(_data.Speed, turnDatas[_currentIndex - 1], currentTurnData);
                }*/
            }
            while (_currentIndex > 0&&currentTime <= turnDatas[_currentIndex].ClickTime)
            {
                _currentIndex--;
            }
        }

        public void GetInput(float inputTime,Vector3 rotation)
        {
            if(inputTime > turnDatas[^1].ClickTime)
            {
                TurnData turnData = new(inputTime, rotation, getPosition(_data.Speed, turnDatas[^1], inputTime));
                turnDatas.Add(turnData);
            }
            else if(inputTime > GlobalTime)
            {
                int i = _currentIndex;
                while(turnDatas[i].ClickTime < inputTime)
                {
                    if (i >= turnDatas.Count)
                        break;
                    i++;
                }
                TurnData turnData = new(inputTime, rotation, getPosition(_data.Speed, turnDatas[i - 1], inputTime));
                turnDatas.Insert(i, turnData);
            }
            else
            {
                int i = _currentIndex;
                while (turnDatas[i].ClickTime > inputTime)
                {
                    i--;
                    if (i < 0)
                        break;
                }
                TurnData turnData = new(inputTime, rotation, turnDatas[i + 1].StartPoint);
                turnDatas.Insert(i+1, turnData);
                RefreshStartPoint(i+2);
            }
        }
        public void RefreshStartPoint(int beginIndex)
        {
            int i = (beginIndex == 0) ? 1 : beginIndex;
            TurnData turnData,prevData;
            float Speed = _data.Speed;
            while(turnDatas[i].ClickTime < GlobalTime)
            {
                if (i >= turnDatas.Count)
                    return;
                turnData = turnDatas[i];
                prevData = turnDatas[i - 1];
                turnData.StartPoint = getStartPosition(Speed, prevData, turnData);
                i++;
            }
        }
        
        
        
        //Data
        public void RefreshTurnDatas(DLController _player)
        {
            List<InputData> playerInput = _player.Inputs;
            _data = GetData<DLMoveClipAsset>();
            List<TurnData> clipTurnDatas = _data.turnDatas;
            
            float start = GetClipTiming().Start, end = GetClipTiming().End;
            int inputIndex = playerInput.FindIndex((x)=>x.ClickTime>=start) - 1;
            if (inputIndex < 0)
                inputIndex = 0;
            
            clipTurnDatas.Clear();

            foreach(TurnData i in _data.PresetTurns)
            {
                clipTurnDatas.Add(i);
            }

            
            while (playerInput[inputIndex].ClickTime < end && inputIndex < playerInput.Count)
            {
                if (playerInput[inputIndex].ClickTime < start)
                {
                    inputIndex++;
                    continue;
                }
                InputData inputData = playerInput[inputIndex];
                TurnData nowData = new TurnData(inputData.ClickTime, inputData.NowRotation, Vector3.zero);
                clipTurnDatas.Add(nowData);
                //Debug.Log($"TurnData,{inputData.ClickTime}→{nowData.ClickTime},{inputData.NowRotation}→{nowData.NowRotation}");
                inputIndex++;
            }

            if (_data.GetBeginPointFromLastClip)
            {
                DLMoveClipAsset prevData = GetPreviousClip().asset as DLMoveClipAsset;
                TurnData prev = prevData.turnDatas[^1];
                clipTurnDatas[0].StartPoint = getPosition(prevData.Speed, prev, start);
            }

            RefreshStartPoint(0);
            /*
            for (int index = 1; index < clipTurnDatas.Count; index++)
            {
                clipTurnDatas[index].StartPoint = getStartPosition(_data.Speed, clipTurnDatas[index-1], clipTurnDatas[index]);
            }*/
        }
    }
}
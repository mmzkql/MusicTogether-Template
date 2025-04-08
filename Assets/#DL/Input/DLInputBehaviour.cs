using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MusicTogether.TimelineKit;

namespace MusicTogether.DancingLine
{
    public class DLInputBehaviour : BaseBehaviour
    {
        private bool _isForceType;
        private DLController _target;
        private ClipTiming _clipTiming;

        protected override void OnStart(object binding)
        {
            _target = binding as DLController;
            if (_target == null) return;

            // 通过GetData判断具体Clip类型
            /*if(GetData<DLInputClipAsset>())
            {
                GetData<DLInputClipAsset>().dlController = _target;//[修1]这应该是一个失败的设计，需要修改
                _isForceType = false;
            }
            else
            {
                GetData<DLForceInputClipAsset>().dlController = _target;//[修1]这应该是一个失败的设计，需要修改
                _isForceType = true;
            }*/
            _isForceType = GetData<DLInputClipAsset>().inputType == InputTypes.ForceInput;
            GetData<DLInputClipAsset>().dlController = _target;
                // 获取当前Clip的Timing参数
            _clipTiming = GetClipTiming();
        }

        protected override void OnUpdate(object binding, float deltaTime)
        {
            if (_target == null) return;

            // 判断当前时间是否在Clip范围内
            bool isActive = IsActive();

            // 根据Clip类型更新对应属性
            if (_isForceType)
            {
                _target.ForceInput = isActive;
            }
            else
            {
                _target.AllowInput = isActive;
            }

            // 如果当前Clip结束，检查下一个Clip是否需要提前激活
            /*if (!isActive && GlobalTime >= _clipTiming.End)
            {
                DLInputClipAsset nextClip = (DLInputClipAsset)GetNextClip().asset;
                if (nextClip != null)
                {
                    float nextClipStart = (float)nextClip.start;
                    if (GlobalTime >= nextClipStart)
                    {
                        // 如果当前时间已经进入下一个Clip的范围，提前激活
                        if (_isForceType)
                        {
                            _target.ForceInput = true;
                        }
                        else
                        {
                            _target.EnableInput = true;
                        }
                    }
                }
            }
            */
        }

        protected override void OnStop(object binding)
        {
            if (_target == null) return;

            // 结束时重置状态
            if (_isForceType)
            {
                _target.ForceInput = false;
            }
            else
            {
                _target.AllowInput = false;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            if(_target)
            {
                GetData<DLInputClipAsset>().dlController = _target;//[修1]这应该是一个失败的设计，需要修改
                _isForceType = false;
                /*if (GetData<DLInputClipAsset>())
                {
                    GetData<DLInputClipAsset>().dlController = _target;//[修1]这应该是一个失败的设计，需要修改
                    _isForceType = false;
                }
                else
                {
                    GetData<DLForceInputClipAsset>().dlController = _target;//[修1]这应该是一个失败的设计，需要修改
                    _isForceType = true;
                }*/
            }
        }
        private bool IsActive()
        {
            // 通过ClipTiming判断当前是否在有效区间
            return GlobalTime >= _clipTiming.Start && GlobalTime <= _clipTiming.End;
        }

        private void UpdateInputDirecion(DLInputClipAsset clipData)
        {
            if (!clipData.dlController || clipData.FreeRotation)
                return;

            List<InputData> playerInputs = clipData.dlController.Inputs;

            float startTime = GetClipTiming().Start, endTime = GetClipTiming().End;
            int StartIndex, EndIndex;
            StartIndex = playerInputs.FindLastIndex((x) => x.ClickTime < startTime);
            EndIndex = playerInputs.FindIndex((x) => x.ClickTime > endTime);
            if (StartIndex < 0) StartIndex = 0;
            if (EndIndex > playerInputs.Count) EndIndex = playerInputs.Count;

            Vector3 firstDirection = clipData.FirstDirection, secondDirection = clipData.SecondDirection;

            for(int i =0; StartIndex+i < EndIndex; i++)
            {
                if (playerInputs[i].ClickTime < startTime)
                    StartIndex++;
                playerInputs[i].NowRotation = i % 2 == 0 ? firstDirection : secondDirection;
            }
        }

        public bool Input_Advanced(float Time)
        {
            float startTime = GetClipTiming().Start, endTime = GetClipTiming().End;
            var clipData = GetData<DLInputClipAsset>();
            if (Time < startTime || Time > endTime)
            {
                return false;
            }
            List<InputData> playerInputs = clipData.dlController.Inputs;

            int InsertIndex = playerInputs.Count,clipbeginIndex = playerInputs.FindIndex((x) => x.ClickTime >= startTime);
            if (playerInputs[^1].ClickTime > Time)
            {
                InsertIndex = playerInputs.FindIndex((x) => x.ClickTime > Time);
            }
            var data = new InputData(Time);
            if (playerInputs[InsertIndex - 1].ClickTime < startTime)
            {    
                data.NowRotation = clipData.FirstDirection;
                //data.BeginPoint =         
            }
            else
                data.NowRotation = (InsertIndex - clipbeginIndex) % 2 == 0 ? clipData.FirstDirection : clipData.SecondDirection;
            return true;
        }
    }
}

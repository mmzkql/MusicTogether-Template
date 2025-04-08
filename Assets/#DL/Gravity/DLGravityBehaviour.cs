using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MusicTogether.TimelineKit;

namespace MusicTogether.DancingLine
{
    public class DLGravityBehaviour : BaseBehaviour
    {
        private DLController _controller;
        private Transform player;
        private Rigidbody playerRigibody;

        private DLGravityClipAsset _data;
        private GravitySimlationType _type;
        private float _height;
        private float a, b, c;
        private float currentTime;
        //private float beginTime,endTime, endPoint;

        protected override void OnStart(object binding)
        {

            _controller = binding as DLController;
            _data = GetData<DLGravityClipAsset>();

            if (_controller == null || !_data)//应该只发生在倒退回起点的情况下
            {
                //Debug.LogError("DLPlayer binding is null!");
                return;
            }
            player = _controller.transform.GetChild(0);
            playerRigibody = player.GetComponent<Rigidbody>();
            _type = _data.Type;

            Debug.Log($"时间{GlobalTime}，角速度{playerRigibody.angularVelocity}，线速度{playerRigibody.velocity}，位置{playerRigibody.position}");

            switch (_type)
            {
                case GravitySimlationType.Static:
                    playerRigibody.velocity = Vector3.zero;
                    playerRigibody.angularVelocity = Vector3.zero;
                    playerRigibody.useGravity = false;

                    _height = _data.Height;
                    break;
                case GravitySimlationType.FakeFall:
                    float h = GetClipTiming().Start, k = _data.BeginPoint;
                    a = _data.Gravity / 2;
                    b = -2 * a * h;
                    c = a * h * h + k;

                    _height = k;
                    break;
                case GravitySimlationType.FakeJump:
                    playerRigibody.velocity = Vector3.zero;
                    playerRigibody.angularVelocity = Vector3.zero;
                    playerRigibody.useGravity = false;

                    float x1 = GetClipTiming().Start, y1 = _data.BeginPoint,x2 = GetClipTiming().End,y2 = _data.EndPoint;
                    a = -1*_data.Power / 2;
                    b = (y1 - y2) / (x1 - x2) - a * (x1 + x2);
                    c = (x1 * y2 - x2 * y1) / (x1 - x2) + a * x1 * x2;
                    Debug.Log($"a:{a},b:{b},c:{c},begin({x1},{y1}),end({x2},{y2})");

                    _height = y1;
                    break;
                case GravitySimlationType.RealMotion:
                    playerRigibody.useGravity = true;

                    _height = _data.Height;
                    break;
            }
            player.localPosition = new Vector3(0, _height, 0);
        }
        protected override void OnUpdate(object binding, float deltaTime)
        {
            if (_type == GravitySimlationType.Static)
                return;
            if (_controller == null)
                return;

            switch(_type)
            {
                case GravitySimlationType.FakeJump:
                    currentTime = GlobalTime;
                    player.localPosition = new Vector3(0, (a * currentTime + b) * currentTime + c, 0);
                    break;
                case GravitySimlationType.FakeFall:
                    currentTime = GlobalTime;
                    player.localPosition = new Vector3(0, (a * currentTime + b) * currentTime + c, 0);
                    break;
            }
        }
    }
}
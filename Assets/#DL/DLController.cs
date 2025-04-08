using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using MusicTogether.General;

namespace MusicTogether.DancingLine
{
    public enum InputTypes { NormalInput, ForceInput, AutoInput }
    [System.Serializable]
    public class InputData
    {
        [HorizontalGroup("Data", Width = 70), LabelWidth(30), LabelText("Time")]
        public float ClickTime; // 点击时间（转向时间）
        [HorizontalGroup("Data", Width = 160), LabelWidth(48), LabelText("Rotation")]
        public Vector3 NowRotation; // 当前方向
        [ReadOnly]public Vector3 BeginPoint;
        public InputData(float ClickTime)
        {
            this.ClickTime = ClickTime;
            this.NowRotation = Vector3.zero;
            this.BeginPoint = Vector3.zero;
        }
        public InputData(float ClickTime, Vector3 NowRotation)
        {
            this.ClickTime = ClickTime;
            this.NowRotation = NowRotation;
            this.BeginPoint = Vector3.zero;
        }
        public InputData(float ClickTime, Vector3 NowRotation, Vector3 beginPoint)
        {
            this.ClickTime = ClickTime;
            this.NowRotation = NowRotation;
            this.BeginPoint = beginPoint;
        }
    }
    public class DLController : MonoBehaviour
    {
        //�������(�Ӵ�С)
        public PlayableDirector playableDirector;

        //[HideInInspector] public DLMoveTrackAsset moveTrackAsset;
        public DLMoveBehaviour moveBehaviour;

        private Transform selfTransform;
        private BoxCollider characterCollider;

        //����(���⵽��)
        //-ת��:����
        [ShowInInspector,ReadOnly] public bool AllowInput { get; set; }
        [ShowInInspector,ReadOnly] public bool ForceInput { get; set; }
        public InputNoteDatas AutoInputData;
        [FormerlySerializedAs("AutoInputs")] public List<InputData> Inputs;

        //-ת��:�ж�
        private bool previousFrameIsGrounded;
        private const float groundedRayDistance = 0.05f;
        private ValueTuple<Vector3, Ray>[] groundedTestRays;
        private readonly RaycastHit[] groundedTestResults = new RaycastHit[1];
        public bool Landed
        {
            get
            {
                for (var i = 0; i < groundedTestRays.Length; i++)
                {
                    groundedTestRays[i].Item2.origin = selfTransform.position +
                                                       selfTransform.localRotation * groundedTestRays[i].Item1;
                    if (Physics.RaycastNonAlloc(groundedTestRays[i].Item2, groundedTestResults,
                            groundedRayDistance + 0.1f, -257, QueryTriggerInteraction.Ignore) > 0)
                        return true;
                }

                return false;
            }
        }
        //-ת��:����
        public Vector3 firstDirection = new(0,0,0), secondDirection = new(0, 90, 0);
        private int nowDirectionIndex = 0;
        private Vector3 nowDirection
        {
            get
            {
                if (nowDirectionIndex == 0)
                    return firstDirection;
                else
                    return secondDirection;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            //playableDirector = GetComponentInParent<PlayableDirector>();

            //����֮��Ҫд����ȡselfTransform
            //����֮��Ҫд����ȡcharacterCollider
            groundedTestRays = new ValueTuple<Vector3, Ray>[]
            {
                new(characterCollider.center - new Vector3(characterCollider.size.x * 0.5f,
                        characterCollider.size.y * 0.5f - 0.1f, characterCollider.size.z * 0.5f),
                    new Ray(Vector3.zero, selfTransform.localRotation * Vector3.down)),
                new(characterCollider.center - new Vector3(characterCollider.size.x * -0.5f,
                        characterCollider.size.y * 0.5f - 0.1f, characterCollider.size.z * 0.5f),
                    new Ray(Vector3.zero, selfTransform.localRotation * Vector3.down)),
                new(characterCollider.center - new Vector3(characterCollider.size.x * 0.5f,
                        characterCollider.size.y * 0.5f - 0.1f, characterCollider.size.z * -0.5f),
                    new Ray(Vector3.zero, selfTransform.localRotation * Vector3.down)),
                new(characterCollider.center - new Vector3(characterCollider.size.x * -0.5f,
                        characterCollider.size.y * 0.5f - 0.1f, characterCollider.size.z * -0.5f),
                    new Ray(Vector3.zero, selfTransform.localRotation * Vector3.down))
            };
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void Input(float inputTime)
        {
            if(ForceInput || (AllowInput&&Landed))
            {
                if (++nowDirectionIndex > 1)
                    nowDirectionIndex = 0;
                moveBehaviour.GetInput(inputTime,nowDirection);
            }
        }
        [Button]
        public void AddAutoTime()
        {
            if (++nowDirectionIndex > 1)
                nowDirectionIndex = 0;
            Inputs.Add(new InputData((float)playableDirector.time, nowDirection));
        }
        [Button]
        public void ReorderTimes()
        {
            Inputs.Sort((a, b) => a.ClickTime.CompareTo(b.ClickTime));
        }
        [Button]
        public void InportData(InputNoteDatas inputData)
        {
            Inputs.Clear();
            foreach(float i in inputData.InputTimes)
            {
                Inputs.Add(new InputData(i, Vector3.zero));
            }
        }
    }
}

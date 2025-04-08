using UnityEngine;
using Sirenix.OdinInspector;
using MusicTogether.TimelineKit;

namespace MusicTogether.DancingLine
{
    public enum GravitySimlationType { Static, RealMotion,FakeFall, FakeJump }
    public enum FakeFallCalculateTarget { EndTime,EndHeight}
    [System.Serializable]
    public class DLGravityClipAsset : BaseClipAsset<DLGravityBehaviour>
    {
        [EnumToggleButtons]
        public GravitySimlationType Type;
        [EnumToggleButtons,ShowIf("@Type == GravitySimlationType.FakeFall")]
        public FakeFallCalculateTarget AutoSetValue;
        //[ShowIf("@Type==GravitySimlationType.Static")]
        public bool Inherit = true;
        [HideInInspector] 
        public bool InheritOnStart = false;

        [ShowIf("@Type==GravitySimlationType.Static && !InheritOnStart"),DisableIf("@Inherit")]//,HideIf("@InheritOnStart")
        public float Height;
        [ShowIf("@Type==GravitySimlationType.Static && InheritOnStart"),ReadOnly,LabelText("Height")]
        public string UnknownText = "???";

        //[ShowIf("@Type!=GravitySimlationType.Static")]
        //public float JumpPower;
        [ShowIf("@Type==GravitySimlationType.FakeFall")]
        public float Gravity = -9.8f;
        [ShowIf("@Type==GravitySimlationType.FakeJump")]
        public float Power = 9.8f;

        [ShowIf("@(Type==GravitySimlationType.FakeFall || Type==GravitySimlationType.FakeJump)&& !InheritOnStart"),DisableIf("@Inherit")]
        public float BeginPoint;
        [ShowIf("@(Type==GravitySimlationType.FakeFall || Type==GravitySimlationType.FakeJump)&& InheritOnStart"), ReadOnly,LabelText("BeginPoint")]
        public string UnknownText2 = "???";

        [ShowIf("@Type==GravitySimlationType.FakeFall || Type==GravitySimlationType.FakeJump"),DisableIf("@AutoSetValue == FakeFallCalculateTarget.EndHeight")]
        public float EndPoint;
    }
}

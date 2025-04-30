using System.Collections;
using System.Collections.Generic;
using MusicTogether.General;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace MusicTogether.DancingBall
{
    public class TapPlacer : MonoBehaviour
    {
        
        [TabGroup("PrefabSettings")]
        public MeshRenderer[] circleCorners;
        [TabGroup("PrefabSettings")]
        public MeshRenderer tapEffect;
        
        private static PlayableDirector Director=>MapHolder.Instance.director;
        private double NowTime => Director.time;
        
        //public BlockStyleData blockStyleData;
        public TapPlacerData tapPlacerData;
        
        private float BeginRadius=>tapPlacerData.beginRadius;
        private TimeRange TapTimeRange => tapPlacerData.tapTimeRange;
        private AnimationCurve RadiusCurve=>tapPlacerData.radiusCurve;
        private Gradient TapColorGradient=>tapPlacerData.tapColorGradient;
        private float FadeTime=>tapPlacerData.fadeTime;
        private AnimationCurve FadeAlphaCurve=>tapPlacerData.fadeAlphaCurve;
        
        
        private int noteIndex;
        private TimeRange _tapTimeRange = new TimeRange(0,0);
        private float _angle;
        private float _tapTime;
        private float Value => _tapTimeRange.GetProgress((float)NowTime);

        private bool _enabled = false,_tapped = false;
        
        //private MaterialPropertyBlock circlePropBlock = new MaterialPropertyBlock(),tapPropBlock= new MaterialPropertyBlock();
        private Color tapColor;
        // Start is called before the first frame update
        public void StartTap(BlockHolder blockHolder,int bpm,NoteType noteType)
        {
            noteIndex=blockHolder.nodeIndex + blockHolder.roadHolder.noteBegin;

            //note = blockNode.nodeIndex + blockNode.nodeManager.begin;
            _angle = 360/circleCorners.Length;
            _tapTimeRange.startTime = (float)InputNoteDatas.GetTime(bpm, noteType, TapTimeRange.startTime+ noteIndex);//note+styleData.tapTimeRange.startTime
            _tapTimeRange.endTime = (float)InputNoteDatas.GetTime(bpm, noteType, TapTimeRange.endTime+ noteIndex);//note+styleData.tapTimeRange.endTime
            
            //circleCorners[0].GetPropertyBlock(circlePropBlock);
            //tapEffect.GetPropertyBlock(tapPropBlock);
            //circlePropBlock.SetColor("_Color", tapColor);
            //tapPropBlock.SetColor("_Color", tapColor);
            
            /*for (int i = 0; i < circleCorners.Length; i++)
            {
                circleCorners[i].SetPropertyBlock(circlePropBlock);
            }*/
            //tapEffect.SetPropertyBlock(tapPropBlock);
            
            tapEffect.gameObject.SetActive(false);
            _enabled=true;
            //Debug.Log($"{blockStyleData.GetInstanceID()},{blockStyleData.tapTimeRange.GetHashCode()},{blockStyleData.tapTimeRange.startTime}");
        }

        void Update()
        {
            if (!_enabled) return;
            if(NowTime < _tapTimeRange.startTime)return;
            
            for (int i = 0; i < circleCorners.Length; i++)
            {
                circleCorners[i].transform.localEulerAngles = new Vector3(0, i*_angle, 0);
                circleCorners[i].gameObject.SetActive(true);
                Debug.Log("SetActive");
            }

            if (_tapped)
            {
                //tapColor = styleData.tapColorGradient.Evaluate(Mathf.Abs(value));
                //tapColor = new Color(tapColor.r, tapColor.g, tapColor.b, 1 - Mathf.Clamp01((float)((nowTime - tapTime)/fadeTime)));
                tapColor.a = FadeAlphaCurve.Evaluate((float)((NowTime - _tapTime) / FadeTime)); //1 - Mathf.Clamp01((float)((nowTime - tapTime) / fadeTime));
                //Debug.Log((nowTime - tapTime)/fadeTime);
                MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
                circleCorners[0].GetPropertyBlock(propBlock);
                // 设置颜色属性
                propBlock.SetColor("_Color", tapColor);
                for (int i = 0; i < circleCorners.Length; i++)
                {
                    circleCorners[i].SetPropertyBlock(propBlock);
                }
                tapEffect.SetPropertyBlock(propBlock);
            }
            else
            {
                float radius = Mathf.Abs(BeginRadius * (1 - Value));
                tapColor = TapColorGradient.Evaluate(Mathf.Abs(1-Mathf.Abs(1-Value)));
            
                MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
                circleCorners[0].GetPropertyBlock(propBlock);
                // 设置颜色属性
                propBlock.SetColor("_Color", tapColor);
                // 应用PropertyBlock
                for (int i = 0; i < circleCorners.Length; i++)
                {
                    float selfAngle = 135-(i * _angle);
                    float angleRad = selfAngle * Mathf.Deg2Rad;
                    circleCorners[i].transform.localPosition = new Vector3(radius * Mathf.Cos(angleRad), 0f, radius * Mathf.Sin(angleRad));
                    circleCorners[i].SetPropertyBlock(propBlock);
                }
                tapEffect.SetPropertyBlock(propBlock);
            }
            //circlePropBlock.SetColor("_Color", tapColor);
            //tapPropBlock.SetColor("_Color", tapColor);
        }

        public void Tap()
        {
            _tapped = true;
            //Color tapColor = styleData.tapColorGradient.Evaluate(value);
            //MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            //tapEffect.GetPropertyBlock(propBlock);
            //propBlock.SetColor("_Color", tapColor);
            //tapEffect.SetPropertyBlock(propBlock);
            tapEffect.gameObject.SetActive(true);
            _tapTime = (float)NowTime;
        }
    }
}

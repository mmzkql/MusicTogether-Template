using System.Collections;
using System.Collections.Generic;
using MusicTogether.General;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Playables;

namespace MusicTogether.DancingBall
{
    public class TapPlacer : MonoBehaviour
    {
        public StyleData styleData;
        public MeshRenderer[] circleCorners;
        public MeshRenderer tapEffect;
        
        private PlayableDirector director;
        private double nowTime => director.time;
        
        private Gradient tapColorGradient;
        public int noteIndex;
        private TimeRange tapTimeRange = new TimeRange(0,0);
        private float fadeTime;
        private AnimationCurve _fadeAlpha;
        private float beginRadius;
        private float angle;
        private float tapTime;
        private float value => tapTimeRange.GetProgress((float)nowTime);

        private bool enabled = false,tapped = false;
        
        //private MaterialPropertyBlock circlePropBlock = new MaterialPropertyBlock(),tapPropBlock= new MaterialPropertyBlock();
        public Color tapColor;
        // Start is called before the first frame update
        public void StartTap(BlockNode blockNode,int BPM,NoteType noteType,int note)
        {
            noteIndex=blockNode.nodeIndex + blockNode.nodeManager.begin;
            //note = blockNode.nodeIndex + blockNode.nodeManager.begin;
            director = blockNode.nodeManager.mapManager.director;
            
            tapColorGradient = styleData.tapColorGradient;
            
            tapTimeRange.startTime = styleData.tapTimeRange.startTime;
            tapTimeRange.endTime = styleData.tapTimeRange.endTime;
            fadeTime = styleData.fadeTime;
            _fadeAlpha = styleData.fadeAlpha;
            
            beginRadius = styleData.beginRadius;
            angle = 360/circleCorners.Length;
            tapTimeRange.startTime = (float)InputNoteDatas.GetTime(BPM, noteType, noteIndex+styleData.tapTimeRange.startTime);//note+styleData.tapTimeRange.startTime
            tapTimeRange.endTime = (float)InputNoteDatas.GetTime(BPM, noteType, noteIndex+styleData.tapTimeRange.endTime);//note+styleData.tapTimeRange.endTime
            
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
            enabled=true;
            Debug.Log($"{styleData.GetInstanceID()},{styleData.tapTimeRange.GetHashCode()},{styleData.tapTimeRange.startTime}");
        }

        void Update()
        {
            if (!enabled) return;
            if(nowTime < tapTimeRange.startTime)return;
            
            for (int i = 0; i < circleCorners.Length; i++)
            {
                circleCorners[i].transform.localEulerAngles = new Vector3(0, i*angle, 0);
                circleCorners[i].gameObject.SetActive(true);
                Debug.Log("SetActive");
            }

            if (tapped)
            {
                //tapColor = styleData.tapColorGradient.Evaluate(Mathf.Abs(value));
                //tapColor = new Color(tapColor.r, tapColor.g, tapColor.b, 1 - Mathf.Clamp01((float)((nowTime - tapTime)/fadeTime)));
                tapColor.a = _fadeAlpha.Evaluate((float)((nowTime - tapTime) / fadeTime)); //1 - Mathf.Clamp01((float)((nowTime - tapTime) / fadeTime));
                //Debug.Log((nowTime - tapTime)/fadeTime);
                //tapColor.a = 1 - Mathf.Clamp01((float)((nowTime - tapTime)/fadeTime));
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
                float radius = Mathf.Abs(beginRadius * (1 - value));
                tapColor = styleData.tapColorGradient.Evaluate(Mathf.Abs(1-Mathf.Abs(1-value)));
            
                MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
                circleCorners[0].GetPropertyBlock(propBlock);
                // 设置颜色属性
                propBlock.SetColor("_Color", tapColor);
                // 应用PropertyBlock
                for (int i = 0; i < circleCorners.Length; i++)
                {
                    float selfAngle = 135-(i * angle);
                    float angleRad = selfAngle * Mathf.Deg2Rad;
                    circleCorners[i].transform.localPosition = new Vector3(radius * Mathf.Cos(angleRad), 0f, radius * Mathf.Sin(angleRad));
                    circleCorners[i].SetPropertyBlock(propBlock);
                }
                tapEffect.SetPropertyBlock(propBlock);
            }
            //circlePropBlock.SetColor("_Color", tapColor);
            //tapPropBlock.SetColor("_Color", tapColor);
        }

        public void tap()
        {
            tapped = true;
            //Color tapColor = styleData.tapColorGradient.Evaluate(value);
            //MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            //tapEffect.GetPropertyBlock(propBlock);
            //propBlock.SetColor("_Color", tapColor);
            //tapEffect.SetPropertyBlock(propBlock);
            tapEffect.gameObject.SetActive(true);
            tapTime = (float)nowTime;
        }
    }
}

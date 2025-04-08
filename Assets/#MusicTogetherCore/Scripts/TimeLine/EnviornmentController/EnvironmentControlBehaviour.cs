using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class EnvironmentControlBehaviour : PlayableBehaviour
{
    //public float duration;
    //public Ease ease;
    public bool useFog;
    public FogMode fogMode;
    public Color fogColor;
    public float density;
    public float startDistance;
    public float endDistance;
    public ExposedReference<Camera> targetCamera;
}
//    public override void OnBehaviourPlay(Playable playable, FrameData info)
//    {
//        var seq = DOTween.Sequence();
//        seq.Append(DOTween
//            .To(() => RenderSettings.fogColor, x => RenderSettings.fogColor = x, fogColor, duration).SetEase(ease)).OnComplete(()=> { RenderSettings.fog = useFog; });
//        seq.Join(DOTween.To(() => RenderSettings.fogStartDistance, x => RenderSettings.fogStartDistance = x,
//            start, duration).SetEase(ease));
//        seq.Join(DOTween.To(() => RenderSettings.fogEndDistance, x => RenderSettings.fogEndDistance = x, end,
//            duration).SetEase(ease));
//        foreach(Camera c in targetCamera)
//        {
//            seq.Join(DOTween.To(() => c.backgroundColor, x => c.backgroundColor = x, fogColor, duration).SetEase(ease));
//        }
//#if UNITY_EDITOR
//        RenderSettings.fogColor = fogColor;
//        RenderSettings.fogStartDistance = start;
//        RenderSettings.fogEndDistance = end;
//        RenderSettings.fog = useFog;
//#endif
//        //DOTween.To(() => camera.backgroundColor, x => camera.backgroundColor = x, fogColor, duration).SetEase(ease);
//    }

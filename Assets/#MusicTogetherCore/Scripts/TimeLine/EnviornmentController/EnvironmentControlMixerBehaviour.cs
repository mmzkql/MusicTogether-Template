using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class EnvironmentControlMixerBehaviour : PlayableBehaviour
{
    bool DeafultEnabled;
    Color DeafultColor;
    FogMode DeafultFogMode;
    float DeafultDensity;
    float DeafultStartDistance;
    float DeafultEndDistance;
    List<Camera> TargetCamera = new();
    bool FirstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!FirstFrameHappened)
        {
            DeafultEnabled = RenderSettings.fog;
            DeafultFogMode = RenderSettings.fogMode;
            DeafultColor = RenderSettings.fogColor;
            DeafultDensity = RenderSettings.fogDensity;
            DeafultStartDistance = RenderSettings.fogStartDistance;
            DeafultEndDistance = RenderSettings.fogEndDistance;

            FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();
        bool currentEnabled = false;
        FogMode currentMode;
        Color blendedColor = Color.clear;

        float blendDensity = 0f;
        float blendDeafultStartDistance = 0f;
        float blendDeafultEndDistance = 0f;

        float totalWeight = 0f;
        int greatestIndex = 0;
        float greatestWeight = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<EnvironmentControlBehaviour> inputPlayable = (ScriptPlayable<EnvironmentControlBehaviour>)playable.GetInput(i);
            EnvironmentControlBehaviour input = inputPlayable.GetBehaviour();

            currentEnabled = input.useFog | currentEnabled;
            blendedColor += input.fogColor * inputWeight;
            blendDensity += input.density;
            blendDeafultStartDistance += input.startDistance;
            blendDeafultEndDistance += input.endDistance;

            totalWeight += inputWeight;
            if (inputWeight > greatestWeight)
            {
                greatestWeight = inputWeight;
                greatestIndex = i;
            }
            Camera targetCamera = input.targetCamera.Resolve((playable.GetGraph().GetResolver()));
            if (!TargetCamera.Exists((X) => X== targetCamera))
            {
                TargetCamera.Add(targetCamera);
            }
        }
        ScriptPlayable<EnvironmentControlBehaviour> greatestInputPlayable = (ScriptPlayable<EnvironmentControlBehaviour>)playable.GetInput(greatestIndex);
        EnvironmentControlBehaviour greatestInput = greatestInputPlayable.GetBehaviour();
        RenderSettings.fog = currentEnabled;
        RenderSettings.fogMode = greatestInput.fogMode;
        RenderSettings.fogColor = blendedColor;
        RenderSettings.fogDensity = blendDensity;
        RenderSettings.fogStartDistance = blendDeafultStartDistance;
        RenderSettings.fogEndDistance = blendDeafultEndDistance;
        foreach (Camera i in TargetCamera)
        {
            i.backgroundColor = blendedColor;
        }
    }
    public override void OnPlayableDestroy(Playable playable)
    {
        FirstFrameHappened = false;

        RenderSettings.fog = DeafultEnabled;
        RenderSettings.fogMode = DeafultFogMode;
        RenderSettings.fogColor = DeafultColor;
        RenderSettings.fogDensity = DeafultDensity;
        RenderSettings.fogStartDistance = DeafultStartDistance;
        RenderSettings.fogEndDistance = DeafultEndDistance;
    }
    
//    public override void OnBehaviourPlay(Playable playable, FrameData info)
//    {
//        //var seq = DOTween.Sequence();
//        //seq.Append(DOTween
//        //    .To(() => RenderSettings.fogColor, x => RenderSettings.fogColor = x, fogColor, duration).SetEase(ease)).OnComplete(()=> { RenderSettings.fog = useFog; });
//        //seq.Join(DOTween.To(() => RenderSettings.fogStartDistance, x => RenderSettings.fogStartDistance = x,
//        //    start, duration).SetEase(ease));
//        //seq.Join(DOTween.To(() => RenderSettings.fogEndDistance, x => RenderSettings.fogEndDistance = x, end,
//        //    duration).SetEase(ease));
//        //foreach(Camera c in targetCamera)
//        //{
//        //    seq.Join(DOTween.To(() => c.backgroundColor, x => c.backgroundColor = x, fogColor, duration).SetEase(ease));
//        //}
//#if UNITY_EDITOR
//        RenderSettings.fogColor = fogColor;
//        RenderSettings.fogStartDistance = start;
//        RenderSettings.fogEndDistance = end;
//        RenderSettings.fog = useFog;
//#endif
//        //DOTween.To(() => camera.backgroundColor, x => camera.backgroundColor = x, fogColor, duration).SetEase(ease);
//    }
}

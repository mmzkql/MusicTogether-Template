using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class EnvironmentControlClip : PlayableAsset, ITimelineClipAsset
{
    //public bool useFog = true;
    //public Color fogColor = Color.white;
    //public float start = 25f;
    //public float end = 120f;
    //public Camera[] targetCamera;

    public EnvironmentControlBehaviour template = new EnvironmentControlBehaviour();
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EnvironmentControlBehaviour>.Create(graph, template);
        return playable;
    }

    //public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    //{
    //    var environmentChanger = ScriptPlayable<EnvironmentControllerBehaviour>.Create(graph);
    //    environmentChanger.GetBehaviour().useFog = useFog;
    //    environmentChanger.GetBehaviour().fogColor = fogColor;
    //    environmentChanger.GetBehaviour().start = start;
    //    environmentChanger.GetBehaviour().end = end;
    //    environmentChanger.GetBehaviour().targetCamera = targetCamera;
    //    return environmentChanger;
    //}
}

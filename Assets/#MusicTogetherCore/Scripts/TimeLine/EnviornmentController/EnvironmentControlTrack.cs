using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[TrackClipType(typeof(EnvironmentControlClip))]
public class EnvironmentControlAsset : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<EnvironmentControlMixerBehaviour>.Create(graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
//#if UNITY_EDITOR
//        Light trackBinding = director.GetGenericBinding(this) as Light;
//        if (trackBinding == null)
//            return;
//        driver.AddFromName<Light>(trackBinding.gameObject, "m_Color");
//        driver.AddFromName<Light>(trackBinding.gameObject, "m_Intensity");
//        driver.AddFromName<Light>(trackBinding.gameObject, "m_Range");
//        driver.AddFromName<Light>(trackBinding.gameObject, "m_BounceIntensity");
//#endif
//        base.GatherProperties(director, driver);
    }
}

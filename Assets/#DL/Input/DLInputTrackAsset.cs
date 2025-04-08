using UnityEngine.Timeline;
using MusicTogether.TimelineKit;

namespace MusicTogether.DancingLine
{
    [TrackColor(0.6f, 0.8f, 0.1f)]
    [TrackBindingType(typeof(DLController))]
    [TrackClipType(typeof(DLInputClipAsset))]
    //[TrackClipType(typeof(DLForceInputClipAsset))]
    public class DLInputTrackAsset : BaseTrackAsset
    {

    }
}


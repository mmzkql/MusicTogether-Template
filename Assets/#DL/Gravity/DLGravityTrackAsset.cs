using UnityEngine.Timeline;
using MusicTogether.TimelineKit;

namespace MusicTogether.DancingLine
{
    [TrackColor(0.333f, 0.722f, 1f)]
    [TrackClipType(typeof(DLGravityClipAsset))]
    [TrackBindingType(typeof(DLController))]
    public class DLGravityTrackAsset : BaseTrackAsset<DLGravityBehaviour>
    {
    }
}


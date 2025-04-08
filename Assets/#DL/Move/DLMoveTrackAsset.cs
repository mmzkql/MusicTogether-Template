using UnityEngine;
using UnityEngine.Timeline;
using MusicTogether.TimelineKit;

namespace MusicTogether.DancingLine
{
    [TrackColor(0.5f, 0.5f, 1f)]
    [TrackClipType(typeof(DLMoveClipAsset))]
    [TrackBindingType(typeof(DLController))]
    public class DLMoveTrackAsset : BaseTrackAsset<DLMoveBehaviour>
    {
    }
}
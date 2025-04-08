using System.Linq;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace MusicTogether.DancingLine
{
    [CustomTimelineEditor(typeof(DLGravityClipAsset))]
    public class DLGravityClipAssetEditor : ClipEditor
    {
        // Start is called before the first frame update
        public override void OnClipChanged(TimelineClip clip)
        {
            var clipAsset = clip.asset as DLGravityClipAsset;
            GravitySimlationType type = clipAsset.Type;
            if (clipAsset != null)
            {
                if (clipAsset.Inherit)
                {
                    var track = clip.GetParentTrack();
                    if (track != null)
                    {
                        var clips = track.GetClips().ToList();
                        int index = clips.IndexOf(clip);
                        clipAsset.InheritOnStart = false;
                        if (index > 0)
                        {
                            var previousClip = clips[index - 1];
                            var previousAsset = previousClip.asset as DLGravityClipAsset;
                            if (previousAsset != null)
                            {
                                GravitySimlationType prevType = previousAsset.Type;
                                if (prevType == GravitySimlationType.Static)
                                {
                                    clipAsset.Height = previousAsset.Height;
                                    clipAsset.BeginPoint = previousAsset.Height;
                                }
                                else if (prevType == GravitySimlationType.RealMotion)
                                {
                                    clipAsset.InheritOnStart = true;
                                }
                                else
                                {
                                    clipAsset.Height = previousAsset.EndPoint;
                                    clipAsset.BeginPoint = previousAsset.EndPoint;
                                }
                            }
                            else
                            {
                                clipAsset.Height = 0f;
                                clipAsset.BeginPoint = 0f;
                            }
                        }
                        else
                        {
                            clipAsset.Inherit = false;
                        }
                    }
                }
                else
                {
                    clipAsset.InheritOnStart = false;
                }
                switch (type)
                {
                    case GravitySimlationType.RealMotion:
                        clip.displayName = $"RealMotion:{clipAsset.Height}¡ú??";
                        break;
                    case GravitySimlationType.FakeJump:
                        clip.displayName = $"FakeJump:{clipAsset.BeginPoint}¡ú{clipAsset.EndPoint}";
                        break;
                    case GravitySimlationType.FakeFall:
                        if(clipAsset.AutoSetValue == FakeFallCalculateTarget.EndTime)
                            clip.duration = Mathf.Pow(Mathf.Abs(2 * (clipAsset.BeginPoint - clipAsset.EndPoint) / clipAsset.Gravity), 0.5f);
                        else
                        {
                            //float t = (float)clip.end;
                            clipAsset.EndPoint = clipAsset.BeginPoint + clipAsset.Gravity / 2 * Mathf.Pow((float)clip.end- (float)clip.start, 2);
                        }
                        clip.displayName = $"FakeFall:{clipAsset.BeginPoint}¡ú{clipAsset.EndPoint}";
                        break;
                    case GravitySimlationType.Static:

                        if (clipAsset.InheritOnStart)
                            clip.displayName = "FreezeAt:??";
                        else
                            clip.displayName = $"StaticAt:{clipAsset.Height}";
                        break;
                }
            }
        }
    }

}

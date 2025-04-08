using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

using MusicTogether.TimelineKit;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MusicTogether.DancingLine
{
    [CustomTimelineEditor(typeof(DLInputClipAsset)) ]
    public class DLInputClipAssetEditor : ClipEditor
    {
        private Texture2D _pointTexture;

        // Start is called before the first frame update
        public override void OnClipChanged(TimelineClip clip)
        {
            var clipAsset = clip.asset as DLInputClipAsset;
            var track = clip.GetParentTrack();
            /*if (track)
            {
                //DLController dlController = clipAsset.dlController;
                var clips = track.GetClips().ToList();
                int index = clips.IndexOf(clip);
                if(index == 0)
                {
                    SetAutoInputs(0,clip);
                }
                else
                {
                    var previousClip = clips[index - 1];
                    var previousAsset = previousClip.asset as DLInputClipAsset;
                    if (previousAsset)
                        SetAutoInputs(previousAsset.inputDatas.Count,clip);
                    else
                        SetAutoInputs(0, clip);
                }
            }*/
            if (clipAsset)
            {
                if (clipAsset.inputType == InputTypes.ForceInput)
                    clip.displayName = "ForceInput";
                else
                    clip.displayName = $"InputArea";
            }
        }

        /*private void SetAutoInputs(int begin, TimelineClip clip)
        {
            DLInputClipAsset clipAsset = (DLInputClipAsset)clip.asset;
            if (!clipAsset.dlController)
                return;
            clipAsset.inputDatas.Clear();
            List<InputData> inputTimes = clipAsset.dlController.Inputs;
            List<InputData> clipTimes = clipAsset.inputDatas;

            int inputIndex = 0; inputIndex = begin;
            float start = (float)clip.start, end = (float)clip.end;
            Vector3 firstDirection = clipAsset.FirstDirection, secondDirection = clipAsset.SecondDirection;

            //clipTimes.Clear();

            //Debug.Log($"TryingToGetInput-count:{inputTimes.Count}");
            if (clipAsset.FreeRotation)
                while (inputTimes[inputIndex].ClickTime < end && inputIndex < inputTimes.Count)
                {
                    //Debug.Log($"Get[{inputIndex}]");
                    if (inputTimes[inputIndex].ClickTime < start)
                    {
                        inputIndex++;
                        continue;
                    }
                    clipTimes.Add(inputTimes[inputIndex]);
                    inputIndex++;
                }
            else
                while (inputIndex < inputTimes.Count && inputTimes[inputIndex].ClickTime < end)
                {
                    //Debug.Log($"Get[{inputIndex}]");
                    if (inputTimes[inputIndex].ClickTime < start)
                    {
                        inputIndex++;
                        continue;
                    }
                    clipTimes.Add(inputTimes[inputIndex]);
                    inputTimes[inputIndex].NowRotation = (inputIndex % 2) == 0 ? firstDirection : secondDirection;
                    clipTimes[^1].NowRotation = inputTimes[inputIndex].NowRotation;
                    inputIndex++;
                }
        }*/
        /*public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            base.DrawBackground(clip, region);
            //if (!TimelineWindow.instance.state.showAudioWaveform)
            //return;

            var position = region.position;
            position.width = 12;
            position.height = 12;
            position.y += position.height / 2;

            if (_pointTexture == null)
                _pointTexture = Resources.Load<Texture2D>("tex_unityuiplayables_icon_diamond");

            DLInputClipAsset clipAsset = (DLInputClipAsset)clip.asset;
            List<InputData> clipTimes = clipAsset.autoInputs;
            float start = (float)clip.start,unitLength = region.position.width/(float)clip.duration, end = (float)clip.end;
            Color FrameColor;
            if (clipAsset.inputType == InputTypes.ForceInput)
                FrameColor = new Color(0.5f, 0.43f, 0.4f);
            else
                FrameColor = Color.gray;
            int i = 0;
            while (i < clipTimes.Count && clipTimes[i].ClickTime < end)
            {
                if (clipTimes[i].ClickTime < start)
                {
                    i++;
                }
                position.x = (clipTimes[i].ClickTime - start) * unitLength;
                GUI.DrawTexture(position, _pointTexture, ScaleMode.ScaleToFit, true, 1, FrameColor, Vector4.zero,
                                            Vector4.zero);
                i++;
            }
        }*/
    }
}

using MusicTogether.TimelineKit;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace MusicTogether.DancingLine
{
    [CustomTimelineEditor(typeof(DLMoveClipAsset))]
    public class DLMoveClipAssetEditor : ClipEditor
    {
        // Start is called before the first frame update
        private Vector3 getStartPosition(float speed, TurnData prevData, TurnData nowData)
        {
            Vector3 Forward = Quaternion.Euler(prevData.NowRotation) * Vector3.forward;
            //Debug.Log($"当前TimeLine时间{GlobalTime}-上次点击时间{prevData.ClickTime}*{Forward * speed}=线位置{ _player.transform.localPosition}");
            return (prevData.StartPoint + speed * (nowData.ClickTime - prevData.ClickTime) * Forward);
        }

        public override void OnClipChanged(TimelineClip clip)
        {
            var clipAsset = clip.asset as DLMoveClipAsset;
            var track = clip.GetParentTrack();
            if (track && clipAsset)
            {
                //DLController dlController = clipAsset.dlController;
                var clips = track.GetClips().ToList();
                int index = clips.IndexOf(clip);
                if (clipAsset.GetBeginPointFromLastClip)
                {
                    if (index == 0)
                    {
                        clipAsset.GetBeginPointFromLastClip = false;
                        return;
                    }
                    var previousClip = clips[index - 1];
                    var previousAsset = previousClip.asset as DLMoveClipAsset;
                    TurnData prevData = previousAsset.turnDatas[^1];
                    List<TurnData> presetTurns = clipAsset.PresetTurns;
    
                    Vector3 Forward = Quaternion.Euler(prevData.NowRotation) * Vector3.forward;
                    Vector3 StartPoint = prevData.StartPoint + previousAsset.Speed * ((float)clip.start - prevData.ClickTime) * Forward + clipAsset.deltaBeginDistance;
                    TurnData nowData = new TurnData((float)clip.start, prevData.NowRotation, StartPoint);
                    if (clipAsset.changeStartDirection)
                        nowData.NowRotation = clipAsset.beginDirection;
                    if (presetTurns.Count < 1)
                        presetTurns.Add(nowData);
                    else
                        presetTurns[0] = nowData;
                }
                else
                {
                    if(clipAsset.PresetTurns.Count < 1)
                    {
                        clipAsset.PresetTurns.Add(new TurnData((float)clip.start, Vector3.zero, clipAsset.beginPoint));
                    }
                    else
                    {
                        clipAsset.PresetTurns[0].ClickTime = (float)clip.start;
                        clipAsset.PresetTurns[0].StartPoint = clipAsset.beginPoint;
                    }
                }
                if (index == 0)
                {
                    RefreshTurnDatas(0, clip);
                }
                else
                {
                    var previousClip = clips[index - 1];
                    var previousAsset = previousClip.asset as DLMoveClipAsset;
                    if (previousAsset)
                    {
                        int beginIndex = 0;
                        for(int i =0;i < index;i++)
                        {
                            beginIndex += (clips[i].asset as DLMoveClipAsset).turnDatas.Count;
                        }
                        RefreshTurnDatas((beginIndex - 1)/2, clip);
                    }
                    else
                        RefreshTurnDatas(0, clip);
                }

                clip.displayName = $"Move-Begin{clipAsset.turnDatas[0].StartPoint}, Speed:{clipAsset.Speed}";
            }
        }
        public void RefreshAllDatas()
        {

        }

        public void RefreshTurnDatas(int begin, TimelineClip clip)
        {
            DLMoveClipAsset clipAsset = (DLMoveClipAsset)clip.asset;
            if (!clipAsset.dlController)
                return;
            int inputIndex = begin;
            float start = (float)clip.start, end = (float)clip.end;
            List<InputData> inputTimes = clipAsset.dlController.Inputs;
            List<TurnData> clipTimeDatas = clipAsset.turnDatas;
            clipTimeDatas.Clear();

            foreach(TurnData i in clipAsset.PresetTurns)
            {
                clipTimeDatas.Add(i);
            }
            while (inputIndex < inputTimes.Count && inputTimes[inputIndex].ClickTime < end)
            {
                if (inputTimes[inputIndex].ClickTime < start)
                {
                    inputIndex++;
                    continue;
                }
                InputData inputData = inputTimes[inputIndex];
                TurnData nowData = new TurnData(inputData.ClickTime, inputData.NowRotation, Vector3.zero);
                clipTimeDatas.Add(nowData);
                //Debug.Log($"TurnData,{inputData.ClickTime}→{nowData.ClickTime},{inputData.NowRotation}→{nowData.NowRotation}");
                inputIndex++;
            }
            if (!clipAsset.GetBeginPointFromLastClip && clipTimeDatas.Count > 0)
                clipTimeDatas[0].StartPoint = clipAsset.beginPoint;
            for (int index = 1; index < clipTimeDatas.Count; index++)
            {
                clipTimeDatas[index].StartPoint = getStartPosition(clipAsset.Speed, clipTimeDatas[index-1], clipTimeDatas[index]);
            }
        }
    }
}

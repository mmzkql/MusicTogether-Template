using UnityEngine;

namespace MusicTogether.General
{
    public enum InheritOption{noInherit,Last,Specified}
    public class InheritSetting
    {
        public InheritOption inheritOption;
        public int specifiedInheritIndex;
        public int GetTargetIndex(int selfIndex)
        {
            targetIndex = selfIndex;
            switch(inheritOption)
            {
                case InheritOption.Last:
                    targetIndex = selfIndex-1;
                    break;
                case InheritOption.Specified:
                    targetIndex = specifiedInheritIndex;
                    break;
                case InheritOption.noInherit:
                    targetIndex = -2;
                    break;
            }
            return targetIndex;
        }
    }
    [Serializable]
    public struct TimeRange
    {
        [HorizontalGroup("time")]public float startTime;
        [HorizontalGroup("time")]public float endTime;

        public TimeRange(float begin, float end)
        {
            startTime = begin;
            endTime = end;
        }
        public TimeRange(float timeStamp,TimeRange deltaTime)
        {
            startTime = timeStamp+deltaTime.startTime;
            endTime = timeStamp+deltaTime.endTime;
        }
        public float GetProgress(float currentTime)
        {
            return (currentTime - startTime) / (endTime - startTime);
        }
    }
    public static class Vector3Extensions
    {
        public static Vector3 Round(this Vector3 vector)
        {
            return new Vector3(
                Mathf.Round(vector.x),
                Mathf.Round(vector.y),
                Mathf.Round(vector.z)
            );
        }
    }
}
namespace MusicTogether.DancingBall
{

}

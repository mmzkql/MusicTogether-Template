using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MusicTogether.General
{
   
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

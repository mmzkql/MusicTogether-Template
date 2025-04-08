using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace MusicTogether.DancingBall
{
    public class MapManager : MonoBehaviour
    {
        public PlayableDirector director;
        public RoadManager[] roadManagers;
    }
}

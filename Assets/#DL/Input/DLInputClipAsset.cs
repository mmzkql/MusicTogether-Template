using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

using MusicTogether.TimelineKit;

namespace MusicTogether.DancingLine
{
    public class DLInputClipAsset : BaseClipAsset<DLInputBehaviour>
    {
        public InputTypes inputType;
        [ReadOnly] public DLController dlController;
        public bool FreeRotation = false;
        [ShowIf("@!FreeRotation")]public Vector3 FirstDirection = new Vector3(0,0,0), SecondDirection = new Vector3(0, 90, 0);
        [HideInInspector] public int prevIndex = 0;
        [ShowIf("@inputType = InputTypes.AutoInput")]public InputData[] autoInputs;
    }
}

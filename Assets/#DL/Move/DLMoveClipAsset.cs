 using UnityEngine;
using Sirenix.OdinInspector;
using MusicTogether.TimelineKit;
using System.Collections.Generic;

namespace MusicTogether.DancingLine
{
    [System.Serializable]
    public class DLMoveClipAsset : BaseClipAsset<DLMoveBehaviour>
    {
        [ReadOnly] public DLController dlController;
        //[ReadOnly] public int ClipIndex = 0;
        [ShowIf("@!GetBeginPointFromLastClip")]public Vector3 beginPoint;
        [ShowIf("@GetBeginPointFromLastClip")] public Vector3 deltaBeginDistance;
        [ShowIf("@changeStartDirection")]public Vector3 beginDirection;
        public bool GetBeginPointFromLastClip = false;
        public bool changeStartDirection = false;
        public float Speed;  // ��ǰ�ٶ�
        
        public bool CalculateBeginPoint = false;
        [ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElement")]
        public List<TurnData> PresetTurns;
        [ReadOnly]public List<TurnData> turnDatas; // ת����������
        // �Զ�������߼�
        private void BeginDrawListElement(int index)
        {
            // ����ǵ�һ�����Ϊֻ��
            if (index == 0 && GetBeginPointFromLastClip)
            {
                GUI.enabled = false; // ���� GUI �༭
            }
            else
            {
                GUI.enabled = true; // ���� GUI �༭
            }
        }
    }
    [System.Serializable]
    public class TurnData
    {
        [HorizontalGroup("Data", Width = 70), LabelWidth(30), LabelText("Time")]
        public float ClickTime; // ���ʱ�䣨ת��ʱ�䣩
        [HorizontalGroup("Data", Width = 120), LabelWidth(48), LabelText("Rotation")]
        public Vector3 NowRotation; // ��ǰ����
        [HorizontalGroup("Data"), LabelWidth(38), LabelText("Begin")]
        public Vector3 StartPoint; // ��ʼλ��

        public TurnData(float ClickTime, Vector3 NowRotation, Vector3 StartPoint)
        {
            this.ClickTime = ClickTime;
            this.NowRotation = NowRotation;
            this.StartPoint = StartPoint;
        }
    }
}
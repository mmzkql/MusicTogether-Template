using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MusicTogether.General
{
    public enum NoteType {Quarter,Eighth, Semi,ThirtySecond}//�ķ��������˷�������ʮ������������ʮ��������
    public class NoteConverter {
        /**
         * 将音符序号从一种类型转换为另一种类型
         * @param originalIndex 原始序号
         * @param fromType 原始音符类型
         * @param toType 目标音符类型
         * @return 转换后的音符序号
         */
        public static int convertNoteIndex(int originalIndex, NoteType fromType, NoteType toType) {
            // 如果类型相同，直接返回原序号
            if (fromType == toType) {
                return originalIndex;
            }
        
            // 计算从原始类型到目标类型的比例因子
            double ratio = getRatio(fromType) / getRatio(toType);
        
            // 计算转换后的序号
            double convertedIndex = originalIndex * ratio;
        
            // 返回四舍五入后的整数值
            return (int) Math.Round(convertedIndex);
        }
    
        /**
         * 获取每种音符类型的时值比例
         * 以ThirtySecond为基准单位1
         */
        private static double getRatio(NoteType type) {
            switch (type) {
                case NoteType.Quarter: return 8.0;    // 三十二分音符的8倍
                case NoteType.Eighth: return 4.0;     // 三十二分音符的4倍
                case NoteType.Semi: return 2.0;      // 三十二分音符的2倍
                case NoteType.ThirtySecond: return 1.0;
                default: return -1;//throw new IllegalArgumentException("Unknown note type");
            }
        }
    }
    public enum TimeSignature {Two_four,Three_four,Four_four,Six_eight }//����
    [CreateAssetMenu(menuName = "MusicTogether/NoteDatas", fileName = "NewNoteData")]
    public class InputNoteDatas : ScriptableObject
    {
        public int BPM;
        public NoteType noteType;
        
        public List<int> NoteIndex;//16������
        public List<double> InputTimes;

        public static double GetTime(int BPM,NoteType noteType,float Note)
        {
            double noteLengthInBeats = 1;
            switch(noteType)
            {
                case NoteType.Quarter:
                    noteLengthInBeats = 1;
                    break;
                case NoteType.Eighth:
                    noteLengthInBeats = 0.5;
                    break;
                case NoteType.Semi:
                    noteLengthInBeats = 0.25;
                    break;
                case NoteType.ThirtySecond:
                    noteLengthInBeats = 0.125;
                    break;
            }

            noteLengthInBeats *= Note * 60;
            noteLengthInBeats /= BPM;
            //Debug.Log($"BPM: {BPM} NoteType: {noteType} Result:{Note}*60/{BPM}*{noteLengthInBeats}={noteLengthInBeats}");
            return noteLengthInBeats; // Note*60 / BPM* noteLengthInBeats;
        }
        [Button]
        public void GenerateTimeByNotes()
        {
            //int NotePerMinuite = BPM * NotePerBit;
            InputTimes.Clear();
            foreach(int i in NoteIndex)
            {
                InputTimes.Add(GetTime(BPM,noteType,i));
            }
        }
        
        //public List<NoteData> Notes;
        /*public class NoteData
        {
            private int BPM;
            public int Note;
            public int Frame;
            public float Time;//Second
            public NoteData(int BPM,int FPS,int note)
            {
                Note = note;
                Time = note * 60 / (BPM * 4);
                Frame = note * 60 * FPS / (BPM * 4);
            }

            void OnValidate()
            {
                Time = Note * 60 / (BPM * 4);
                Frame = Note * 60 * 60 / (BPM * 4);//Default FPS:60 #######!!!
            }
        }*/
    }

}


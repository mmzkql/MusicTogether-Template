using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicTogether.DancingLine
{
    /*[CreateAssetMenu(menuName = "Dancing Line Fanmade/DLInputData", fileName = "NewInputData")]
    public class NoteData : ScriptableObject
    {
        public int BPM;
        public NoteType noteType;
        //public int NotePerBit;

        public List<int> NoteIndex;//16������
        public List<double> InputTimes;

        public double GetTime(int Note)
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
            return Note*60 / BPM* noteLengthInBeats;
        }
        [Button]
        public void GenerateTimeByNotes()
        {
            //int NotePerMinuite = BPM * NotePerBit;
            InputTimes.Clear();
            foreach(int i in NoteIndex)
            {
                InputTimes.Add(GetTime(i));
            }
        }
    }*/
}
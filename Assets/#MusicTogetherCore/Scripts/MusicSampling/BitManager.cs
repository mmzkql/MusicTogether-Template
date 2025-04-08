using System.Collections.Generic;
using UnityEngine;

namespace MusicTogether.General
{
    public class BitManager : MonoBehaviour
    {
        //Data
        public InputNoteDatas inputData;
        // UI & Prefab References
        public RectTransform Menu;
        public GameObject BitPrefab,NotePrefab,SamplePrefab;
        public AudioSource TapSound;

        // Audio Visualization
        [HideInInspector]public float[] audioSamples;
        public float Zoom = 1.0f;
        public float NoteWidth;
        public int SampleCountPerNote = 10;
        public float SampleZoom = 1;

        // Runtime Data
        [HideInInspector]public NoteUnit currentNote;
        public BasicPlayerInteraction player;
        public List<NoteUnit> selectedNotes = new List<NoteUnit>();
        public List<BitUnit> activeBitUnits = new List<BitUnit>();

        // Calculated Properties
        public int BPM => player.BPM;
        public int NotePerBit => player.BitLength;
        public double BitProgress => player.BitDouble;
        public int VisibleBitCount
            => Mathf.CeilToInt(Menu.rect.width / (NotePerBit * NoteWidth));


        //private double LastPlayProgress;
        /*private double PlayProgress
        {
            get
            {
                return player.PlayProgress;
            }
        }*/
        /*public int NoteIndex
        {
            get
            {
                return player.BitLength;
            }
        }*/
        /*public int Bit
        {
            get
            {
                return player.Bit;
            }
        }*/
        public int SamplePerNote
        {
            get
            {
                return 661500 / BPM;
            }
        }

        // Start is called before the first frame update
        private void Awake()
        {
            //rectTransform = GetComponent<RectTransform>();

        }
        void Start()
        {
            Reset();
        }
        private void Reset()
        {
            LoadAudioClip(player.audioSource.clip);
            foreach(BitUnit i in activeBitUnits)
            {
                i.Reset(this, i.BitIndex);
            }
        }
        // Update is called once per frame
        void Update()
        {
            /*
            if (activeBitUnits.Count < 1)
            {
                BitUnit bit = Instantiate(BitPrefab, transform).GetComponent<BitUnit>();
                bit.Reset(this, Bit);
                activeBitUnits.Add(bit);
            }
            if(PlayProgress>LastPlayProgress)
            {
                if (activeBitUnits[^1].Bit < Bit + VisibleBitCount/2 + 2)
                {
                    BitUnit bit = Instantiate(BitPrefab,transform).GetComponent<BitUnit>();
                    bit.Reset(this, activeBitUnits[^1].Bit + 1);
                    activeBitUnits.Add(bit);
                }
            }
            else
            {
                if (activeBitUnits[0].Bit > Bit - VisibleBitCount/2 - 2)
                {
                    BitUnit bit = Instantiate(BitPrefab, transform).GetComponent<BitUnit>();
                    bit.Reset(this, activeBitUnits[0].Bit - 1);
                    activeBitUnits.Insert(0,bit);
                }
            }*/
            //LastPlayProgress = PlayProgress;
            MaintainBitUnitPool();
  
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleNoteSelection();
            }
        }
        private void MaintainBitUnitPool()
        {
            if (activeBitUnits.Count == 0) AddBitUnit(player.Bit);

            var lastBit = activeBitUnits[^1].BitIndex;
            if (BitProgress > lastBit - VisibleBitCount / 2)
            {
                AddBitUnit(lastBit + 1);
            }

            var firstBit = activeBitUnits[0].BitIndex;
            if (BitProgress < firstBit + VisibleBitCount / 2)
            {
                InsertBitUnit(firstBit - 1);
            }
        }
        private void AddBitUnit(int bitIndex)
        {
            var unit = Instantiate(BitPrefab, Menu.transform).GetComponent<BitUnit>();
            unit.Reset(this, bitIndex);
            activeBitUnits.Add(unit);
        }
        private void InsertBitUnit(int bitIndex)
        {
            var unit = Instantiate(BitPrefab, Menu.transform).GetComponent<BitUnit>();
            unit.Reset(this, bitIndex);
            activeBitUnits.Insert(0, unit);
        }
        public void ToggleNoteSelection()
        {
            if(currentNote && !selectedNotes.Exists(x => x.Index == currentNote.Index))
            {
                currentNote.ToggleSelection();
            }
        }

        public void LoadAudioClip(AudioClip clip)
        {
            // ������ȫ���
            if (!clip)
            {
                Debug.LogError("AudioClip cannot be null!");
                return;
            }

            // ��֤��Ƶ��������
            if (clip.loadType != AudioClipLoadType.DecompressOnLoad)
            {
                Debug.LogError($"AudioClip '{clip.name}' must have Load Type set to DecompressedOnLoad!");
                Debug.Log("����Unity�༭����ѡ����Ƶ�ļ�����Inspector�����н�Load Type��ΪDecompressedOnLoad");
                return;
            }

            try
            {
                // �������������������� �� ��������
                int totalSamples = clip.samples * clip.channels;

                // ���������洢����
                audioSamples = new float[totalSamples];

                // ��ȡ��Ƶ���ݣ�����ʼ������ʼ��ȡ��
                bool success = clip.GetData(audioSamples, 0);

                if (!success)
                {
                    Debug.LogError("��Ƶ���ݶ�ȡʧ�ܣ�������Ƶ����");
                    audioSamples = null;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"��Ƶ�����쳣: {e.Message}");
                audioSamples = null;
            }
        }

        public void SaveData()
        {
            float bpmXbitLength = BPM * NotePerBit;
            inputData.NoteIndex.Clear();
            for(int i = 0;i<selectedNotes.Count;i++)
            {
                inputData.NoteIndex.Add(selectedNotes[i].Index);
            }
        }
    }
}

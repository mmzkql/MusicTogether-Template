using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MusicTogether.General
{

    public class NoteUnit : MonoBehaviour
    {
        [Header("Colors")] public Color NormalColor, PlayingColor, SelectedColor, SelectedPlayingColor;

        private Color CurrentColor
        {
            get
            {
                if (IsActiveNote) return IsSelected ? SelectedPlayingColor : PlayingColor;
                return IsSelected ? SelectedColor : NormalColor;
            }
        }

        // Components
        private Image _background;
        private RectTransform _waveformBar;

        // Runtime Data
        private BitManager _bitManager;
        private BasicPlayerInteraction _player;

        public int Index { get; private set; }
        public bool IsSelected => _bitManager.selectedNotes.Find(x => x.Index == Index);
        private bool IsActiveNote => _player.NoteIndex == Index;


        private GameObject SamplePrefab;
        private int SampleCountPerNote;

        private int SampleBegin, SampleEnd;

        private float SampleSum;

        // Start is called before the first frame update
        public void Refresh(BitUnit parentBit, int subIndex)
        {
            _bitManager = parentBit._bitManager;
            _player = _bitManager.player;
            Index = parentBit.BitIndex * _bitManager.NotePerBit + subIndex;

            _background = GetComponent<Image>();
            _waveformBar = transform.GetChild(0).GetComponent<RectTransform>();


            SamplePrefab = _bitManager.SamplePrefab;
            SampleCountPerNote = _bitManager.SampleCountPerNote;

            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
            UpdateVisualState();

            if (Index >= 0)
            {
                SampleBegin = Mathf.Clamp(661500 * (Index) / _bitManager.BPM, 0, _bitManager.audioSamples.Length - 1);
                SampleEnd = Mathf.Clamp(661500 * (Index + 1) / _bitManager.BPM, 0, _bitManager.audioSamples.Length - 1);

                float sum = 0;
                int count = SampleEnd - SampleBegin; // ������ʼ�ͽ���λ�õĲ�������
                /*for (int i = SampleBegin; i < SampleEnd; i++)
                {
                    sum += Mathf.Abs(_bitManager.audioSamples[i]); // ȡ����ֵ�Ա�����������
                }
                SampleSum = sum / count;*/
                for (int j = 0; j < SampleCountPerNote; j++)
                {
                    sum = 0;
                    RectTransform SampleChild = Instantiate(SamplePrefab, transform).transform.GetChild(0)
                        .GetComponent<RectTransform>();
                    for (int i = count * (j) / SampleCountPerNote; i < count * (j + 1) / SampleCountPerNote; i++)
                    {
                        sum += Mathf.Abs(_bitManager.audioSamples[SampleBegin + i]); // ȡ����ֵ�Ա�����������
                    }

                    SampleSum = sum * _bitManager.SampleZoom * SampleCountPerNote / count; //
                    SampleChild.anchorMin = new Vector2(0, SampleSum);
                    SampleChild.sizeDelta = new Vector2(0, 0);
                    //SampleChild.sizeDelta = new Vector2(0, GetComponent<RectTransform>().rect.height * SampleSum);
                    SampleChild.gameObject.SetActive(true);
                }
            }
            else
                SampleSum = 0;
            //_waveformBar.sizeDelta = new Vector2(0, GetComponent<RectTransform>().rect.height * SampleSum);
        }

        // Update is called once per frame
        public void ToggleSelection()
        {
            if (!IsSelected) _bitManager.selectedNotes.Add(this);
            else _bitManager.selectedNotes.Remove(this);
        }

        private void Update()
        {
            if (_player.NoteIndex == Index && _bitManager.currentNote != this)
            {
                _bitManager.currentNote = this;
                if (IsSelected)
                {
                    //_bitManager.TapSound.Stop();
                    //_bitManager.TapSound.Play();

                }

            }

            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            _background.color = CurrentColor;
        }
    }
}
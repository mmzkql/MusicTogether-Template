using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MusicTogether.General
{
    public class BitUnit : MonoBehaviour
    {
        // UI Components
        public Transform noteContainer;
        public TextMeshProUGUI indexLabel;
        private RectTransform _rectTransform;
        [HideInInspector] public BitManager _bitManager;
        private BasicPlayerInteraction _player;

        //Data
        public int BitIndex { get; private set; }
        private int _bitLength; //以16分音符为基本单位
        private float _noteWidth;

        // Start is called before the first frame update
        public void Reset(BitManager _manager, int bitIndex)
        {
            _bitManager = _manager;
            _player = _manager.player;

            BitIndex = bitIndex;
            //BitBegin = Bit * 60 / player.BPM;
            _bitLength = _manager.NotePerBit;
            _noteWidth = _manager.NoteWidth;
            _rectTransform = GetComponent<RectTransform>();

            for (int i = 0; i < _bitLength; i++)
            {
                NoteUnit note = Instantiate(_manager.NotePrefab, noteContainer).GetComponent<NoteUnit>();
                note.Refresh(this, i);
                //Notes.Add(note);///////会报错
            }

            indexLabel.text = BitIndex.ToString();
            _rectTransform.pivot = new Vector2(1 / (float)_bitLength / 2, 0.5f);
            _rectTransform.anchoredPosition =
                new Vector2((float)((BitIndex - _player.BitDouble) * _bitLength * _noteWidth), 0f);
            gameObject.SetActive(true);
        }

        void Start()
        {
            /*for (int i = 0;i<BitLength;i++)
            {
                var note = Instantiate(NotePrefab);
                note.transform.SetParent(transform);
                Notes.Add(note.GetComponent<NoteUnit>());
            }*/
        }

        // Update is called once per frame
        void Update()
        {
            _rectTransform.anchoredPosition =
                new Vector2((float)((BitIndex - _player.BitDouble) * _bitLength * _noteWidth), 0f);
            _rectTransform.sizeDelta = new Vector2(_bitLength * _noteWidth, 0f);
            transform.localScale = Vector3.one;
            /*if (Mathf.Abs(BitIndex - _player.Bit) > _bitManager.VisibleBitCount)
            {
                _bitManager.activeBitUnits.Remove(this);
                Destroy(gameObject);
            }*/
        }
    }
}
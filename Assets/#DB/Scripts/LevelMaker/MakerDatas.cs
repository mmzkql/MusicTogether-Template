using System;
using System.Collections.Generic;
using MusicTogether.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MusicTogether.DancingBall
{
    public enum MusicDataInheritOption{Map,Last,Custom}
    public enum StyleDataInheritOption{Map,Manager,Last,Custom}
    public enum RoadPlacementStyle { Classic, Chebyshev, Free, DontChange }
    public enum FacingDirection{Up,Back,Right,Left,Front,Down}
    public enum AnimationType { Transform, Color ,Animation}
    public enum TransformAnimationType { Position, Rotation, Scale }
    [Serializable]
    public class MusicData
    {
        public InputNoteDatas clickNoteData;
        public int bpm;
        public NoteType noteProcessType;
        public List<int> clickNotes;


        [Button]
        public void UpdateNoteData()
        {
            clickNotes.Clear();
            NoteType fromType = clickNoteData.noteType, toType = noteProcessType;
            clickNoteData.NoteIndex.ForEach(item=>clickNotes.Add(NoteConverter.convertNoteIndex(item,fromType,toType)));
            clickNotes.Sort((a, b) => a.CompareTo(b));
        }
    }

    [Serializable]
    public class BlockPlacementData
    {
        public RoadPlacementStyle placementStyle;
        public float scale;
        
        [BoxGroup("Anchor", ShowLabel = true,CenterLabel = true)]
        [HorizontalGroup("Anchor/Content")] 
        [ShowInInspector, HideLabel, ReadOnly, PreviewField(ObjectFieldAlignment.Left, Height = 50)]
        private Texture2D _anchorTexture;
        
        [VerticalGroup("Anchor/Content/Coordinates")][PropertySpace]
        [PropertyRange(-0.5f, 0.5f), OnValueChanged("UpdateCoordinateSystemTexture")]
        
        public float anchorx;
        [VerticalGroup("Anchor/Content/Coordinates")]
        [PropertyRange(-0.5f, 0.5f), OnValueChanged("UpdateCoordinateSystemTexture")]
        public float anchory;
        
        
        public FacingDirection tileDirection = FacingDirection.Down;
        public bool doubleDirection;
        public FacingDirection tileDirection2 = FacingDirection.Down;
        
        private void UpdateCoordinateSystemTexture()
        {
            int size = 100;
            _anchorTexture = new Texture2D(size, size);

            // 透明背景
            Color transparent = new Color(0.219f, 0.219f, 0.219f, 1.0f);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    _anchorTexture.SetPixel(i, j, transparent);
                }
            }

            // 绘制坐标轴
            Color axisColor = Color.white;
            // X轴（从左到右）
            for (int i = 0; i < size; i++)
            {
                _anchorTexture.SetPixel(i, size / 2, axisColor);
            }

            // Y轴（从下到上）
            for (int j = 0; j < size; j++)
            {
                _anchorTexture.SetPixel(size / 2, j, axisColor);
            }

            // 绘制锚点
            int anchorX = (int)((anchorx + 0.5f) * size);
            int anchorY = (int)((anchory + 0.5f) * size);
            Color anchorColor = new Color(0.8f, 0.6f, 0.0f, 1.0f);

            // 绘制一个3x3的锚点
            int r = 5;
            for (int i = -r; i <= r; i++)
            {
                for (int j = -r; j <= r; j++)
                {
                    int px = Mathf.Clamp(anchorX + i, 0, size - 1);
                    int py = Mathf.Clamp(anchorY + j, 0, size - 1);
                    _anchorTexture.SetPixel(px, py, anchorColor);
                }
            }

            _anchorTexture.Apply();
        }
    }
}
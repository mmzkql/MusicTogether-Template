using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MusicTogether.DancingBall
{
    public class BlockStyle : MonoBehaviour
    {
        //BasicStyle
        [DisableIf("@!(isTurnNode||isCorner)")] 
        public RoadType roadType;
        [Title("Data")]
        [PropertySpace]
        [DisableIf("@inherit == true")]
        public FacingDirection tileDirection = FacingDirection.Down;
        [DisableIf("@inherit == true")]
        public bool doubleDirection;
        [DisableIf("@inherit == true||!doubleDirection")]
        public FacingDirection tileDirection2 = FacingDirection.Down;
        
        [ShowInInspector, HideLabel, ReadOnly, PreviewField(ObjectFieldAlignment.Left, Height = 50)]
        [BoxGroup("Anchor", ShowLabel = true,CenterLabel = true)][HorizontalGroup("Anchor/Content")] 
        private Texture2D _anchorTexture;
        
        [PropertySpace] [PropertyRange(-0.5f, 0.5f), OnValueChanged("UpdateCoordinateSystemTexture")]
        [VerticalGroup("Anchor/Content/Coordinates")]
        [DisableIf("@inherit == true")]
        public float x;
        
        [PropertyRange(-0.5f, 0.5f), OnValueChanged("UpdateCoordinateSystemTexture")]
        [VerticalGroup("Anchor/Content/Coordinates")]
        [DisableIf("@inherit == true")]
        public float y;

        //AdvancedStyle
        public bool inheritStyleData = true;
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        [DisableIf("@inherit == true")] 
        public StyleData styleData;


        [Title("Dependencies")] public Transform tileHolder;
        public Animator animator;
        
        [FormerlySerializedAs("top")] [FoldoutGroup("RoadFaces")] [VerticalGroup("RoadFaces/Hex")]
        public MeshFilter topMesh;

        [FormerlySerializedAs("back")] [HorizontalGroup("RoadFaces/Hex/Middle1")][PropertySpace(SpaceBefore = 10)]
        //[LabelWidth(20)] 
        public MeshFilter backMesh;
    
        [HorizontalGroup("RoadFaces/Hex/Middle1")][PropertySpace(SpaceBefore = 10)]
        //[LabelWidth(20)] // 缩小中间标签宽度
        public MeshFilter leftMesh;
    
        [HorizontalGroup("RoadFaces/Hex/Middle2")][PropertySpace(SpaceBefore = 10)]
        public MeshFilter rightMesh;

        [HorizontalGroup("RoadFaces/Hex/Middle2")][PropertySpace(SpaceBefore = 10)]
        public MeshFilter frontMesh;
        
        [VerticalGroup("RoadFaces/Hex")] [PropertySpace(SpaceBefore = 10)]
        public MeshFilter bottomMesh;
        
        //BakingDataes
        public Dictionary<FacingDirection, MeshFilter> _directionTransforms;
        private FacingDirection _prevDirection;
        
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
            int anchorX = (int)((x + 0.5f) * size);
            int anchorY = (int)((y + 0.5f) * size);
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

        public void UpdateData(RoadManager roadManager,int nodeIndex,bool inheritBasicStyle)
        {
            List<int> styleIndex = roadManager.StyleIndexes;
            List<BlockNode> nodes = roadManager.Nodes;
            
            int lastCornerIndex = nodes.FindLastIndex((i) =>i.nodeIndex< nodeIndex && (i.isCorner||i.isTurnNode));
            lastCornerIndex = Mathf.Clamp(lastCornerIndex, 0, styleIndex.Count - 1);
            
            int lastStyleDataIndex = nodes.FindLastIndex((i) =>i.nodeIndex<nodeIndex && !i.style.inheritStyleData);
            lastStyleDataIndex = Mathf.Clamp(lastStyleDataIndex, 0, styleIndex.Count - 1);

            if (inheritBasicStyle)
            {
                roadType = nodes[lastCornerIndex].style.roadType;
                tileDirection = nodes[lastCornerIndex].style.tileDirection;
                doubleDirection = nodes[lastCornerIndex].style.doubleDirection;
                tileDirection2 = nodes[lastCornerIndex].style.tileDirection2;
                x = nodes[lastCornerIndex].style.x;
                y = nodes[lastCornerIndex].style.y;
                
            }
            else
            {
                if (nodeIndex != 0)
                {
                    var prevstyle = roadManager.Nodes[styleIndex[lastStyleDataIndex]].style;
                    if(prevstyle.doubleDirection&& tileDirection!=prevstyle.tileDirection)
                    {
                        _prevDirection = prevstyle.tileDirection2;
                    }
                    else
                    {
                        _prevDirection = prevstyle.tileDirection;
                    }
                }
            }

            if (inheritStyleData)
            {
                styleData = nodes[lastStyleDataIndex].style.styleData;
            }
            
            tileHolder.localEulerAngles = new Vector3(-transform.localEulerAngles.x,0,0);
            if (roadType == RoadType.Free)
            {
                tileHolder.GetChild(0).localEulerAngles = transform.localEulerAngles;
            }
            else
                tileHolder.GetChild(0).localEulerAngles = Vector3.zero;
            ApplyAnchor();
            ApplyTile(nodeIndex,inheritBasicStyle);
        }
        void ApplyTile(int index,bool inheritBasicStyle)
        {
            //应用朝向
            _directionTransforms = new Dictionary<FacingDirection, MeshFilter>
            {
                { FacingDirection.Up, topMesh },
                { FacingDirection.Down, bottomMesh },
                { FacingDirection.Left, leftMesh },
                { FacingDirection.Right, rightMesh },
                { FacingDirection.Front, frontMesh },
                { FacingDirection.Back, backMesh }
            };
            
            foreach (var meshFilter in _directionTransforms.Values)
            {
                meshFilter.gameObject.SetActive(false);
            }

            // 启用当前方向的面
            if (_directionTransforms.TryGetValue(tileDirection, out var target))
            {
                target.gameObject.SetActive(true);
                target.mesh = styleData.tileMesh;
            }
            if (doubleDirection && _directionTransforms.TryGetValue(tileDirection2, out var target2))
            {
                target2.gameObject.SetActive(true);
                target2.mesh = styleData.tileMesh;
            }
            // 如果不是继承模式且之前有方向，启用之前方向的面
            if (!inheritBasicStyle&& index !=0 && _prevDirection != tileDirection)
            {
                if (_directionTransforms.TryGetValue(_prevDirection, out var prevTarget))
                {
                    prevTarget.gameObject.SetActive(true);
                    prevTarget.mesh = styleData.tileMesh;
                }
            }
        }
        void ApplyAnchor()
        {
                // 第一层子物体：localPosition变为(0, y, x)
                tileHolder.GetChild(0).localPosition = new Vector3(0f, y, x);
            
                // 第二层子物体（子物体的子物体）：localPosition变为(0, -y, -x)
                foreach (Transform grandChild in tileHolder.GetChild(0))
                {
                    grandChild.localPosition = new Vector3(0f, -y, -x);
                }
        }

        public void GetNodes(List<Transform> nodes)
        {
            nodes.Clear();
            _directionTransforms = new Dictionary<FacingDirection, MeshFilter>
            {
                { FacingDirection.Up, topMesh },
                { FacingDirection.Down, bottomMesh },
                { FacingDirection.Left, leftMesh },
                { FacingDirection.Right, rightMesh },
                { FacingDirection.Front, frontMesh },
                { FacingDirection.Back, backMesh }
            };
            Transform first = null, second = null;
            if (_directionTransforms.TryGetValue(tileDirection, out var target))
            {
                first = target.transform;
                nodes.Add(first);
            }
            if (!doubleDirection)
            {
                return;
            }

            if (_directionTransforms.TryGetValue(tileDirection, out var target2))
            {
                second = target2.transform;
            }
            nodes.Add(second);
        }
        private void OnEnable()
        {
            UpdateCoordinateSystemTexture();
        }
    }
}
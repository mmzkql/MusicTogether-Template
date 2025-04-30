using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MusicTogether.DancingBall
{
    public class BlockHolder : MonoBehaviour
    {
        public MapHolder MapHolder => MapHolder.Instance;
        public RoadHolder roadHolder;
        //Information
        [TabGroup("BlockDatas")] 
        public int nodeIndex;
        public bool custommTime;
        public float nodeTime;
        //Config
        [TabGroup("BlockDatas")] 
        public bool isTurn, isCorner ,teleport,jump;
        
        //Style
        public BlockStyleData styleData;

        //Prefab
        [TabGroup("PrefabBindings")]
        public Transform anchorBlock;
        [TabGroup("PrefabBindings")]
        public Renderer debugBlock;
        [TabGroup("PrefabBindings")] 
        public TapPlacer tapPlacer;
        [TabGroup("PrefabBindings")] 
        public Animator animator;
        
        [TabGroup("PrefabBindings")]
        //[VerticalGroup("PrefabBindings/Hex")]
        [PropertySpace(SpaceBefore = 10)]
        public Transform topTile;

        [TabGroup("PrefabBindings")]
        //[HorizontalGroup("PrefabBindings/Hex/Middle1")][PropertySpace(SpaceBefore = 10)]
        public Transform backTile;

        [TabGroup("PrefabBindings")]
        //[HorizontalGroup("PrefabBindings/Hex/Middle1")][PropertySpace(SpaceBefore = 10)]
        public Transform leftTile;

        [TabGroup("PrefabBindings")]
        //[HorizontalGroup("PrefabBindings/Hex/Middle2")][PropertySpace(SpaceBefore = 10)]
        public Transform rightTile;

        [TabGroup("PrefabBindings")]
        //[HorizontalGroup("PrefabBindings/Hex/Middle2")][PropertySpace(SpaceBefore = 10)]
        public Transform frontTile;

        [TabGroup("PrefabBindings")]
        //[VerticalGroup("PrefabBindings/Hex")] [PropertySpace(SpaceBefore = 10)]
        public Transform bottomTile;
        
        //Data
        [HideInInspector]public List<Transform> nodes = new List<Transform>();
        
        /*public void UpdateData(RoadMaker roadMaker,int nodeIndex,bool inheritBasicStyle)
        {
            List<int> styleIndex = roadMaker.StyleIndexes;
            List<BlockMaker> nodes = roadMaker.blockMakers;
            
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
                    var prevstyle = roadMaker.blockMakers[styleIndex[lastStyleDataIndex]].style;
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
            if (roadType == RoadPlacementStyle.Free)
            {
                tileHolder.GetChild(0).localEulerAngles = transform.localEulerAngles;
            }
            else
                tileHolder.GetChild(0).localEulerAngles = Vector3.zero;
            ApplyAnchor();
            ApplyTile(nodeIndex,inheritBasicStyle);
        }*/
        /*void ApplyTile(int index,bool inheritBasicStyle)
        {
            //应用朝向
            _directionTransforms = new Dictionary<FacingDirection, MeshFilter>
            {
                { FacingDirection.Up, topTile },
                { FacingDirection.Down, bottomTile },
                { FacingDirection.Left, leftTile },
                { FacingDirection.Right, rightTile },
                { FacingDirection.Front, frontTile },
                { FacingDirection.Back, backTile }
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
        }*/
        /*void ApplyAnchor()
        {
                // 第一层子物体：localPosition变为(0, y, x)
                tileHolder.GetChild(0).localPosition = new Vector3(0f, y, x);
            
                // 第二层子物体（子物体的子物体）：localPosition变为(0, -y, -x)
                foreach (Transform grandChild in tileHolder.GetChild(0))
                {
                    grandChild.localPosition = new Vector3(0f, -y, -x);
                }
        }*/

        /*public void GetNodes(List<Transform> nodes)
        {
            nodes.Clear();
            _directionTransforms = new Dictionary<FacingDirection, MeshFilter>
            {
                { FacingDirection.Up, topTile },
                { FacingDirection.Down, bottomTile },
                { FacingDirection.Left, leftTile },
                { FacingDirection.Right, rightTile },
                { FacingDirection.Front, frontTile },
                { FacingDirection.Back, backTile }
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
        }*/
        /*private void OnEnable()
        {
            UpdateCoordinateSystemTexture();
        }*/
    }
}
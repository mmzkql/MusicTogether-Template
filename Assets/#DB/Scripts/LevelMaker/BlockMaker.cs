using System.Collections.Generic;
using MusicTogether.General;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace MusicTogether.DancingBall
{
    //[ExecuteAlways]
    public class BlockMaker : MonoBehaviour
    {
        //Dependencies
        private static MapMaker TargetMapMaker=>MapMaker.Instance;
        [HideInInspector]public RoadMaker targetRoadMaker;
        [TabGroup("PrefabBindings")]
        public BlockHolder targetBlockHolder;
        [TabGroup("PrefabBindings")]
        public Transform anchorBlock;
        [TabGroup("PrefabBindings")]
        public Renderer debugBlock;
        
        //Data
        public int BlockIndex
        {
            get => targetBlockHolder.nodeIndex;
            set => targetBlockHolder.nodeIndex = value;
        }

        public bool IsTurn
        {
            get => targetBlockHolder.isTurn;
            set => targetBlockHolder.isTurn = value;
        }

        public bool IsCorner
        {
            get => targetBlockHolder.isCorner;
            set => targetBlockHolder.isCorner = value;
        }
        [TabGroup("BlockData")] 
        public BlockPlacementData placementData;
        [TabGroup("BlockData")] 
        public StyleDataInheritOption styleDataOption;
        [TabGroup("BlockData")]
        public BlockStyleData StyleData
        {
            get => targetBlockHolder.styleData;
            set => targetBlockHolder.styleData = value;
        }

        private Color DebugMaskColor
        {
            get
            {
                if(targetBlockHolder.isTurn||targetBlockHolder.isCorner)
                {
                    if (styleDataOption == StyleDataInheritOption.Custom)
                        return TargetMapMaker.turnAndStyleMaskColor;
                    return TargetMapMaker.turnMaskColor;
                }
                if (styleDataOption == StyleDataInheritOption.Custom)
                    return TargetMapMaker.styleMaskColor;
                return TargetMapMaker.normalMaskColor;
            }
        }
        
        //private Vector3 _previousPosition, _previousRotation;
        //private bool _justified = false,_justifyPosition=false,_justifyRotation=false;
        //
        //BakingData
        private Dictionary<FacingDirection, Transform> _directionTransforms;
        private FacingDirection _prevDirection;

        public void UpdateBlockData()
        {
            if (!(IsTurn || IsCorner))
            {
                if(BlockIndex > 0)
                {
                    placementData = targetRoadMaker.blockMakers[BlockIndex - 1].placementData;
                }
                else
                {
                    IsCorner = true;
                    placementData = new BlockPlacementData();
                }
            }

            switch (styleDataOption)
            {
                case StyleDataInheritOption.Custom:
                    break;
                case StyleDataInheritOption.Last:
                    if (BlockIndex > 0)
                    {
                        StyleData = targetRoadMaker.blockMakers[BlockIndex-1].StyleData;
                    }
                    else
                    {
                        styleDataOption = StyleDataInheritOption.Map;
                        goto case StyleDataInheritOption.Map;
                    }
                    break;
                case StyleDataInheritOption.Map:
                    StyleData = TargetMapMaker.mapStyleData;
                    break;
            }
        }
        public void UpdateBlockData(RoadMaker roadMaker,int selfIndex)
        {
            targetRoadMaker = roadMaker;
            BlockIndex = selfIndex;
            UpdateBlockData();
        }

        public void UpdateBlockPosition(int selfIndex)
        {
            
            //position
            if (selfIndex < 1)
            {
                targetRoadMaker.transform.Translate(transform.localPosition,Space.World);
                transform.localPosition = Vector3.zero;
                return;
            }
            
            BlockMaker previousBlockMaker = targetRoadMaker.blockMakers[selfIndex-1];
            Vector3 prevPosition = previousBlockMaker.transform.position;
            Vector3 prevDirection = previousBlockMaker.placementData.placementDirection;
            float length = (placementData.scale + previousBlockMaker.placementData.scale)/2f;
            Vector3 direction = PlacingDirection(placementData.placementStyle,prevPosition,prevDirection,length);
            
            if (placementData.placementStyle != RoadPlacementStyle.DontChange)
            {
                transform.position = prevPosition + direction;
            }
            //rotation
            //scale
            transform.localScale = Vector3.one * placementData.scale;
            
            //anchor
            float anchorX = placementData.anchorX;
            float anchorY = placementData.anchorY;
            anchorBlock.localPosition = new Vector3(0,-anchorY,-anchorX);
            anchorBlock.GetChild(0).localPosition = new Vector3(0, anchorY,anchorX);
            anchorBlock.localRotation = Quaternion.Euler(DirectionToEulerAngles(direction));
        }

        public void UpdateBlockStyle(int selfIndex)
        {
            //ResourceHandle
            Undo.DestroyObjectImmediate(anchorBlock.GetChild(0).gameObject);
            
            //Block Spawn
            GameObject block = Instantiate(StyleData.blockPrefab, anchorBlock, true);
            
            Undo.RegisterCreatedObjectUndo(block, "Create block");
            float anchorX = placementData.anchorX;
            float anchorY = placementData.anchorY;
            block.transform.localPosition = new Vector3(0, anchorY,anchorX);
            
            //TapPlacer Spawn
            /*GameObject tapPlacerObject = Instantiate(StyleData.tapPlacerPrefab, anchorBlock, true);
            
            Undo.RegisterCreatedObjectUndo(tapPlacerObject, "Create tapPlacer");
            TapPlacer tapPlacer = tapPlacerObject.GetComponent<TapPlacer>();*/
            TapPlacer tapPlacer = block.GetComponent<BlockHolder>().tapPlacer;
            tapPlacer.tapPlacerData = StyleData.tapPlacerData;
            
            //Tile
            _directionTransforms = new Dictionary<FacingDirection, Transform>
            {
                { FacingDirection.Up,targetBlockHolder.topTile },
                { FacingDirection.Down, targetBlockHolder.bottomTile },
                { FacingDirection.Left, targetBlockHolder.leftTile },
                { FacingDirection.Right, targetBlockHolder.rightTile },
                { FacingDirection.Front, targetBlockHolder.frontTile },
                { FacingDirection.Back, targetBlockHolder.backTile }
            };
            foreach (var tile in _directionTransforms.Values)
            {
                tile.gameObject.SetActive(false);
            }

            Transform activeTile = null;
            if (_directionTransforms.TryGetValue(placementData.tileDirection, out activeTile))
            {
                activeTile.gameObject.SetActive(true);
            }
            if (placementData.doubleDirection || (selfIndex > 0 && (targetBlockHolder.isTurn||targetBlockHolder.isCorner)))
            {
                if(_directionTransforms.TryGetValue(placementData.tileDirection2, out var target2))
                {
                    target2.gameObject.SetActive(true);
                }
            }
            tapPlacer.transform.SetParent(activeTile);
            tapPlacer.transform.localPosition = Vector3.zero;
            tapPlacer.transform.localRotation = Quaternion.identity;
        }
        
        /*public void ResetValue(RoadMaker maker)
        {
            nodeManager = maker;
            director = maker.mapMaker.director;
            scale = nodeManager.Scale;
            transform.localScale = new Vector3(scale, scale, scale);
            if (nodeIndex == 0)
                style.inheritStyleData = false;
            DebugStyle();
        }
        public void ResetValue(RoadMaker maker, int index)
        {
            nodeIndex = index;
            ResetValue(maker);
        }

        public void UpdateStyle()
        {
            style.UpdateData(nodeManager,nodeIndex,isTurnNode||isCorner);
            style.GetNodes(nodes);
        }
        public void UpdatePosition(int prev, float Length)
        {
            Vector3 prevPosition = nodeManager.blockMakers[prev].transform.position;
            Vector3 prevRotation = nodeManager.blockMakers[prev].transform.localEulerAngles;

            if (nodeIndex == 0)
            {
                nodeManager.transform.localPosition += transform.localPosition;
                transform.localPosition = Vector3.zero;
                return;
            }
            Vector3 target = Vector3.zero;
            switch (style.roadType)
            {
                case RoadPlacementStyle.Free:
                    target = NormalPosition(prevPosition, prevRotation, Length);
                    break;
                case RoadPlacementStyle.Classic:
                    target = chebyshevPosition(prevPosition, prevRotation,
                        Length); //strictPosition(prevPosition, eulerAngles, Length);
                    break;
                case RoadPlacementStyle.Chebyshev:
                    target = chebyshevPosition(prevPosition, prevRotation, Length);
                    break;
                case RoadPlacementStyle.DontChange:
                    target = transform.position;
                    break;
            }
            transform.position = target;
        }*/
        
        public void DebugStyle()
        {
            if (debugBlock != null)
            {
                MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
                debugBlock.GetPropertyBlock(propBlock);
            
                // 设置颜色属性
                propBlock.SetColor("_Color", DebugMaskColor);
                // 应用PropertyBlock
                debugBlock.SetPropertyBlock(propBlock);
            }
        }

        /*void MonitorJustification()
        {
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabAsset(this)||(PrefabStageUtility.GetCurrentPrefabStage() != null))
                return;
            if (Application.isPlaying) return;
#endif

            //Monitor
            if (transform.position != _previousPosition)
            {
                _justifyPosition = true;
                _previousPosition = transform.position;
                return;
            }
            if (transform.localEulerAngles != _previousRotation)
            {
                _justifyRotation = true;
                _previousRotation = transform.localEulerAngles;
                return;
            }

            if (_justifyPosition)
            {
                if (nodeIndex > 0)
                {
                    nodeManager.blockMakers[nodeIndex - 1].transform.LookAt(transform);
                }
                _justifyPosition=false;
                nodeManager.UpdateBlockPosAndStyle(nodeIndex-1);
            }

            if (_justifyRotation)
            {
                if(style.roadType == RoadPlacementStyle.Classic)
                    transform.localEulerAngles = (transform.localEulerAngles / 45).Round() * 45;
                if (!isTurnNode) isCorner = true;
                _justifyRotation = false;
                nodeManager.UpdateBlockPosAndStyle((int)Mathf.Ceil(nodeIndex));
            }
        }*/

        public static Vector3 PlacingDirection(RoadPlacementStyle placementType,Vector3 prevPosition, Vector3 direction, float length)
        {
            Vector3 finalDirection = direction.normalized*length;
            switch (placementType)
            {
                case RoadPlacementStyle.Classic:
                    finalDirection = (finalDirection / length).Round()*length;
                    break;
                case RoadPlacementStyle.Chebyshev:
                    float maxComponent = Mathf.Max(
                        Mathf.Abs(finalDirection.x),
                        Mathf.Abs(finalDirection.y),
                        Mathf.Abs(finalDirection.z)
                    );
                    finalDirection*=length/maxComponent;
                    break;
                case RoadPlacementStyle.Free:
                    finalDirection *= length;
                    break;
            }
            return finalDirection;
        }
        public static Vector3 DirectionToEulerAngles(Vector3 direction)
        {
            // 归一化方向向量（确保是单位向量）
            direction = direction.normalized;

            // 计算俯仰角（Pitch，绕X轴）
            float pitch = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;

            // 计算偏航角（Yaw，绕Y轴）
            float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // 返回欧拉角（Roll默认为0）
            return new Vector3(pitch, yaw, 0f);
        }
        /*public Vector3 NormalPosition(Vector3 startPoint, Vector3 eulerAngles, float length)
        {
            float pitch = eulerAngles.x * Mathf.Deg2Rad;
            float yaw = eulerAngles.y * Mathf.Deg2Rad;
            float roll = eulerAngles.z * Mathf.Deg2Rad; 

             Vector3 direction = new Vector3(
                Mathf.Sin(yaw) * Mathf.Cos(pitch),
                -Mathf.Sin(pitch),
                Mathf.Cos(yaw) * Mathf.Cos(pitch)
            );
            direction.Normalize();

            // ???????????
            Vector3 endPoint = startPoint + direction * length;
            return endPoint;
        }

        public Vector3 chebyshevPosition(Vector3 startPoint, Vector3 eulerAngles, float chebyshevLength) //?????б???????
        {
            // 1. ????λ??????????????????????????
            float pitch = eulerAngles.x * Mathf.Deg2Rad;
            float yaw = eulerAngles.y * Mathf.Deg2Rad;
            //?????????z,x,y??
            Vector3 direction = new Vector3(
                Mathf.Sin(yaw) * Mathf.Cos(pitch),
                -Mathf.Sin(pitch),
                Mathf.Cos(yaw) * Mathf.Cos(pitch)
            );
            direction.Normalize(); // ?????λ????

            // 2. ???????????????????????????? chebyshevLength
            float maxComponent = Mathf.Max(
                Mathf.Abs(direction.x),
                Mathf.Abs(direction.y),
                Mathf.Abs(direction.z)
            );

            // 3. ???????????????б??????? = chebyshevLength
            Vector3 scaledDirection = direction * (chebyshevLength / maxComponent);

            // 4. ???????
            Vector3 endPoint = startPoint + scaledDirection;
            return endPoint;
        }*/
    }
}

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace MusicTogether.DancingBall
{
    //[ExecuteAlways]
    public class BlockMaker : MonoBehaviour
    {
        //Dependencies
        private static MapMaker TargetMapMaker=>MapMaker.Instance;
        [HideInInspector]public RoadMaker targetRoadMaker;
        public Block targetBlock;
        
        public Renderer debugBlock;
        //Information
        public int nodeIndex;
        //Config
        public bool isTurnNode, isCorner ,teleport,jump;
        //Data
        public BlockPlacementData customPlacementData;
        public StyleDataInheritOption styleDataOption;
        public BlockStyleData customStyleData;
        
        private Color DebugMaskColor
        {
            get
            {
                if(isTurnNode||isCorner)
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

        private Vector3 previousPosition, previousRotation;
        private bool Justified = false,justifyPosition=false,justifyRotation=false;
        //
        public List<Transform> nodes = new List<Transform>();
        
        [Button]
        public void ApplyStyle()
        {
            nodeManager.UpdateBlockStyle(nodeIndex);
        }
        [Button]
        public void UpdateBlock()
        {
            nodeManager.UpdateBlockPosAndStyle(nodeIndex);
        }
        public void ResetValue(RoadMaker maker)
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
            Vector3 prevPosition = nodeManager.Nodes[prev].transform.position;
            Vector3 prevRotation = nodeManager.Nodes[prev].transform.localEulerAngles;

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
                    target = normalPosition(prevPosition, prevRotation, Length);
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
        }
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
            if (style.roadType == RoadPlacementStyle.Free)
            {
                debugBlock.transform.localEulerAngles = Vector3.zero;
            }
            else
                debugBlock.transform.eulerAngles = Vector3.zero;
        }
        private void Awake()
        {
            style.GetNodes(nodes);
        }

        void Start()
        {
            if (isTurnNode)
            {
                MeshFilter target = null;
                switch (style.tileDirection)
                {
                    case FacingDirection.Up:
                        target = style.topMesh;
                        break;
                    case FacingDirection.Down:
                        target = style.bottomMesh;
                        break;
                    case FacingDirection.Left:
                        target = style.leftMesh;
                        break;
                    case FacingDirection.Right:
                        target = style.rightMesh;
                        break;
                    case FacingDirection.Front:
                        target = style.frontMesh;
                        break;
                    case FacingDirection.Back:
                        target = style.backMesh;
                        break;
                }

                GameObject tap = Instantiate(style.styleData.tapPrefab);
                //Debug.Log(tap);
                //Debug.Log(target);
                //tap.transform.SetParent(target.transform);
                tap.transform.position = target.transform.position;
                tap.transform.rotation = target.transform.rotation;
                tapPlacer = tap.GetComponent<TapPlacer>();
                tapPlacer.StartTap(this,nodeManager.BPM,nodeManager.noteProcessType,nodeIndex);
            }
        }
        void Update()
        {
            if (nodeManager.enableJustify)
                MonitorJustification();
        }

        void MonitorJustification()
        {
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabAsset(this)||(PrefabStageUtility.GetCurrentPrefabStage() != null))
                return;
            if (Application.isPlaying) return;
#endif

            //Monitor
            if (transform.position != previousPosition)
            {
                justifyPosition = true;
                previousPosition = transform.position;
                return;
            }
            if (transform.localEulerAngles != previousRotation)
            {
                justifyRotation = true;
                previousRotation = transform.localEulerAngles;
                return;
            }

            if (justifyPosition)
            {
                if (nodeIndex > 0)
                {
                    nodeManager.Nodes[nodeIndex - 1].transform.LookAt(transform);
                }
                justifyPosition=false;
                nodeManager.UpdateBlockPosAndStyle(nodeIndex-1);
            }

            if (justifyRotation)
            {
                if(style.roadType == RoadPlacementStyle.Classic)
                    transform.localEulerAngles = (transform.localEulerAngles / 45).Round() * 45;
                if (!isTurnNode) isCorner = true;
                justifyRotation = false;
                nodeManager.UpdateBlockPosAndStyle((int)Mathf.Ceil(nodeIndex));
            }
        }
        
        public Vector3 normalPosition(Vector3 startPoint, Vector3 eulerAngles, float length)
        {
            // ??????????????????Unity??Mathf??????????????
            float pitch = eulerAngles.x * Mathf.Deg2Rad; // ?????? (X??)
            float yaw = eulerAngles.y * Mathf.Deg2Rad; // ????? (Y??)
            float roll = eulerAngles.z * Mathf.Deg2Rad; // ????? (Z??) - ??????????

            // ???????????????????????Z-Y-X????????
            //Vector3 direction = new Vector3(Mathf.Cos(yaw) * Mathf.Cos(pitch),Mathf.Sin(yaw) * Mathf.Cos(pitch),Mathf.Sin(pitch));
            Vector3 direction = new Vector3(
                Mathf.Sin(yaw) * Mathf.Cos(pitch),
                -Mathf.Sin(pitch),
                Mathf.Cos(yaw) * Mathf.Cos(pitch)
            );
            // ??λ????????????????????????????????????1??
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
        }

        
    }
}

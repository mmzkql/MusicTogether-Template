using System;
using JetBrains.Annotations;
using MusicTogether.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MusicTogether.DancingBall
{
    [Serializable]
    public class StyleAnimation
    {
        [Title("@AnimationTitle()")]
        public AnimationType animationType; // Transform 或 Color
        [ShowIf("@animationType==AnimationType.Transform")]public TransformAnimationType transformType;
        
        [PropertySpace(10)]
        [HorizontalGroup("CustomTimeRange"),DisableIf("@CustomTimeRange==false")][CanBeNull] public TimeRange timeRange;
        [PropertySpace(10),HideLabel,ToggleLeft]
        [HorizontalGroup("CustomTimeRange")] public bool customTimeRange;
        
#if UNITY_EDITOR
        [ShowIf("IsEditingPrefab")] [InfoBox("只能在预制体编辑模式下设置目标Transform", InfoMessageType.Warning, "@!IsEditingPrefab()")]
        [OnValueChanged("UpdatePathButton")]
#endif
        public Transform animTransform;
        
        [DisableIf("IsEditingPrefab")]public string animTransformPath;

        [ShowIf("@animationType==AnimationType.Transform")]public AnimationCurve animCurve;
        [ShowIf("@animationType==AnimationType.Color")]public Gradient colorGradient;
        [ShowIf("@animationType==AnimationType.Animation")]public Animation animation;
        
#if UNITY_EDITOR
        private string AnimationTitle()
        {
            string text = "";
            switch (animationType)
            {
                case AnimationType.Transform:
                    switch (transformType)
                    {
                        case TransformAnimationType.Position:
                            text = "Position";
                            break;
                        case TransformAnimationType.Rotation:
                            text = "Rotation";
                            break;
                        case TransformAnimationType.Scale:
                            text =  "Scale";
                            break;
                    }
                    break;
                case AnimationType.Color:
                    text =  "Color";
                    break;
                case AnimationType.Animation:
                    text =  "Animation";
                    break;
            }
            text += "动画";
            return text;
        }
        // 判断是否在编辑预制体
        private bool IsEditingPrefab()
        {
            if (UnityEditor.Selection.activeGameObject == null)
                return false;
            
            var blockStyle = UnityEditor.Selection.activeGameObject.GetComponent<BlockHolder>();
            if (blockStyle == null)
                return false;
            
            return UnityEditor.PrefabUtility.IsPartOfPrefabAsset(blockStyle.gameObject)||(UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null);
        }

        // 更新路径方法
        private void UpdateTransformPath()
        {
            if (!IsEditingPrefab())
            {
                Debug.LogWarning("只能在预制体编辑模式下更新路径");
                return;
            }

            if (animTransform == null)
            {
                animTransformPath = "";
                return;
            }

            var blockHolder = UnityEditor.Selection.activeGameObject.GetComponent<BlockHolder>();
            if (blockHolder == null) return;

            animTransformPath = GetRelativePath(blockHolder.transform, animTransform);
            UnityEditor.EditorUtility.SetDirty(blockHolder.styleData); // 标记ScriptableObject为脏
        }

        private string GetRelativePath(Transform root, Transform target)
        {
            if (target == root) return "";
            if (!target.IsChildOf(root)) 
            {
                Debug.LogError("目标Transform不是BlockStyle的子对象");
                return "";
            }

            var path = new System.Text.StringBuilder();
            var current = target;
            while (current != null && current != root)
            {
                if (path.Length > 0)
                    path.Insert(0, '/');
                path.Insert(0, current.name);
                current = current.parent;
            }
            return path.ToString();
        }

        //[Button("更新路径")]
        [ShowIf("IsEditingPrefab")]
        private void UpdatePathButton()
        {
            UpdateTransformPath();
        }
#endif
    }
    
    [CreateAssetMenu(menuName = "MTDancingBal/BlockStyle", fileName = "BlockStyle_styleName")]
    public class BlockStyleData : ScriptableObject
    {
        [FoldoutGroup("BasicData")]
        [PreviewField]
        public GameObject blockPrefab;
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public TapPlacerData tapPlacerData;

        
        //[FoldoutGroup("EnhancementData")] public bool antiClipping;
        
        [HideLabel,PropertySpace]
        [FoldoutGroup("StyleData")] public TimeRange timeRange;
        [PropertySpace]
        [FoldoutGroup("StyleData")] public StyleAnimation[] styleAnimations;
        //[FoldoutGroup("StyleData")] public GameObject[] RoadTilePrefabs;
    }
}

using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MusicTogether.DancingBall
{

    [CustomEditor(typeof(BlockMaker))]
    public class BlockMakerEditor : OdinEditor
    {
        private void OnSceneGUI()
        {
            BlockMaker blockMaker = (BlockMaker)target;
        }

        private void DrawLabel(BlockMaker maker)
        {
            GUIStyle fontStyle = new GUIStyle();
            fontStyle.normal.background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            fontStyle.normal.textColor = Color.black;
            fontStyle.fontStyle = FontStyle.Normal;
            fontStyle.fontSize = 18; 
            
            string Information = $"Node{maker.targetBlockHolder.nodeIndex}:";
            if (maker.targetBlockHolder.isTurn)
            {
                Information += "TapBlock";
            }
            else if (maker.targetBlockHolder.isCorner)
            {
                Information += "CornerBlock";
            }
            else
            {
                Information += "NormalBlock";
            }

            switch (maker.styleDataOption)
            {
                case StyleDataInheritOption.Custom:
                    Information += "CustomStyle";
                    break;
                case StyleDataInheritOption.Last:
                    Information += "LastStyle";
                    break;
                case StyleDataInheritOption.Map:
                    Information += "MapStyle";
                    break;
            }
            Handles.Label(maker.transform.position + Vector3.up * 4, Information, fontStyle);
        }
        private void JustifyPlacementData(BlockMaker maker)
        {
            if (PrefabUtility.IsPartOfPrefabAsset(this)||PrefabStageUtility.GetCurrentPrefabStage() != null)
                return;
            if (Application.isPlaying) return;
            if (maker.BlockIndex >= maker.targetRoadMaker.blockMakers.Count)
            {
                return;
            }

            if (!(maker.IsTurn || maker.IsCorner))
            {
                maker.IsCorner = true;
                maker.UpdateBlockData();
            }
            BlockMaker nextMaker = maker.targetRoadMaker.blockMakers[maker.BlockIndex+1];
            Vector3 direction = (nextMaker.transform.position - maker.transform.position).normalized;
            maker.placementData.placementDirection = direction;
        }
    }
}
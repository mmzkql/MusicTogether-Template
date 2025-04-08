using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace MusicTogether.DancingBall
{

    [CustomEditor(typeof(BlockNode))]
    public class NodeEditor : OdinEditor
    {
        private void OnSceneGUI()
        {
            GUIStyle fontStyle = new GUIStyle();
            fontStyle.normal.background = new Texture2D(1, 1, TextureFormat.RGBA32, false); //���ñ������
            fontStyle.normal.textColor = Color.black; //����������ɫ
            fontStyle.fontStyle = FontStyle.Normal; // ����Ӵ���б
            fontStyle.fontSize = 18; //�����С


            BlockNode node = (BlockNode)target;
            string Information = $"Node{node.nodeIndex}:";
            if (node.isTurnNode)
            {
                Information += "TurnBlock";
            }
            else if (!node.style.inheritStyleData)
            {
                Information += "StyleBlock";
            }
            else
            {
                Information += "NormalBlock";
            }

            Handles.Label(node.transform.position + Vector3.up * 4, Information, fontStyle);
        }
    }
}
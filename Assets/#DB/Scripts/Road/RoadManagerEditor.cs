using Sirenix.OdinInspector.Editor;
using MusicTogether.DancingBall;
using MusicTogether.General;
using UnityEditor;
using UnityEngine;

[ExecuteAlways,CustomEditor(typeof(RoadManager))]
public class RoadManagerEditor : OdinEditor 
{
    void OnSceneGUI()
    {
        RoadManager manager = (RoadManager)target;
        
        if(manager.Nodes.Count == 0)
            return;
        for(int i =0;i < manager.Nodes.Count - 1;i++)
        {
            if(manager.Nodes[i]&&manager.Nodes[i+1])
                Debug.DrawLine(manager.Nodes[i].transform.position, manager.Nodes[i + 1].transform.position);
        }
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RoadManager manager = (RoadManager)target;
        if(GUILayout.Button("重置")) 
        {
            ResetData(manager);
        }
        //serializedObject.Update();//开头，更新显示，可以不写
 
        /*中间，自定义绘制*/
        
        serializedObject.ApplyModifiedProperties();//结尾，应用修改，必须有，不然不能修改
    }
    // Update is called once per frame
    public void ResetData(RoadManager manager)
    {
        manager.enableJustify = false;
        //输入数据
        NoteType fromType = manager.inputData.noteType, toType = manager.noteProcessType;
        manager.TurnIndexes.Clear();
        manager.inputData.NoteIndex.ForEach(item=>manager.TurnIndexes.Add(NoteConverter.convertNoteIndex(item,fromType,toType)));
        manager.TurnIndexes.Sort((a, b) => a.CompareTo(b));
        //更新列表
        manager.Nodes.Clear();
        manager.StyleIndexes.Clear();
        //--GetNode
        foreach (BlockNode i in manager.GetComponentsInChildren<BlockNode>())
        {
            manager.Nodes.Add(i);
        }
        manager.Nodes.Sort((a, b) => a.nodeIndex.CompareTo(b.nodeIndex));
        //--FillNode
        if(manager.Nodes.Count < 1)
        {
            manager.Nodes.Insert(0, CreateNode(manager,0));
            manager.Nodes[0].transform.localPosition = Vector3.zero;
        }
        
        for (int i = 0; i < manager.end - manager.begin; i++)
        {
            if (i > manager.Nodes.Count - 1 || i > manager.Nodes[i].nodeIndex)
            {
                manager.Nodes.Insert(i, CreateNode(manager,i));
            }
            manager.Nodes[i].isTurnNode = (manager.TurnIndexes.Exists((x) => x==i+manager.begin));
            /*if (manager.Nodes[i].isStyleNode||manager.Nodes[i].isTurnNode &&!manager.StyleIndexes.Exists((x) => x==(i)))
            {
                manager.StyleIndexes.Add(i);
            }*/
        }
        manager.StyleIndexes.Sort((a, b) => a.CompareTo(b));
        
        manager.UpdateValue();
        manager.UpdateBlockPosAndStyle(0);
    }
    BlockNode CreateNode(RoadManager manager,int index)
    {
        GameObject l = PrefabUtility.InstantiatePrefab(manager.RoadUnitPrefab, manager.transform) as GameObject;
        l.GetComponent<BlockNode>().nodeIndex = index;
        l.transform.SetSiblingIndex(index);
        return l.GetComponent<BlockNode>();
    }
}

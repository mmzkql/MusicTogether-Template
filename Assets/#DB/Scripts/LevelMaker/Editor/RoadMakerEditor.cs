using Sirenix.OdinInspector.Editor;
using MusicTogether.DancingBall;
using MusicTogether.General;
using UnityEditor;
using UnityEngine;

[ExecuteAlways,CustomEditor(typeof(RoadMaker))]
public class RoadMakerEditor : OdinEditor 
{
    void OnSceneGUI()
    {
        RoadMaker maker = (RoadMaker)target;
        
        if(maker.nodes.Count == 0)
            return;
        for(int i =0;i < maker.nodes.Count - 1;i++)
        {
            if(maker.nodes[i]&&maker.nodes[i+1])
                Debug.DrawLine(maker.nodes[i].transform.position, maker.nodes[i + 1].transform.position);
        }
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RoadMaker maker = (RoadMaker)target;
        if(GUILayout.Button("重置")) 
        {
            ResetData(maker);
        }
        //serializedObject.Update();//开头，更新显示，可以不写
 
        /*中间，自定义绘制*/
        
        serializedObject.ApplyModifiedProperties();//结尾，应用修改，必须有，不然不能修改
    }
    // Update is called once per frame
    public void ResetData(RoadMaker maker)
    {
        maker.enableJustify = false;
        //输入数据
        NoteType fromType = maker.inputData.noteType, toType = maker.noteProcessType;
        maker.TurnIndexes.Clear();
        maker.inputData.NoteIndex.ForEach(item=>maker.TurnIndexes.Add(NoteConverter.convertNoteIndex(item,fromType,toType)));
        maker.TurnIndexes.Sort((a, b) => a.CompareTo(b));
        //更新列表
        maker.nodes.Clear();
        maker.StyleIndexes.Clear();
        //--GetNode
        foreach (BlockMaker i in maker.GetComponentsInChildren<BlockMaker>())
        {
            maker.nodes.Add(i);
        }
        maker.nodes.Sort((a, b) => a.nodeIndex.CompareTo(b.nodeIndex));
        //--FillNode
        if(maker.nodes.Count < 1)
        {
            maker.nodes.Insert(0, CreateNode(maker,0));
            maker.nodes[0].transform.localPosition = Vector3.zero;
        }
        
        for (int i = 0; i < maker.end - maker.begin; i++)
        {
            if (i > maker.nodes.Count - 1 || i > maker.nodes[i].nodeIndex)
            {
                maker.nodes.Insert(i, CreateNode(maker,i));
            }
            maker.nodes[i].isTurnNode = (maker.TurnIndexes.Exists((x) => x==i+maker.begin));
            /*if (manager.Nodes[i].isStyleNode||manager.Nodes[i].isTurnNode &&!manager.StyleIndexes.Exists((x) => x==(i)))
            {
                manager.StyleIndexes.Add(i);
            }*/
        }
        maker.StyleIndexes.Sort((a, b) => a.CompareTo(b));
        
        maker.UpdateValue();
        maker.UpdateBlockPosAndStyle(0);
    }
    BlockMaker CreateNode(RoadMaker maker,int index)
    {
        GameObject l = PrefabUtility.InstantiatePrefab(maker.RoadUnitPrefab, maker.transform) as GameObject;
        l.GetComponent<BlockMaker>().nodeIndex = index;
        l.transform.SetSiblingIndex(index);
        return l.GetComponent<BlockMaker>();
    }
}

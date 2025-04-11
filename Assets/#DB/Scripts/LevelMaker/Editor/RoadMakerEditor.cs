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
        
        if(maker.blockMakers.Count == 0)
            return;
        for(int i =0;i < maker.blockMakers.Count - 1;i++)
        {
            if(maker.blockMakers[i]&&maker.blockMakers[i+1])
                Debug.DrawLine(maker.blockMakers[i].transform.position, maker.blockMakers[i + 1].transform.position);
        }
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RoadMaker maker = (RoadMaker)target;
        if(GUILayout.Button("重置")) 
        {
            //ResetData(maker);
        }
        //serializedObject.Update();//开头，更新显示，可以不写
 
        /*中间，自定义绘制*/
        
        serializedObject.ApplyModifiedProperties();//结尾，应用修改，必须有，不然不能修改
    }
    // Update is called once per frame
    /*public void ResetData(RoadMaker maker)
    {
        maker.enableJustify = false;
        //输入数据
        NoteType fromType = maker.inputData.noteType, toType = maker.noteProcessType;
        maker.TurnIndexes.Clear();
        maker.inputData.NoteIndex.ForEach(item=>maker.TurnIndexes.Add(NoteConverter.convertNoteIndex(item,fromType,toType)));
        maker.TurnIndexes.Sort((a, b) => a.CompareTo(b));
        //更新列表
        maker.blockMakers.Clear();
        maker.StyleIndexes.Clear();
        //--GetNode
        foreach (BlockMaker i in maker.GetComponentsInChildren<BlockMaker>())
        {
            maker.blockMakers.Add(i);
        }
        maker.blockMakers.Sort((a, b) => a.nodeIndex.CompareTo(b.nodeIndex));
        //--FillNode
        if(maker.blockMakers.Count < 1)
        {
            maker.blockMakers.Insert(0, CreateNode(maker,0));
            maker.blockMakers[0].transform.localPosition = Vector3.zero;
        }
        
        for (int i = 0; i < maker.end - maker.begin; i++)
        {
            if (i > maker.blockMakers.Count - 1 || i > maker.blockMakers[i].nodeIndex)
            {
                maker.blockMakers.Insert(i, CreateNode(maker,i));
            }
            maker.blockMakers[i].isTurnNode = (maker.TurnIndexes.Exists((x) => x==i+maker.begin));
        }
        
        maker.UpdateValue();
        maker.UpdateBlockPosAndStyle(0);
    }
    BlockMaker CreateNode(RoadMaker maker,int index)
    {
        GameObject l = PrefabUtility.InstantiatePrefab(maker.RoadUnitPrefab, maker.transform) as GameObject;
        l.GetComponent<BlockMaker>().nodeIndex = index;
        l.transform.SetSiblingIndex(index);
        return l.GetComponent<BlockMaker>();
    }*/
}

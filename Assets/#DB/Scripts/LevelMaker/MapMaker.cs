using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MusicTogether.DancingBall
{
    public class MapMaker : MonoBehaviour
    {
        public static MapMaker Instance;
        public MapHolder MapHolder=>MapHolder.Instance;
        [Title("DefaultDatas")]
        public bool drawTrack,drawBlockMask;
        
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public MusicData mapMusicData;
        
        public GameObject mapBlockHolderPrefab;
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public BlockStyleData mapBlockStyleData;
        
        [Title("Debug")]
        public Color normalMaskColor,turnMaskColor,styleMaskColor,turnAndStyleMaskColor;
        [Title("Manager")]
        public List<RoadMaker> roadMakers;
        
        public RoadMaker CreateNewRoad(int targetIndex)
        {
            GameObject newRoad = new GameObject($"RoadHolder ({targetIndex})");
            newRoad.transform.SetParent(transform);
            newRoad.transform.localPosition = Vector3.zero;
            newRoad.transform.localRotation = Quaternion.identity;
            newRoad.transform.localScale = Vector3.one;
            
            RoadMaker maker = newRoad.AddComponent<RoadMaker>();
            RoadHolder holder = newRoad.AddComponent<RoadHolder>();
            maker.UpdateData(targetIndex,holder);
            
            return maker;
        }
        public void UpdateRoadList()
        {
            roadMakers.Clear();
            MapHolder.roadHolders.Clear();

            roadMakers = GetComponentsInChildren<RoadMaker>(true).ToList();
            roadMakers.Sort((a,b)=>a.RoadIndex.CompareTo(b.RoadIndex));
            
            int minIndex = roadMakers[0].RoadIndex;
            int maxIndex = roadMakers[^1].RoadIndex;
            var existingIndices = roadMakers.Select(r => r.RoadIndex).OrderBy(i => i).ToList();
            var missingIndices = Enumerable.Range(minIndex, maxIndex - minIndex + 1)
                .Except(existingIndices)
                .ToList();
        
            // 为每个缺失的索引创建新的RoadMaker对象
            foreach (var missingIndex in missingIndices)
            {
                roadMakers.Insert(missingIndex, CreateNewRoad(missingIndex));
            }
            
            foreach (RoadMaker roadMaker in roadMakers)
            {
                roadMaker.transform.SetSiblingIndex(roadMaker.RoadIndex);
                roadMaker.gameObject.name = $"RoadHolder ({roadMaker.RoadIndex})";
                
                MapHolder.roadHolders.Add(roadMaker.roadHolder);
            }
        }
        public void UpdateRoadData(int beginIndex)
        {
            for (int i = beginIndex; i < roadMakers.Count; i++)
            {
                roadMakers[i].UpdateData(i);
            }
        }
        
        [ButtonGroup("MakerTools")]
        public void UpdateRoadListTool()
        {
            UpdateRoadList();
        }
        [ButtonGroup("MakerTools")]
        public void CreateNewRoadTool()
        {
            CreateNewRoad(roadMakers.Count);
            UpdateRoadList();
        }
        [ButtonGroup("MakerTools")]
        public void UpdateRoadData()
        {
            UpdateRoadData(0);
        }
    }
}
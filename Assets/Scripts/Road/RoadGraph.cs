using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoadGraph : MonoBehaviour
{
    public struct RoadGraphNode
    {
        GridCell right, left, up, down;
        GridCell middle; // Mine

    }


    public void SetRoadGraphNode(GridCell _gridCell)
    {
        RoadGraphNode roadGraphNode = new RoadGraphNode();

    }

    internal void ConstructRoad(GameObject[] RoadPrefabs ,GridCell gridFrameForRoad, RoadPreviewData _previewRoadData)
    {
        SetRoadGraphNode(gridFrameForRoad);

        // 연결 돼 있던 Preview Object를 실질적인 Road Object로 교체
        gridFrameForRoad.objectInThisGridSpace = Instantiate(RoadPrefabs[(int)_previewRoadData.roadType],
        new Vector3(_previewRoadData.roadPosition.x, 0.5f, _previewRoadData.roadPosition.y), _previewRoadData.roadRotation);
    }
}

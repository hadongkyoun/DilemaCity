using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum RoadType
{
    RoadEnd,
    RoadStraight,
    RoadCorner,
    RoadThreeWay,
    RoadFourWay,
}
public class RoadGraph : MonoBehaviour
{


    private Dictionary<GridCell, RoadGraphVertex> roadList = new Dictionary<GridCell, RoadGraphVertex>();

    // �巡���Ͽ� ���� �� ���ε鸸 ����ǰԲ� Ʈ��ŷ���ִ� ����Ʈ
    private List<RoadGraphVertex> startToLastCellList = new List<RoadGraphVertex>();


    private GameGrid gameGrid;
    private void Start()
    {
        gameGrid = FindObjectOfType<GameGrid>();
    }
    private class RoadGraphVertex
    {
        public GridCell node; // Mine
        public Vector2Int nodePosition;

        // 4������ Edge�� ���� ��ųʸ�
        public Edge edge = new Edge();
    }
    private class Edge
    {
        public RoadGraphVertex left = null;
        public RoadGraphVertex right = null;
        public RoadGraphVertex up = null;
        public RoadGraphVertex down = null;
        //Direction
    }


    public void SetRoadGraphNode(GridCell _gridCell, Vector2Int middleCellPosition, RoadDirection roadDrageDirection)
    {
        // �׷��� ���ؽ� ���� 
        RoadGraphVertex roadGraphVertex = new RoadGraphVertex();
        roadGraphVertex.node = _gridCell;
        roadGraphVertex.nodePosition = middleCellPosition;

        startToLastCellList.Add(roadGraphVertex);
        BasicConnectVertex(_gridCell, roadGraphVertex, middleCellPosition, roadDrageDirection);

        if (roadList.ContainsKey(_gridCell) == false)
        {
            roadList.Add(roadGraphVertex.node, roadGraphVertex);
        }
        else
        {
            ConnectEdge(_gridCell, roadGraphVertex, middleCellPosition, roadDrageDirection);
            Debug.Log("Connected!!!");
        }
    }

    private void BasicConnectVertex(GridCell _gridCell, RoadGraphVertex roadGraphVertex, Vector2Int middleCellPosition, RoadDirection roadDrageDirection)
    {
        if (roadDrageDirection == RoadDirection.Direction_XPlus)
        {
            
            GridCell cellFrame = gameGrid.GetGridCellFromPosition(middleCellPosition.x - 1, middleCellPosition.y);
            if (cellFrame != null)
            {
                foreach(RoadGraphVertex vertexBefore in startToLastCellList)
                {
                    if(vertexBefore.node == cellFrame)
                    {
                        Debug.Log(cellFrame.name);
                        vertexBefore.edge.right = roadGraphVertex;
                        roadGraphVertex.edge.left = vertexBefore;

                        break;
                    }
                }
            }
        }
        else if (roadDrageDirection == RoadDirection.Direction_XMinus)
        {
            GridCell cellFrame = gameGrid.GetGridCellFromPosition(middleCellPosition.x + 1, middleCellPosition.y);
            if (cellFrame != null)
            {
                foreach (RoadGraphVertex vertexBefore in startToLastCellList)
                {
                    if (vertexBefore.node == cellFrame)
                    {
                        vertexBefore.edge.left = roadGraphVertex;
                        roadGraphVertex.edge.right = vertexBefore;
                        break;
                    }
                }
            }
        }
        else if (roadDrageDirection == RoadDirection.Direction_ZPlus)
        {
            GridCell cellFrame = gameGrid.GetGridCellFromPosition(middleCellPosition.x, middleCellPosition.y - 1);
            if (cellFrame != null)
            {
                foreach (RoadGraphVertex vertexBefore in startToLastCellList)
                {
                    if (vertexBefore.node == cellFrame)
                    {
                        vertexBefore.edge.up = roadGraphVertex;
                        roadGraphVertex.edge.down = vertexBefore;
                        break;
                    }
                }
            }
        }
        else if (roadDrageDirection == RoadDirection.Direction_ZMinus)
        {
            GridCell cellFrame = gameGrid.GetGridCellFromPosition(middleCellPosition.x, middleCellPosition.y + 1);
            if (cellFrame != null)
            {
                foreach (RoadGraphVertex vertexBefore in startToLastCellList)
                {
                    if (vertexBefore.node == cellFrame)
                    {
                        vertexBefore.edge.down = roadGraphVertex;
                        roadGraphVertex.edge.up = vertexBefore;
                        Debug.Log("Before : " + vertexBefore.edge.down.node.name + ", Current : " + roadGraphVertex.edge.up.node.name);
                        break;
                    }
                }
            }
        }
    }

    private void ConnectEdge(GridCell _gridCell, RoadGraphVertex roadGraphVertex, Vector2Int middleCellPosition, RoadDirection roadDrageDirection)
    {

        

        // �浹 �� ��� ��

        if (roadDrageDirection == RoadDirection.Direction_ZMinus || roadDrageDirection == RoadDirection.Direction_ZPlus)
        {
            if (roadList.TryGetValue(_gridCell, out RoadGraphVertex _roadGraphVertex))
            {
                // ������ �浹 �ߴ� ������ �̾� �ޱ�
                roadGraphVertex.edge.left = _roadGraphVertex.edge.left;
                roadGraphVertex.edge.right = _roadGraphVertex.edge.right;
            }
            Debug.Log(CountNearByRoads(roadGraphVertex));

        }

        if (roadDrageDirection == RoadDirection.Direction_XPlus || roadDrageDirection == RoadDirection.Direction_XMinus)
        {
            
            if (roadList.TryGetValue(_gridCell, out RoadGraphVertex _roadGraphVertex))
            {
                roadGraphVertex.edge.up = _roadGraphVertex.edge.up;
                roadGraphVertex.edge.down = _roadGraphVertex.edge.down;
            }
        }

        
    }
    private int CountNearByRoads(RoadGraphVertex _vertex)
    {
        int count = 0;
        if (_vertex.edge.right != null)
        {
            count++;
        }
        if (_vertex.edge.left != null)
        {
            count++;
        }
        if (_vertex.edge.up != null)
        {
            count++;
        }
        if (_vertex.edge.down != null)
        {
            count++;
        }

        return count;
    }
    public void UpdateRoadFromNearByRoads(GameObject[] RoadPrefabs, RoadDirection currentDragDirection)
    {
        foreach (RoadGraphVertex vertex in startToLastCellList)
        {
            int nearRoadsNum = CountNearByRoads(vertex);
            RoadType roadType = RoadType.RoadEnd;
            Quaternion rotateRotation = Quaternion.identity;

            switch (nearRoadsNum)
            {
                // �̿��� ���ΰ� ���� ���
                case 0:
                    roadType = RoadType.RoadEnd;
                    break;
                // �̿��� ���ΰ� �Ѱ� �ִ� ���
                case 1:
                    Debug.Log(nearRoadsNum);
                    roadType = RoadType.RoadEnd;
                    // �׷� ��� ���� ���� ��, X�� ���� �� ���
                    if (currentDragDirection == RoadDirection.Direction_XPlus || currentDragDirection == RoadDirection.Direction_XMinus)
                    {
                        // ���ʿ� ���� �ƴٸ�
                        if (vertex.edge.left != null)
                        {
                            rotateRotation = Quaternion.identity;
                        }
                        else
                        {
                            rotateRotation = Quaternion.Euler(0, 180, 0);
                        }
                    }
                    else if (currentDragDirection == RoadDirection.Direction_ZPlus || currentDragDirection == RoadDirection.Direction_ZMinus)
                    {
                        if (vertex.edge.up != null)
                        {
                            rotateRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else
                        {
                            rotateRotation = Quaternion.Euler(0, -90, 0);
                        }
                    }
                    break;
                // �̿��� ���ΰ� �ΰ� �� ���
                case 2:
                    if (currentDragDirection == RoadDirection.Direction_XPlus || currentDragDirection == RoadDirection.Direction_XMinus)
                    {
                        // �� ���ΰ� ���� ���� ���� �� �ִ�. �׷��� ��Ʈ����Ʈ��
                        if (vertex.edge.right != null && vertex.edge.left != null)
                        {
                            // �� �Ʒ� ���ΰ� ������ ������
                            if (vertex.edge.up != null && vertex.edge.down != null)
                            {
                                roadType = RoadType.RoadFourWay;
                            }
                            // �� Ȥ�� �Ʒ� ���θ� ������ ���� ��
                            else if (vertex.edge.up == null && vertex.edge.down != null)
                            {
                                roadType = RoadType.RoadThreeWay;
                            }
                            else if (vertex.edge.up != null && vertex.edge.down == null)
                            {
                                roadType = RoadType.RoadThreeWay;
                            }
                            else
                            {
                                Debug.Log(nearRoadsNum);
                                roadType = RoadType.RoadStraight;
                            }
                        }

                        // ���� ���� ��� ���� ��,
                        else
                        {
                        }
                    }
                    else if (currentDragDirection == RoadDirection.Direction_ZPlus || currentDragDirection == RoadDirection.Direction_ZMinus)
                    {
                        // �� ���ΰ� ���� ���� ���� �� �ִ�. �׷��� ��Ʈ����Ʈ��
                        if (vertex.edge.up != null && vertex.edge.down != null)
                        {
                            roadType = RoadType.RoadStraight;
                            rotateRotation = Quaternion.Euler(0, 90, 0);
                        }

                        // ���� ���� ��� ���� ��,
                        else if (vertex.edge.down == null)
                        {

                            // ���������� ���� �� �ڳ�
                            if (vertex.edge.right != null)
                            {
                                roadType = RoadType.RoadCorner;

                            }
                            // �������� ���� �� �ڳ�
                            else
                            {
                                roadType = RoadType.RoadCorner;
                            }
                        }
                        // ������ ���� ��� ���� ��,
                        else
                        {
                            // ���������� ���� �� �ڳ�
                            if (vertex.edge.right != null)
                            {
                                roadType = RoadType.RoadCorner;
                            }
                            // �������� ���� �� �ڳ�
                            else
                            {
                                roadType = RoadType.RoadCorner;
                            }
                        }
                    }
                    break;
            }

            if (vertex.node.objectInThisGridSpace != null)
            {
                Destroy(vertex.node.objectInThisGridSpace);
            }
            // ���� �� �ִ� Preview Object�� �������� Road Object�� ��ü
            vertex.node.objectInThisGridSpace = Instantiate(RoadPrefabs[(int)roadType],
            new Vector3(vertex.nodePosition.x, 0.5f, vertex.nodePosition.y), rotateRotation);
        }

        ResetStartToLastCellList();
    }

    internal void ConstructRoad(GridCell gridFrameForRoad, RoadPreviewData _previewRoadData, RoadDirection roadDirection)
    {
        SetRoadGraphNode(gridFrameForRoad, _previewRoadData.roadPosition, roadDirection);

        gridFrameForRoad.isOccupied = true;
        gridFrameForRoad.DefineObjectDataType(ObjectDataType.RoadType);
    }

    private void ResetStartToLastCellList()
    {
        startToLastCellList.Clear();
    }
}

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


    RoadGraphVertex roadGraphVertex = new RoadGraphVertex();
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


    public void SetRoadGraphNode(GridCell _gridCell, Vector2Int middleCellPosition, RoadDirection roadDragDirection)
    {
        if(roadList.TryGetValue(_gridCell, out RoadGraphVertex value))
        {
            roadGraphVertex = value;
        }
        else
        {
            roadGraphVertex = new RoadGraphVertex();
        roadGraphVertex.node = _gridCell;
        roadGraphVertex.nodePosition = middleCellPosition;
        }
        // �׷��� ���ؽ� ���� 
        




        if (roadList.ContainsKey(_gridCell) == false)
        {
            roadList.Add(roadGraphVertex.node, roadGraphVertex);
        }
        else
        {
            ConnectEdge(_gridCell, roadGraphVertex, middleCellPosition, roadDragDirection);
            Debug.Log("Connected!!!");
        }

        startToLastCellList.Add(roadGraphVertex);
        BasicConnectVertex(_gridCell, roadGraphVertex, middleCellPosition, roadDragDirection);
        roadGraphVertex = null;
    }

    private void BasicConnectVertex(GridCell _gridCell, RoadGraphVertex roadGraphVertex, Vector2Int middleCellPosition, RoadDirection roadDragDirection)
    {
        GridCell cellFrame = null;

        if (roadDragDirection == RoadDirection.Direction_XPlus)
        {
            cellFrame = gameGrid.GetGridCellFromPosition(middleCellPosition.x - 1, middleCellPosition.y);

            foreach (RoadGraphVertex vertex in startToLastCellList)
            {
                if (vertex.node == cellFrame)
                {
                    roadGraphVertex.edge.left = vertex;
                    vertex.edge.right = roadGraphVertex;
                }
            }
        }
        else if (roadDragDirection == RoadDirection.Direction_XMinus)
        {
            cellFrame = gameGrid.GetGridCellFromPosition(middleCellPosition.x + 1, middleCellPosition.y);

            foreach (RoadGraphVertex vertex in startToLastCellList)
            {
                if (vertex.node == cellFrame)
                {
                    roadGraphVertex.edge.right = vertex;
                    vertex.edge.left = roadGraphVertex;
                }
            }
        }

        else if (roadDragDirection == RoadDirection.Direction_ZPlus)
        {

            cellFrame = gameGrid.GetGridCellFromPosition(middleCellPosition.x, middleCellPosition.y - 1);
            foreach (RoadGraphVertex vertex in startToLastCellList)
            {
                if (vertex.node == cellFrame)
                {
                    roadGraphVertex.edge.down = vertex;
                    vertex.edge.up = roadGraphVertex;
                }
            }


        }
        else if (roadDragDirection == RoadDirection.Direction_ZMinus)
        {
            cellFrame = gameGrid.GetGridCellFromPosition(middleCellPosition.x, middleCellPosition.y + 1);
            foreach (RoadGraphVertex vertex in startToLastCellList)
            {
                if (vertex.node == cellFrame)
                {
                    roadGraphVertex.edge.up = vertex;
                    vertex.edge.down = roadGraphVertex;
                }
            }
        }
    }

    private void ConnectEdge(GridCell _gridCell, RoadGraphVertex roadGraphVertex, Vector2Int middleCellPosition, RoadDirection roadDragDirection)
    {

        // �浹 �� ��� ��

        if (roadList.TryGetValue(_gridCell, out RoadGraphVertex _roadGraphVertex))
        {
            // ������ �浹 �ߴ� ������ �̾� �ޱ�


            roadGraphVertex.edge.down = _roadGraphVertex.edge.down;
            roadGraphVertex.edge.up = _roadGraphVertex.edge.up;
            roadGraphVertex.edge.left = _roadGraphVertex.edge.left;
            roadGraphVertex.edge.right = _roadGraphVertex.edge.right;
            if (roadGraphVertex.edge.right == null)
                Debug.Log(CountNearByRoads(roadGraphVertex));

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
            Debug.Log(nearRoadsNum);
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

                            roadType = RoadType.RoadStraight;

                        }

                        // ���� ���� ��� ���� ��,
                        else if (vertex.edge.right == null)
                        {
                            if (vertex.edge.up != null)
                            {
                                roadType = RoadType.RoadCorner;
                                rotateRotation = Quaternion.Euler(0, 270, 0);
                            }
                            else
                            {
                                roadType = RoadType.RoadCorner;
                                rotateRotation = Quaternion.Euler(0, 180, 0);
                            }
                        }
                        else
                        {
                            if (vertex.edge.up != null)
                            {
                                roadType = RoadType.RoadCorner;
                            }
                            else
                            {
                                roadType = RoadType.RoadCorner;
                                rotateRotation = Quaternion.Euler(0, 90, 0);
                            }
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
                                rotateRotation = Quaternion.Euler(0, 270, 0);
                            }
                        }
                        // �Ʒ� ���� ��� ���� ��,
                        else
                        {
                            // ���������� ���� �� �ڳ�
                            if (vertex.edge.right != null)
                            {
                                roadType = RoadType.RoadCorner;
                                rotateRotation = Quaternion.Euler(0, 90, 0);
                            }
                            // �������� ���� �� �ڳ�
                            else
                            {
                                roadType = RoadType.RoadCorner;
                                rotateRotation = Quaternion.Euler(0, 180, 0);
                            }
                        }
                    }
                    break;
                case 3:
                    roadType = RoadType.RoadThreeWay;

                    if (vertex.edge.down == null)
                    {
                        rotateRotation = Quaternion.identity;
                    }
                    else if (vertex.edge.up == null)
                    {
                        rotateRotation = Quaternion.Euler(0, 180, 0);
                    }
                    else if (vertex.edge.left == null)
                    {
                        rotateRotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (vertex.edge.right == null)
                    {
                        rotateRotation = Quaternion.Euler(0, 270, 0);
                    }


                    break;
                case 4:
                    roadType = RoadType.RoadFourWay;
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

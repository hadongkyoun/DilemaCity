using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

public enum RoadDirection
{
    Direction_XPlus,
    Direction_XMinus,
    Direction_ZPlus,
    Direction_ZMinus,
}

public struct RoadPreviewData
{
    public GameObject roadGameObject;
    public RoadType roadType;
    public Vector2Int roadPosition;
    public Quaternion roadRotation;
    public bool previewOn;
}

// �� �� roadManager�� ���� RoadSystem���� ���� ����
public class RoadSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject RoadPrefab;

    [Tooltip("RoadEnd: 0, RoadStraight, RoadCorner, RoadThreeWay, RoadFourWay")]
    [SerializeField]
    private GameObject[] RoadPrefabs;


    private PreviewSystem previewSystem;

    private Vector2Int coordinatedGridPos;

    private GridCell startGrid;
    private GridCell currentGrid;
    private GridCell lastGrid;

    private Stack<GridCell> roads = new Stack<GridCell>();
    private Dictionary<GridCell, RoadPreviewData> previewRoads = new Dictionary<GridCell, RoadPreviewData>();

    private GridCell[] nearByCells = new GridCell[4];

    private GameGrid gameGrid;

    [SerializeField]
    private RoadGraph roadGraph;


    private RoadDirection currentDragDirection;
    private void Start()
    {
        previewSystem = GetComponentInParent<PreviewSystem>();
    }

    public void RoadSystemUpdate(GridCell _gridCell)
    {
        //Instantiate(RoadPrefab, new Vector3(_position.x, 0.5f, _position.y), Quaternion.identity);
        if (Input.GetMouseButtonDown(0))
        {
            startGrid = _gridCell;
        }
        // Drag
        else if (Input.GetMouseButton(0))
        {
            currentGrid = _gridCell;
            currentDragDirection = DragDirection();
        }
        else
        {
            lastGrid = _gridCell;
            // ��ġ Ȯ��
            CreateRealRoad();
        }
    }

    public void StartCreatePreviewRoad(GameGrid _gameGrid)
    {
        gameGrid = _gameGrid;
        if (currentGrid == null)
        {
            return;
        }

        SetPreviewRoadFromStartToLast();
    }

    void SetPreviewRoadFromStartToLast()
    {
        // �巡�� �� ��ġ �� ���� ��ġ ����
        RoadPreviewData roadSample = new RoadPreviewData();
        roadSample.roadRotation = Quaternion.identity;

        switch (currentDragDirection)
        {

            case RoadDirection.Direction_XPlus:
                for (int i = startGrid.GetPosition().x; i <= currentGrid.GetPosition().x; i++)
                {
                    roadSample.roadPosition = new Vector2Int(i, startGrid.GetPosition().y);
                    CreatePreviewRoad(roadSample);
                }
                break;

            case RoadDirection.Direction_XMinus:

                for (int i = startGrid.GetPosition().x; i >= currentGrid.GetPosition().x; i--)
                {
                    roadSample.roadPosition = new Vector2Int(i, startGrid.GetPosition().y);
                    CreatePreviewRoad(roadSample);
                }
                break;

            case RoadDirection.Direction_ZPlus:
                for (int i = startGrid.GetPosition().y; i <= currentGrid.GetPosition().y; i++)
                {
                    roadSample.roadPosition = new Vector2Int(startGrid.GetPosition().x, i);
                    roadSample.roadRotation = Quaternion.Euler(0, 90, 0);

                    CreatePreviewRoad(roadSample);
                }
                break;
            case RoadDirection.Direction_ZMinus:
                for (int i = startGrid.GetPosition().y; i >= currentGrid.GetPosition().y; i--)
                {
                    roadSample.roadPosition = new Vector2Int(startGrid.GetPosition().x, i);
                    roadSample.roadRotation = Quaternion.Euler(0, 90, 0);

                    CreatePreviewRoad(roadSample);
                }
                break;
            default:
                break;
        }
    }


    private void CreatePreviewRoad(RoadPreviewData roadSample)
    {
        GridCell coordinatedGridCell = gameGrid.GetGridCellFromPosition(roadSample.roadPosition.x, roadSample.roadPosition.y);
        GameObject previewRoad = null;

        //RoadType roadType = RoadType.RoadEnd;
        //switch (CheckRoadNumNearBy(roadSample))
        //{
        //    case 0:
        //        roadType = RoadType.RoadEnd;
        //        break;
        //    case 1:
        //        roadType = RoadType.RoadEnd;
        //        break;
        //    case 2:
        //        roadType = RoadType.RoadThreeWay;
        //        break;
        //    case 3:
        //        roadType = RoadType.RoadFourWay;
        //        break;
        //}


        if (previewRoads.TryGetValue(coordinatedGridCell, out RoadPreviewData roadPreviewData))
        {
            if (!roadPreviewData.previewOn)
            {
                previewRoad = previewSystem.StartShowingPlacementPreview(RoadPrefabs[(int)RoadType.RoadStraight], roadSample.roadPosition, roadSample.roadRotation);
                roadSample.roadGameObject = previewRoad;
                roadSample.previewOn = true;
                previewRoads.Add(coordinatedGridCell, roadSample);
                roads.Push(coordinatedGridCell);
            }
        }
        else
        {
            previewRoad = previewSystem.StartShowingPlacementPreview(RoadPrefabs[(int)RoadType.RoadStraight], roadSample.roadPosition, roadSample.roadRotation);
            roadSample.roadGameObject = previewRoad;
            roadSample.previewOn = true;
            previewRoads.Add(coordinatedGridCell, roadSample);
            roads.Push(coordinatedGridCell);
        }
        Debug.Log(previewRoads.Count);

    }
    //// ������ ������ ���� �����ϴ� �Լ� ( �� �� ThreeWay���� ��� �� ���� )
    //private RoadType DetermineRoadType(GameGrid _gameGrid, RoadPreviewData _roadSample)
    //{

    //}

    // �ֺ� ���ο� ��� ���ΰ� �ִ��� Ȯ���ϴ� �Լ�
    private int CheckRoadNumNearBy(RoadPreviewData _roadSample)
    {
        int roadNum = 0;
        GridCell _gridCell = null;

        _gridCell = gameGrid.GetGridCellFromPosition(_roadSample.roadPosition.x - 1, _roadSample.roadPosition.y);
        if (_gridCell.isOccupied == false && _gridCell.objectInThisGridSpace && _gridCell.ReturnObjectType().Equals(ObjectDataType.RoadType))
        {
            if (gameGrid.GetGridCellFromPosition(_roadSample.roadPosition.x - 1, _roadSample.roadPosition.y).ReturnObjectType().Equals(ObjectDataType.RoadType))
            {
                roadNum++;
            }
        }

        if (gameGrid.GetGridCellFromPosition(_roadSample.roadPosition.x + 1, _roadSample.roadPosition.y).objectInThisGridSpace)
        {
            if (gameGrid.GetGridCellFromPosition(_roadSample.roadPosition.x + 1, _roadSample.roadPosition.y).ReturnObjectType().Equals(ObjectDataType.RoadType))
                roadNum++;
        }

        if (gameGrid.GetGridCellFromPosition(_roadSample.roadPosition.x, _roadSample.roadPosition.y - 1).objectInThisGridSpace)
        {
            if (gameGrid.GetGridCellFromPosition(_roadSample.roadPosition.x, _roadSample.roadPosition.y - 1).ReturnObjectType().Equals(ObjectDataType.RoadType))
                roadNum++;
        }

        if (gameGrid.GetGridCellFromPosition(_roadSample.roadPosition.x, _roadSample.roadPosition.y + 1).objectInThisGridSpace)
        {
            if (gameGrid.GetGridCellFromPosition(_roadSample.roadPosition.x, _roadSample.roadPosition.y + 1).ReturnObjectType().Equals(ObjectDataType.RoadType))
                roadNum++;
        }

        return roadNum;
    }
    private void CreateRealRoad()
    {
        int index = previewRoads.Count - 1;
        while (previewRoads.Count > 0)
        {

            GridCell gridFrameForRoad = roads.Pop();
            if (previewRoads.TryGetValue(gridFrameForRoad, out RoadPreviewData _previewRoadData))
            {

                roadGraph.ConstructRoad(RoadPrefabs, gridFrameForRoad, _previewRoadData);

                gridFrameForRoad.DefineObjectDataType(ObjectDataType.RoadType);
                gridFrameForRoad.isOccupied = true;

                _previewRoadData.previewOn = false;
                Destroy(_previewRoadData.roadGameObject);
                index--;
                previewRoads.Remove(gridFrameForRoad);
            }

        }

        ResetRoadSystem();
    }


    // �巡�� �ϴ� ���⿡ ���õ� �Լ�
    private RoadDirection DragDirection()
    {
        int xAmount = currentGrid.GetPosition().x - startGrid.GetPosition().x;
        int zAmount = currentGrid.GetPosition().y - startGrid.GetPosition().y;


        ////
        //if(Mathf.Abs(xAmount) < Mathf.Abs(zAmount))
        //{

        //}

        if (Mathf.Abs(xAmount) < Mathf.Abs(zAmount))
        {


            // z minus �������� �巡�� ��
            if (zAmount < 0)
            {
                ClearPreviewRoadsWhenChangedDirection(RoadDirection.Direction_ZMinus);
                return RoadDirection.Direction_ZMinus;
            }
            else
            {
                ClearPreviewRoadsWhenChangedDirection(RoadDirection.Direction_ZPlus);
                return RoadDirection.Direction_ZPlus;
            }

        }
        else
        {


            if (xAmount < 0)
            {
                ClearPreviewRoadsWhenChangedDirection(RoadDirection.Direction_XMinus);
                return RoadDirection.Direction_XMinus;
            }
            else
            {
                ClearPreviewRoadsWhenChangedDirection(RoadDirection.Direction_XPlus);
                return RoadDirection.Direction_XPlus;
            }
        }
    }

    private void ClearPreviewRoadsWhenChangedDirection(RoadDirection _direction)
    {
        if (currentDragDirection != _direction)
        {
            foreach (var item in previewRoads)
            {
                Destroy(item.Value.roadGameObject);
            }
            previewRoads.Clear();
        }
    }

    public void ResetRoadSystem()
    {
        roads.Clear();
        previewRoads.Clear();
        startGrid = null;
        currentGrid = null;
        lastGrid = null;
    }
}

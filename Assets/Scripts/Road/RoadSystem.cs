using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    Direction_X,
    Direction_Z
}


// 추 후 roadManager는 따로 RoadSystem으로 정리 예정
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
    private Stack<RoadData> previewRoads = new Stack<RoadData>();

    private GridCell[] nearByCells = new GridCell[4];

    private GameGrid gameGrid;
    private struct RoadData
    {
        public GameObject roadGameObject;
        public RoadType roadType;
        public Vector2Int roadPosition;
        public Quaternion roadRotation;
    }

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
            // 설치 확정
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
        // 드래그 시 설치 될 도로 위치 조정
        RoadData roadSample = new RoadData();
        roadSample.roadRotation = Quaternion.identity;
        if (currentDragDirection == RoadDirection.Direction_X)
        {
            for (int i = startGrid.GetPosition().x; i <= currentGrid.GetPosition().x; i++)
            {

                roadSample.roadPosition = new Vector2Int(i, startGrid.GetPosition().y);
                GridCell coordinatedGridCell = gameGrid.GetGridCellFromPosition(roadSample.roadPosition.x, roadSample.roadPosition.y);

                if (coordinatedGridCell.objectInThisGridSpace != null)
                {
                    continue;
                }

                CreatePreviewRoad(coordinatedGridCell, roadSample);
            }
        }
        else
        {
            for (int i = startGrid.GetPosition().y; i <= currentGrid.GetPosition().y; i++)
            {

                roadSample.roadPosition = new Vector2Int(startGrid.GetPosition().x, i);
                roadSample.roadRotation = Quaternion.Euler(0, 90, 0);
                GridCell coordinatedGridCell = gameGrid.GetGridCellFromPosition(roadSample.roadPosition.x, roadSample.roadPosition.y);

                if (coordinatedGridCell.objectInThisGridSpace != null)
                {
                    continue;
                }

                CreatePreviewRoad(coordinatedGridCell, roadSample);

            }
        }
    }


    private void CreatePreviewRoad(GridCell coordinatedGridCell, RoadData roadSample)
    {
        GameObject previewRoad = null;

        previewRoad = previewSystem.StartShowingPlacementPreview(RoadPrefabs[(int)RoadType.RoadStraight], roadSample.roadPosition, roadSample.roadRotation);
        roadSample.roadGameObject = previewRoad;
        roadSample.roadType = RoadType.RoadStraight;


        previewRoads.Push(roadSample);
        coordinatedGridCell.objectInThisGridSpace = previewRoad;
        roads.Push(coordinatedGridCell);

    }
    private void CreateRealRoad()
    {
        int index = 0;
        while (previewRoads.Count > 0)
        {
            RoadData previewRoadsData = previewRoads.Pop();
            GridCell gridFrameForRoad = roads.Pop();
            // 이미 도로가 설치 돼 있는 경우에는
            if (gridFrameForRoad.isOccupied)
            {
                if (gridFrameForRoad.ReturnObjectType().Equals(ObjectDataType.RoadType))
                {
                    gridFrameForRoad.objectInThisGridSpace = Instantiate(RoadPrefabs[(int)previewRoadsData.roadType],
                   new Vector3(previewRoadsData.roadPosition.x, 0.5f, previewRoadsData.roadPosition.y), previewRoadsData.roadRotation);


                    // 주변 도로 상황에 따라 도로 변경
                    gridFrameForRoad.ChangeRoadFromNearByData();

                    


                    
                }
            }
            else
            {
                gridFrameForRoad.isOccupied = true;
                gridFrameForRoad.objectInThisGridSpace = Instantiate(RoadPrefabs[(int)previewRoadsData.roadType],
                new Vector3(previewRoadsData.roadPosition.x, 0.5f, previewRoadsData.roadPosition.y), previewRoadsData.roadRotation);
                gridFrameForRoad.DefineObjectDataType(ObjectDataType.RoadType);
            }
            Destroy(previewRoadsData.roadGameObject);
            index++;
        }

        ResetRoadSystem();
    }

    // 도로의 방향을 최종 결정하는 함수
    private void DetermineRoadDirection(int _roadNumNearBy)
    {
        switch (_roadNumNearBy)
        {
            case 1:
                
                break;
            case 2:

                break;
            case 3:

                break;
            case 4:
                
                break;
        }
    }


    // 드래그 하는 방향에 관련된 함수
    private RoadDirection DragDirection()
    {
        int xAmount = Mathf.Abs(startGrid.GetPosition().x - currentGrid.GetPosition().x);
        int zAmount = Mathf.Abs(startGrid.GetPosition().y - currentGrid.GetPosition().y);
        if (zAmount > xAmount)
        {
            if (currentDragDirection == RoadDirection.Direction_X)
            {
                while (previewRoads.Count > 0)
                {
                    Destroy(previewRoads.Pop().roadGameObject);
                }
            }
            return RoadDirection.Direction_Z;
        }
        else
        {
            if (currentDragDirection == RoadDirection.Direction_Z)
            {
                while (previewRoads.Count > 0)
                {
                    Destroy(previewRoads.Pop().roadGameObject);
                }
            }
            return RoadDirection.Direction_X;
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

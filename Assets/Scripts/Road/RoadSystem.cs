using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



public enum RoadDirection
{
    Default,
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

// 추 후 roadManager는 따로 RoadSystem으로 정리 예정
public class RoadSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject RoadPrefab;

    [SerializeField]
    [Tooltip("RoadEnd: 0, RoadStraight, RoadCorner, RoadThreeWay, RoadFourWay")]
    private GameObject[] RoadPrefabs;

    private PreviewSystem previewSystem;

    private Vector2Int coordinatedGridPos;

    private GridCell startGrid;
    private GridCell currentGrid;
    private GridCell lastGrid;

    private Dictionary<GridCell, RoadPreviewData> previewRoads = new Dictionary<GridCell, RoadPreviewData>();

    private GridCell[] nearByCells = new GridCell[4];

    private GameGrid gameGrid;

    [SerializeField]
    private RoadGraph roadGraph;


    private RoadDirection currentDragDirection = RoadDirection.Default;
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
        else if(Input.GetMouseButtonUp(0))
        {
            lastGrid = _gridCell;
            // 설치 확정
            CreateRealRoad();
        }
    }

    public void StartCreatePreviewRoad(GameGrid _gameGrid)
    {
        gameGrid = _gameGrid;
        if (startGrid == null || currentGrid == null)
        {
            return;
        }

        SetPreviewRoadFromStartToLast();
    }

    void SetPreviewRoadFromStartToLast()
    {
        // 드래그 시 설치 될 도로 위치 조정
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


        if (previewRoads.Count > 0)
        {
            if (previewRoads.TryGetValue(coordinatedGridCell, out RoadPreviewData roadPreviewData) && roadPreviewData.previewOn)
            {
                return;
            }
            else
            {
                previewRoad = previewSystem.StartShowingPlacementPreview(RoadPrefabs[(int)RoadType.RoadStraight], roadSample.roadPosition, roadSample.roadRotation);
                roadSample.roadGameObject = previewRoad;
                roadSample.previewOn = true;
                previewRoads.Add(coordinatedGridCell, roadSample);
            }
        }
        else
        {
            previewRoad = previewSystem.StartShowingPlacementPreview(RoadPrefabs[(int)RoadType.RoadEnd], roadSample.roadPosition, roadSample.roadRotation);
            roadSample.roadGameObject = previewRoad;
            roadSample.previewOn = true;
            previewRoads.Add(coordinatedGridCell, roadSample);
        }

    }
    //// 도로의 방향을 최종 결정하는 함수 ( 추 후 ThreeWay에서 사용 될 예정 )
    //private RoadType DetermineRoadType(GameGrid _gameGrid, RoadPreviewData _roadSample)
    //{

    //}
    private void CreateRealRoad()
    {
        foreach(var roadData in previewRoads)
        {
            roadGraph.ConstructRoad(roadData.Key, roadData.Value, currentDragDirection);
            Destroy(roadData.Value.roadGameObject);
        }

        roadGraph.UpdateRoadFromNearByRoads(RoadPrefabs, currentDragDirection);
        ResetRoadSystem();
    }


    // 드래그 하는 방향에 관련된 함수
    private RoadDirection DragDirection()
    {
        if(startGrid == null)
        {
            return RoadDirection.Default;
        }
        int xAmount = currentGrid.GetPosition().x - startGrid.GetPosition().x;
        int zAmount = currentGrid.GetPosition().y - startGrid.GetPosition().y;


        ////
        //if(Mathf.Abs(xAmount) < Mathf.Abs(zAmount))
        //{

        //}

        if (Mathf.Abs(xAmount) < Mathf.Abs(zAmount))
        {


            // z minus 방향으로 드래그 중
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
        previewRoads.Clear();
        startGrid = null;
        currentGrid = null;
        lastGrid = null;
        //currentDragDirection = RoadDirection.Default;
    }
}

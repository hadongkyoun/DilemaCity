using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameGrid gameGrid;
    public int toggleSystemID = -1;
    [SerializeField] private LayerMask whatIsAGridLayer;
    GridCell gridCell = null;
    GridCell lastGridCell = null;

    private BuildingSystem buildingSystem;
    private RoadSystem roadSystem;
    public bool gridChanged = false;



    void Start()
    {
        gameGrid = FindObjectOfType<GameGrid>();
        buildingSystem = GetComponentInChildren<BuildingSystem>();
        roadSystem = GetComponentInChildren<RoadSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        gridCell = IsMouseOverAGridSpace();
        if (toggleSystemID == 0)
        {
            if (gridCell != null)
            {
                buildingSystem.BuildingSystemUpdate(gameGrid, gridCell);
                if (gridChanged)
                {
                    buildingSystem.CheckCanBuild(gameGrid, gridCell.GetPosition().x, gridCell.GetPosition().y);
                    gridChanged = false;
                }
            }
        }
        else if (toggleSystemID == 1)
        {
            if (gridCell != null)
            {
                roadSystem.RoadSystemUpdate(gridCell);

                if(gridChanged)
                {
                    // 실질적인 도로 생성 시작
                    roadSystem.StartCreatePreviewRoad(gameGrid);
                    gridChanged = false;
                }
            }
        }
    }

    // Returns the grid cell if mouse is over a grid cell and returns null if it is not
    private GridCell IsMouseOverAGridSpace()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, whatIsAGridLayer))
        {
            if (hitInfo.transform.TryGetComponent<GridCell>(out GridCell _gridCell))
            {
                // Building DetermineCanBuild를 자주 호출 하지 않기 위해 gridCell 참조를 하여 커서 이동시에만 확인하도록 함.
                if (lastGridCell != _gridCell)
                {
                    gridChanged = true;
                    lastGridCell = _gridCell;
                }
                return _gridCell;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return lastGridCell;
        }
    }

    public void WhichSystemActivated(int _id) 
    {
        switch (_id)
        {
            case 0:
                toggleSystemID = 0;
                roadSystem.ResetRoadSystem();
                break;
            case 1:
                toggleSystemID = 1;
                buildingSystem.ResetBuildingSystem();
                break;
            default:
                toggleSystemID = -1;
                break;
        }
    }

    public GridCell GetLastGrid()
    {
        return lastGridCell;
    }
}

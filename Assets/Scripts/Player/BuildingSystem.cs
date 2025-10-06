using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField]
    private PreviewSystem previewSystem;

    [SerializeField]
    private GameObject BuildingPrefab;
    private BuildingData buildingData;
    private bool needToCheckCanBuild = false;

    // 추 후 roadManager는 따로 RoadSystem으로 정리 예정
    [SerializeField]
    private RoadManager roadManager;

    private int activatedManagerId = -1;

    GameGrid gameGrid;
    [SerializeField] private LayerMask whatIsAGridLayer;
    GridCell gridCell = null;
    GridCell lastGridCell = null;

    void Start()
    {
        gameGrid = FindObjectOfType<GameGrid>();
    }


    void Update()
    {
        gridCell = IsMouseOverAGridSpace();
        if (gridCell != null)
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (buildingData != null && buildingData.canBuild)
                {
                    //gridCell.GetComponent<MeshRenderer>().material.color = Color.red;
                    if (activatedManagerId == 1)
                    {
                        Instantiate(BuildingPrefab, new Vector3(gridCell.GetPosition().x, 0.5f, gridCell.GetPosition().y), Quaternion.identity);
                        Vector2Int buildingSize = Vector2Int.zero;


                        if(buildingData != null)
                        {
                            buildingData.ConstructBuildingComplete(gameGrid, gridCell.GetPosition().x, gridCell.GetPosition().y);
                        }
                        else
                        {
                            Debug.Log("ERROR!! : There is no preview building system. No building data!!");
                        }

                        buildingData = null;
                        previewSystem.StopShowingPreview();
                    }
                    //else if (activatedManagerId == 2)
                    //{
                    //    roadManager.InstantiateRoad(gridCell.GetPosition());
                    //    gridCell.isOccupied = true;
                    //}

                }
            }
            else
            {
                if (activatedManagerId == 1)
                {
                    if (previewSystem.GetPreviewObject == null)
                    {
                        previewSystem.StartShowingPlacementPreview(BuildingPrefab, gridCell.GetPosition());
                    }
                    else
                    {
                        if (buildingData == null)
                        {
                            buildingData = previewSystem.GetPreviewObject.GetComponent<BuildingData>();
                        }
                        if (needToCheckCanBuild)
                        {
                            buildingData.DetermineCanBuild(gameGrid, gridCell.GetPosition().x, gridCell.GetPosition().y);
                            needToCheckCanBuild = false;
                        }
                        previewSystem.UpdatePosition(new Vector3(gridCell.GetPosition().x, 0.5f, gridCell.GetPosition().y), buildingData.canBuild);
                    }
                    
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
                if(lastGridCell != _gridCell)
                {
                    needToCheckCanBuild = true;
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
            return null;
        }
    }

    public void SetManagerID(int _id)
    {
        buildingData = null;
        activatedManagerId = _id;
    }
}

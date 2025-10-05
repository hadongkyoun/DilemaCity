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
    // 추 후 roadManager는 따로 RoadSystem으로 정리 예정
    [SerializeField]
    private RoadManager roadManager;

    private int activatedManagerId = -1;

    GameGrid gameGrid;
    [SerializeField] private LayerMask whatIsAGridLayer;

    GridCell gridCell = null;

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
                // 그리드에 오브젝트가 있는가?
                Debug.Log(gridCell.isOccupied);

                if (!gridCell.isOccupied)
                {
                    //gridCell.GetComponent<MeshRenderer>().material.color = Color.red;
                    if (activatedManagerId == 1)
                    {
                        Instantiate(BuildingPrefab, new Vector3(gridCell.GetPosition().x, 0.5f, gridCell.GetPosition().y), Quaternion.identity);
                        gridCell.isOccupied = true;
                    }
                    else if (activatedManagerId == 2)
                    {
                        roadManager.InstantiateRoad(gridCell.GetPosition());
                        gridCell.isOccupied = true;
                    }

                }
            }
            else
            {
                if (activatedManagerId == 1)
                {
                    if (previewSystem.GetPreviewObject == null)
                        previewSystem.StartShowingPlacementPreview(BuildingPrefab, gridCell.GetPosition());
                    previewSystem.UpdatePosition(new Vector3(gridCell.GetPosition().x, 0.5f, gridCell.GetPosition().y), true);
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
        activatedManagerId = _id;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private BuildingManager buildingManager;
    [SerializeField]
    private RoadManager roadManager;

    private int activatedManagerId = -1;

    GameGrid gameGrid;
    [SerializeField] private LayerMask whatIsAGridLayer;

    GridCell cellMouseIsOver = null;
    void Start()
    {
        gameGrid = FindObjectOfType<GameGrid>();
    }


    void Update()
    {
        cellMouseIsOver = IsMouseOverAGridSpace();
        if (cellMouseIsOver != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 그리드에 오브젝트가 있는가?
                Debug.Log(cellMouseIsOver.isOccupied);
                //if(cellMouseIsOver.isOccupied == false)
                //{

                //}


                //cellMouseIsOver.GetComponent<MeshRenderer>().material.color = Color.red;
                if (activatedManagerId == 2)
                {
                    roadManager.InstantiateRoad(cellMouseIsOver.GetPosition());

                }
                else if (activatedManagerId == 1)
                {
                    buildingManager.InstantiateBuilding(cellMouseIsOver.GetPosition());
                }
            }
        }
    }

    // Returns the grid cell if mouse is over a grid cell and returns null if it is not
    private GridCell IsMouseOverAGridSpace()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, whatIsAGridLayer))
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    [SerializeField]
    private Vector2Int buildingSize;

    // x 양수 z 양수 (1) , x 음수 z 양수 (2) , x 음수 z 음수 (3) , x 양수 z 음수 (4)
    public int buildingDirection = 1;
    public bool canBuild = false;

    private List<GameObject> cellsUnderBuilding = new List<GameObject>();


    public void DetermineCanBuild(GameGrid _gameGrid, int startGridPosX, int startGridPosZ)
    {
        switch (buildingDirection)
        {
            case 1:
                for (int i = 0; i < buildingSize.x; i++)
                {
                    for (int j = 0; j < buildingSize.y; j++)
                    {
                        GameObject cellObject;
                        cellObject = _gameGrid.GetGridCellFromPosition(startGridPosX + i, startGridPosZ + j);

                        if(cellObject == null)
                        {
                            canBuild = false;
                            cellsUnderBuilding.Clear();
                            return;
                        }
                        else
                        {
                            if (cellObject != null && cellObject.TryGetComponent<GridCell>(out GridCell _gridCell))
                            {
                                if (_gridCell.isOccupied)
                                {
                                    canBuild = false;
                                    cellsUnderBuilding.Clear();
                                    return;
                                }
                            }
                        }
                    }
                }
                canBuild = true;
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    public void ConstructBuildingComplete(GameGrid _gameGrid, int startGridPosX, int startGridPosZ)
    {
        for (int i = 0; i < buildingSize.x; i++)
        {
            for (int j = 0; j < buildingSize.y; j++)
            {
                GameObject cellObject;
                cellObject = _gameGrid.GetGridCellFromPosition(startGridPosX + i, startGridPosZ + j);
                if (cellObject != null && cellObject.TryGetComponent<GridCell>(out GridCell _gridCell))
                {
                    _gridCell.isOccupied = true;
                }
            }

            cellsUnderBuilding.Clear();
            canBuild = false;
        }
    }
}

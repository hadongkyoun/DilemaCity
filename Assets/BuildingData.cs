using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    [SerializeField]
    private Vector2Int buildingSize;

    // x ��� z ��� (1) , x ���� z ��� (2) , x ���� z ���� (3) , x ��� z ���� (4)
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
                        GridCell cell = null;
                        GameObject cellObject = null;
                        cell = _gameGrid.GetGridCellFromPosition(startGridPosX + i, startGridPosZ + j);

                        if(cell != null)
                        {
                            cellObject = cell.gameObject;
                        }

                        // �ǹ��� ���� ��� ���
                        if(cellObject == null)
                        {
                            canBuild = false;
                            cellsUnderBuilding.Clear();
                            return;
                        }
                        else
                        {
                            // �ǹ� ������ ��ġ �� �ǹ��� �ִ� ���
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
                cellObject = _gameGrid.GetGridCellFromPosition(startGridPosX + i, startGridPosZ + j).gameObject;
                if (cellObject != null && cellObject.TryGetComponent<GridCell>(out GridCell _gridCell))
                {
                    _gridCell.isOccupied = true;
                    _gridCell.DefineObjectDataType(ObjectDataType.BuildingType);
                    _gridCell.objectInThisGridSpace = cellObject;
                }
            }

            cellsUnderBuilding.Clear();
            canBuild = false;
        }
    }
}

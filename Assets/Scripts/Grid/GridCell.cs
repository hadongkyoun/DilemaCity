using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectDataType
{
    RoadType,
    BuildingType,
}



public class GridCell : MonoBehaviour
{

    private int posX;
    private int posZ;

    private ObjectDataType objectDataType = default;

    // 이 Grid Cell 안에 있는 GameObject
    public GameObject objectInThisGridSpace = null;

    // 이 Grid Cell이 GameObject를 가지고 있는가?
    public bool isOccupied = false;
    public void SetPosition(int x, int z)
    {
        posX = x;
        posZ = z;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posZ);
    }

    public GridCell GetGridCellFromPosition(int x, int z)
    {
        if (posX == x && posZ == z)
            return this;
        else
        {
            Debug.Log("Error : There is no gridcell with given position.");
            return null;
        }
    }

    public ObjectDataType ReturnObjectType()
    {
        if (objectDataType != default)
        {
            return objectDataType;

        }
        else
        {
            return default;
        }
    }

    public void DefineObjectDataType(ObjectDataType _type)
    {
        objectDataType = _type;
    }

    public void ResetGridCellInformation()
    {
        isOccupied = false;
        objectInThisGridSpace = null;
        objectDataType = default;
        //roadDirection = default;
    }

}

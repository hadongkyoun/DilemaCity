using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private int posX;
    private int posZ;

    // 이 Grid Cell 안에 있는 GameObject
    public GameObject objectInThisGridSpace = null;

    // 이 Grid Cell이 GameObject를 가지고 있는가?
    public bool isOccupied = false;

    // Grid에서 이 Grid Cell의 Position 정보
    public void SetPosition(int x, int z)
    {
        posX = x;
        posZ = z;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posZ);
    }
}

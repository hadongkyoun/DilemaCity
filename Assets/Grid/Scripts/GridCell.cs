using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private int posX;
    private int posZ;

    // �� Grid Cell �ȿ� �ִ� GameObject
    public GameObject objectInThisGridSpace = null;

    // �� Grid Cell�� GameObject�� ������ �ִ°�?
    public bool isOccupied = false;

    // Grid���� �� Grid Cell�� Position ����
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    // Size of game board of grid ( 10x10 ���� Cell �� �� )
    [Header("Grid Size")]
    [SerializeField]
    private int height = 10;
    [SerializeField]
    private int width = 10;
    [Space(10)]

    [Header("Grid and Grid Space size")]
    [Tooltip("�׸��忡 �Ҵ�Ǵ� Grid Cell Prefab ������ ������ ���� �����ϴ�.")]
    [SerializeField]
    // Each Grid Size
    private float gridSpaceSize = 5f;

    [SerializeField]
    private GameObject gridCellPrefab;
    private GameObject[,] gameGrid;

    void Start()
    {
        // height * width �� ��ŭ Cell �� ����.
        gameGrid = new GameObject[height, width];
        StartCoroutine(CreateGrid());
    }

    private IEnumerator CreateGrid()
    {
        if (gridCellPrefab == null)
        {
            Debug.LogError("ERROR: Grid Cell Prefab on the Game grid is not assigned");
            yield return null;
        }

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                gameGrid[x, z] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, -gridCellPrefab.transform.localScale.y/2f, z * gridSpaceSize), Quaternion.identity);
                if (gameGrid[x, z].TryGetComponent<GridCell>(out GridCell _gridCell))
                {
                    // �׸��� ��ǥ�� ���� ������ (���� �� �ǹ���ġ�� ����)
                    _gridCell.SetPosition(x, z);
                }
                gameGrid[x, z].transform.parent = transform;
                gameGrid[x, z].gameObject.name = $"Grid Space ( X: {x}, Z: {z} )";

                yield return new WaitForSeconds(.005f);
            }
        }
    }

    // Gets the grid position from world position
    public Vector2Int GetGridPosFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridSpaceSize);
        int z = Mathf.FloorToInt(worldPosition.z / gridSpaceSize);

        x = Mathf.Clamp(x, 0, width);
        z = Mathf.Clamp(z, 0, height);

        return new Vector2Int(x, z);
    }

    // Gets the world position of a grid position

    public Vector3 GetWorldPosFromGridPos(Vector2Int gridPos)
    {
        float x = gridPos.x * gridSpaceSize;
        float y = gridPos.y * gridSpaceSize;

        return new Vector3(x, 0, y);
    }
}

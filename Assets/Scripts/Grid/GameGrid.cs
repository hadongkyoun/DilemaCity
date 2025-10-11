using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    // Grid Size에 맞춰서 변경하기 위함
    [SerializeField]
    private GameObject GroundPrefab;

    // Size of game board of grid ( 10x10 개의 Cell 이 들어감 )
    [Header("Grid Size : width and height must be same")]
    [SerializeField]
    private int height = 10;
    [SerializeField]
    private int width = 10;
    [Space(10)]

    [Header("Grid and Grid Space size")]
    [Tooltip("그리드에 할당되는 Grid Cell Prefab 끼리의 간격이 조정 가능하다.")]
    [SerializeField]
    // Each Grid Size
    private int gridSpaceSize = 5;

    [SerializeField]
    private GameObject gridCellPrefab;
    private GameObject[,] gameGrid;

    private IsometricCameraHandler isometricCameraHandler;
    void Start()
    {
        // height * width 수 만큼 Cell 이 들어간다.
        gameGrid = new GameObject[height, width];
        StartCoroutine(CreateGrid());

        GroundPrefab.transform.localScale = new Vector3(width, 1, height);
        if (width % 2 == 1)
        {
            GroundPrefab.transform.position = new Vector3(width / 2, 0, height / 2);
        }
        else
        {
            GroundPrefab.transform.position = new Vector3(width / 2 -0.5f, 0, height / 2 - 0.5f);
        }

        Camera.main.GetComponentInParent<IsometricCameraHandler>().SetLimitHandlerSizeAndStartPosition(new Vector2(-20, width + 20), new Vector2(-20, height + 20), new Vector3(width / 2, 0, height / 2));
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
                gameGrid[x, z] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0.501f, z * gridSpaceSize), Quaternion.Euler(90, 0, 0));
                if (gameGrid[x, z].TryGetComponent<GridCell>(out GridCell _gridCell))
                {
                    // 그리드 좌표에 관한 정보만 (도로 및 건물설치를 위해)
                    _gridCell.SetPosition(x * gridSpaceSize, z * gridSpaceSize);
                }
                gameGrid[x, z].transform.parent = transform;
                gameGrid[x, z].gameObject.name = $"Grid Space ( X: {x}, Z: {z} )";

                yield return null;
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

    public GridCell GetGridCellFromPosition(int x, int z)
    {
        if (height <= x || width <= z)
        {
            return null;
        }
        if(x < 0 || z < 0)
        {
            return null;
        }

        if (gameGrid[x,z].TryGetComponent<GridCell>(out GridCell gridCell))
        {
            return gridCell;
        }
        else
        {
            return null;
        }
    }
}

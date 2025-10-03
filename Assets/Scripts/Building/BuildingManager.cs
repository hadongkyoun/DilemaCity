using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField]
    private GameObject BuildingPrefab;

    public void InstantiateBuilding(Vector2Int _position)
    {
        Instantiate(BuildingPrefab, new Vector3(_position.x, 0, _position.y), Quaternion.identity);
    }
}

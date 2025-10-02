using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [SerializeField]
    private GameObject RoadPrefab;

    public void InstantiateRoad(Vector2Int _position)
    {
        Instantiate(RoadPrefab, new Vector3(_position.x, 0, _position.y), Quaternion.identity);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 추 후 roadManager는 따로 RoadSystem으로 정리 예정
public class RoadManager : MonoBehaviour
{
    [SerializeField]
    private GameObject RoadPrefab;

    public void InstantiateRoad(Vector2Int _position)
    {
        Instantiate(RoadPrefab, new Vector3(_position.x, 0.5f, _position.y), Quaternion.identity);
    }
}

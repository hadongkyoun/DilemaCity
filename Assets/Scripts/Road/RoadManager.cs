using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �� �� roadManager�� ���� RoadSystem���� ���� ����
public class RoadManager : MonoBehaviour
{
    [SerializeField]
    private GameObject RoadPrefab;

    public void InstantiateRoad(Vector2Int _position)
    {
        Instantiate(RoadPrefab, new Vector3(_position.x, 0.5f, _position.y), Quaternion.identity);
    }
}

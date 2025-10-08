using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{

    private PreviewSystem previewSystem = null;
    private GameObject previewObject = null;

    [SerializeField]
    private GameObject BuildingPrefab;
    private BuildingData buildingData;

    private void Start()
    {
        previewSystem = GetComponentInParent<PreviewSystem>();
    }

    public void BuildingSystemUpdate(GameGrid _gameGrid, GridCell _gridCell)
    {
        if (_gridCell != null)
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (buildingData != null && buildingData.canBuild)
                {
                    _gridCell.objectInThisGridSpace = Instantiate(BuildingPrefab, 
                        new Vector3(_gridCell.GetPosition().x, 0.5f, _gridCell.GetPosition().y), Quaternion.identity);
                    
                    Vector2Int buildingSize = Vector2Int.zero;

                    buildingData.ConstructBuildingComplete(_gameGrid, _gridCell.GetPosition().x, _gridCell.GetPosition().y);
                    

                    ResetBuildingSystem();
                }
            }
            else
            {
                // 건물 미리보기를 위한 오브젝트가 준비 돼 있지 않은 경우
                if (previewObject == null)
                {
                    previewObject = previewSystem.StartShowingPlacementPreview(BuildingPrefab, _gridCell.GetPosition(), Quaternion.identity);
                }
                else
                {
                    if (buildingData == null)
                    {
                        buildingData = previewObject.GetComponent<BuildingData>();
                    }

                    previewSystem.UpdatePosition(new Vector3(_gridCell.GetPosition().x, 0.5f, _gridCell.GetPosition().y), buildingData.canBuild);
                }


            }
        }
    }

    public void CheckCanBuild(GameGrid _gameGrid, int x, int y)
    {
        if (buildingData != null)
        {
            buildingData.DetermineCanBuild(_gameGrid, x, y);
        }
    }
    public void ResetBuildingSystem()
    {
        buildingData = null;
        previewSystem.StopShowingPreview();
        previewObject = null;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    private GameObject PreviewObject;
    public GameObject GetPreviewObject { get { return PreviewObject; } }

    [SerializeField]
    private Material PreviewMaterialsPrefab;
    private Material PreviewMaterialInstance;



    private void Start()
    {
        PreviewMaterialInstance = new Material(PreviewMaterialsPrefab);
    }

    public GameObject StartShowingPlacementPreview(GameObject _prefab, Vector2Int _position, Quaternion _rotation)
    {
        PreviewObject = Instantiate(_prefab, new Vector3(_position.x, 0.5f, _position.y), _rotation);
        PreparePreview(PreviewObject);

        return PreviewObject;
    }

    private void PreparePreview(GameObject _previewObject)
    {
        MeshRenderer[] renderers = _previewObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = PreviewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }


    public void StopShowingPreview()
    {
        if (PreviewObject != null)
        {
            ApplyFeedback(true);
            Destroy(PreviewObject);
        }
    }

    public void UpdatePosition(Vector3 _position, bool validity)
    {
        MovePreviewObject(_position);
        ApplyFeedback(validity);
    }

    private void MovePreviewObject(Vector3 _position)
    {
        PreviewObject.transform.position = new Vector3(_position.x, 0.5f, _position.z);
    }

    // 설치 가능 여부에 따른 preview 색 변화
    private void ApplyFeedback(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        PreviewMaterialInstance.color = c;
    }
}

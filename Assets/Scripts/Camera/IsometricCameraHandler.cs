using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCameraHandler : MonoBehaviour
{
    private Vector2 handlerLimitX;
    private Vector2 handlerLimitZ;
    [Space(10)]
    [Header("Speed")]
    private float camSpeed = 6f;
    private Camera realCamera;
    [Space(10)]
    [Header("Zoom")]
    [SerializeField]
    private float zoomSpeed = 6;
    [SerializeField]
    private float zoomSmoothness = 5;
    [SerializeField]
    private float minZoom = 2;
    [SerializeField]
    private float maxZoom = 200;

    private float currentZoom;


    void Awake()
    {
        realCamera = GetComponentInChildren<Camera>();
        currentZoom = maxZoom;
        realCamera.orthographicSize = currentZoom;
    }

    public void SetLimitHandlerSizeAndStartPosition(Vector2 limitHandlerX, Vector2 limitHandlerZ, Vector3 startPosition)
    {
        transform.position = new Vector3(startPosition.x, transform.position.y, startPosition.z);
        handlerLimitX = limitHandlerX;
        handlerLimitZ = limitHandlerZ;
    }

    void Update()
    {
        Vector2 camDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.position += Quaternion.Euler(0, realCamera.transform.eulerAngles.y, 0) * new Vector3(camDirection.x, 0, camDirection.y) * (camSpeed * Time.deltaTime);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, handlerLimitX.x, handlerLimitX.y),
        transform.position.y, Mathf.Clamp(transform.position.z, handlerLimitZ.x, handlerLimitZ.y));

        currentZoom = Mathf.Clamp(currentZoom - Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        realCamera.orthographicSize = Mathf.Lerp(realCamera.orthographicSize, currentZoom, zoomSmoothness * Time.deltaTime);
    }

    public void RotateCamLeft()
    {
        transform.Rotate(Vector3.up, -90);
    }
    public void RotateCamRight()
    {
        transform.Rotate(Vector3.up, 90);
    }
}

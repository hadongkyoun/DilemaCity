using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCameraHandler : MonoBehaviour
{
    [Header("Limit max cam position")]
    [SerializeField]
    private Vector2 handlerLimitX;
    [SerializeField]
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
    private float maxZoom = 40;

    private float currentZoom;

    void Start()
    {
       realCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        Vector2 camDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.position += Quaternion.Euler(0,realCamera.transform.eulerAngles.y, 0)*new Vector3(camDirection.x, 0, camDirection.y) * (camSpeed * Time.deltaTime);

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

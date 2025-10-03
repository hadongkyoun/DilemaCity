using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCameraHandler : MonoBehaviour
{
    public float camSpeed = 6f;
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 camDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.position += Quaternion.Euler(0, camera.transform.eulerAngles.y, 0)*new Vector3(camDirection.x, 0, camDirection.y) * (camSpeed * Time.deltaTime);
    }
}

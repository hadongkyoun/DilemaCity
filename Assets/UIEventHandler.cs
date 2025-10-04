using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEventHandler : MonoBehaviour
{
    [SerializeField]
    private Button rotateLeftBtn;
    [SerializeField]
    private Button rotateRightBtn;

    private IsometricCameraHandler isoCamHandler;

    void Start()
    {
        isoCamHandler = Camera.main.GetComponentInParent<IsometricCameraHandler>();
        rotateLeftBtn.onClick.AddListener(RotateCamLeft);
        rotateRightBtn.onClick.AddListener(RotateCamRight);
    }

    private void RotateCamLeft()
    {
        if (isoCamHandler != null)
        {
            isoCamHandler.RotateCamLeft();
        }
    }
    private void RotateCamRight()
    {
        if (isoCamHandler != null)
        {
            isoCamHandler.RotateCamRight();
        }
    }
}

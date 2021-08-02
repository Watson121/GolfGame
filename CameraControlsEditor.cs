using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Level Editor Camera Controls
/// Similar to how the main camera works but has a bit more control
/// </summary>
public class CameraControlsEditor : MonoBehaviour
{
    public Camera playerCamera;
    private Vector3 previousCamPosition;
    private float cameraSpeed = 6.0f;

    // Update is called once per frame
    void Update()
    {
        #region Camera Rotation

        if (Input.GetMouseButtonDown(1)) previousCamPosition = playerCamera.ScreenToViewportPoint(Input.mousePosition);

        if (Input.GetMouseButton(1))
        {
            Vector3 direction = previousCamPosition - playerCamera.ScreenToViewportPoint(Input.mousePosition);

            float yRot = direction.y * 180;

            playerCamera.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            playerCamera.transform.Rotate(new Vector3(0, 1, 0), direction.x * -180, Space.World);

            Vector3 currentRotation = playerCamera.transform.localRotation.eulerAngles;
            currentRotation.z = 0;
            playerCamera.transform.localRotation = Quaternion.Euler(currentRotation);

            previousCamPosition = playerCamera.ScreenToViewportPoint(Input.mousePosition);

        }

        if (Input.GetButton("EditorSpeed")) cameraSpeed = 10.0f;
        else if (Input.GetButtonUp("EditorSpeed")) cameraSpeed = 6.0f;

        playerCamera.transform.Translate(Input.GetAxis("EditorHorizontal") * cameraSpeed * Time.deltaTime, 0, 
            Input.GetAxis("EditorVertical") * cameraSpeed * Time.deltaTime);


        #endregion

    }
}

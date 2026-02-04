using UnityEngine;
using NaughtyAttributes;

public class PlayerCameraRotation : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] Vector2 xySensitivity = Vector2.one;
    [SerializeField, MinMaxSlider(-90f, 90f)] Vector2 minMaxYAngle = Vector2.zero;

    private void Start()
    {
        MouseToggle("-");
    }

    private void Update()
    {
        RotateCamera();
    }

    private void RotateCamera()
    {
        var mouseAxis = GetMouseAxis();

        var yAxis = mouseAxis.x;
        var xAxis = mouseAxis.y;

        var yRotation = transform.localEulerAngles.y + yAxis * xySensitivity.x * Time.deltaTime;
        var xRotation = playerCamera.transform.localEulerAngles.x - xAxis * xySensitivity.y * Time.deltaTime;

        if (xRotation > 180f) xRotation -= 360f;
        xRotation = Mathf.Clamp(xRotation, minMaxYAngle.x, minMaxYAngle.y);

        LookAt(new Vector2(xRotation, yRotation));
    }

    public Vector2 GetMouseAxis()
    {
        var xAxis = Input.GetAxis("Mouse X");
        var yAxis = Input.GetAxis("Mouse Y");
        return new Vector2(xAxis, yAxis);
    }

    public void LookAt(Vector2 direction)
    {
        transform.localEulerAngles = new Vector3(0, direction.y, 0);
        playerCamera.transform.localEulerAngles = new Vector3(direction.x, 0, 0);
    }

    public void MouseToggle(string state = "~")
    {
        bool visible = Cursor.visible;
        CursorLockMode lockState = Cursor.lockState;

        if (state == "~")
        {
            visible = !visible;
            lockState = lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        }
        else if (state == "-")
        {
            visible = false;
            lockState = CursorLockMode.Locked;
        }
        else
        {
            visible = true;
            lockState = CursorLockMode.None;
        }

        Cursor.visible = visible; Cursor.lockState = lockState;
    }

    public void CameraToggle(string state = "~")
    {
        bool toggle = playerCamera.activeSelf;

        if (state == "~")
        {
            toggle = !toggle;
        }
        else if (state == "-")
        {
            toggle = false;
        }
        else
        {
            toggle = true;
        }

        playerCamera.SetActive(toggle);
    }
}

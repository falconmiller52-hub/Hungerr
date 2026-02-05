using UnityEngine;
using NaughtyAttributes;

public class PlayerCameraRotation : MonoBehaviour
{
    [SerializeField, Label("Player Camera Assign")] GameObject playerCamera;
    [Space, SerializeField, Label("Camera Sensitivity of X and Y axis")] Vector2 xySensitivity = Vector2.one;
    [SerializeField, Label("Minimum and Maximum Angle of Y axis"), MinMaxSlider(-90f, 90f)] Vector2 minMaxYAngle = Vector2.zero;

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
        var mouseAxis = MouseAxis;

        var yAxis = mouseAxis.x;
        var xAxis = mouseAxis.y;

        var yRotation = transform.localEulerAngles.y + yAxis * Sensitivity.x * Time.deltaTime;
        var xRotation = playerCamera.transform.localEulerAngles.x - xAxis * Sensitivity.y * Time.deltaTime;

        if (xRotation > 180f) xRotation -= 360f;
        xRotation = Mathf.Clamp(xRotation, MinMaxYAngle.x, MinMaxYAngle.y);

        LookAt(new Vector2(xRotation, yRotation));
    }

    public void LookAt(Vector2 direction)
    {
        transform.localEulerAngles = new Vector3(0, direction.y, 0);
        CameraPivot.transform.localEulerAngles = new Vector3(direction.x, 0, 0);
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

    public Vector2 MouseAxis
    {
        get {
            var xAxis = Input.GetAxis("Mouse X");
            var yAxis = Input.GetAxis("Mouse Y");
            return new Vector2(xAxis, yAxis);
        }
    }

    public GameObject CameraPivot 
    { 
        get { return playerCamera; } 
        set { playerCamera = value; } 
    }

    public Vector2 Sensitivity 
    { 
        get { return xySensitivity; } 
        set { xySensitivity = value; } 
    }

    public Vector2 MinMaxYAngle 
    { 
        get { return minMaxYAngle; } 
        set { minMaxYAngle = value; } 
    }
}

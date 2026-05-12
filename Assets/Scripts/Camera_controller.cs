using UnityEngine;

public class Camera_controller : MonoBehaviour
{
    public float mouseSensitivity = 200f; //마우스감도

    public Transform playerBody; 

    float xRotation = 0f;
    float mouseX;
    float mouseY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 고정
    }

    void Update()
    {

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // 카메라는 Player랑 같이 돌아감
        playerBody.Rotate(Vector3.up * mouseX); //Player 회전 
    }
}

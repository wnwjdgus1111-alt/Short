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
        // ── [추가됨] 단서 보드가 열려있으면 카메라 회전 차단 ──
        // 📘 ClueBoardManager.IsBoardOpen은 static 변수이므로
        //    인스턴스 없이 클래스명.변수명으로 바로 접근할 수 있습니다.
        //    보드가 열려있으면 마우스 회전을 하지 않고 즉시 return합니다.
        if (ClueBoardManager.IsBoardOpen) return;

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // 카메라는 Player랑 같이 돌아감
        playerBody.Rotate(Vector3.up * mouseX); //Player 회전 
    }
}

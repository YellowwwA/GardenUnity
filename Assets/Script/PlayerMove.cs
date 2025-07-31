using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f; // 도/초

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        //transform.rotation = Quaternion.Normalize(transform.rotation);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D
        float moveZ = Input.GetAxisRaw("Vertical");   // W/S

        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            // 시점 기준으로 방향 변환
            Vector3 moveDir = transform.TransformDirection(inputDir);

            // ✅ 전진(W), 좌(A), 우(D)일 때만 회전 (뒤로 이동할 때는 회전 안 함)
            if (moveZ >= 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // 이동
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }
    }
}

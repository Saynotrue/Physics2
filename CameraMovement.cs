using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 50f;

    void Update()
    {
        // 이동 (WASD + Q/E)
        float horizontal = Input.GetAxis("Horizontal"); // A, D
        float vertical = Input.GetAxis("Vertical");     // W, S

        Vector3 move = new Vector3(horizontal, 0, vertical);

        if (Input.GetKey(KeyCode.E)) move.y -= 1;
        if (Input.GetKey(KeyCode.Q)) move.y += 1;

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.Self);

        // 회전 (J/L 좌우, I/K 상하)
        float yaw = 0f;   // 좌우 회전
        float pitch = 0f; // 상하 회전

        if (Input.GetKey(KeyCode.J)) yaw -= 1;
        if (Input.GetKey(KeyCode.L)) yaw += 1;

        if (Input.GetKey(KeyCode.I)) pitch -= 1;
        if (Input.GetKey(KeyCode.K)) pitch += 1;

        transform.Rotate(Vector3.up * yaw * rotateSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.right * pitch * rotateSpeed * Time.deltaTime, Space.Self);
    }
}
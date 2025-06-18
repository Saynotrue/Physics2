using UnityEngine;

public class RocketController : MonoBehaviour
{
    public float angle = 45f;    // 발사 각도
    public float power = 10f;    // 발사 힘

    private Rigidbody rb;
    private bool launched = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !launched)
        {
            Launch();
            launched = true;
        }
    }

    void Launch()
    {
        // 발사 방향 계산 (XZ 평면에서 위로 angle도)
        float rad = angle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        rb.AddForce(direction * power, ForceMode.Impulse);
    }
}
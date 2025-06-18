using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class RocketAutoLauncher : MonoBehaviour
{
    [Header("Target Input UI")]
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    public TMP_InputField inputZ;
    public Button launchButton;

    [Header("Output UI")]
    public TMP_Text angleText;
    public TMP_Text speedText;

    [Header("Target Transform")]
    public Transform targetTransform;

    [Header("Physics Params")]
    public float rocketMass = 1.0f;
    public float dragCoefficient = 0.5f;
    public float crossSectionalArea = 0.01f;
    public float airDensity = 1.225f;
    public float gravity = 9.81f;

    [Header("Launch Search")]
    public float angleMin = 20f;
    public float angleMax = 80f;
    public float speedMin = 5f;
    public float speedMax = 100f;
    public float simulationTimeStep = 0.02f;
    public float acceptableDistance = 1.0f;

    [Header("Trajectory Line")]
    public LineRenderer trajectoryLine;
    public int maxSteps = 500;

    private Rigidbody rb;
    private bool launched = false;
    private Vector3 targetPos;
    private float selectedAngle;
    private float selectedSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -gravity, 0);
        launchButton.onClick.AddListener(OnLaunchButtonClicked);

        // 라인 렌더러 두께 조절 (더 얇게)
        if (trajectoryLine != null)
        {
            trajectoryLine.startWidth = 0.25f; // 시작 두께
            trajectoryLine.endWidth = 0.25f;   // 끝 두께
        }

    }

    void OnLaunchButtonClicked()
    {
        if (launched) return;

        float x = float.Parse(inputX.text);
        float y = float.Parse(inputY.text);
        float z = float.Parse(inputZ.text);
        targetPos = new Vector3(x, y, z);

        if (targetTransform != null)
        {
            targetTransform.position = targetPos; // 타겟 오브젝트 이동
        }

        Debug.Log("Launching towards: " + targetPos);
        TryAutoLaunch();
    }

    void TryAutoLaunch()
    {
        Vector3 startPos = transform.position;
        Vector3 horizontal = new Vector3(targetPos.x - startPos.x, 0, targetPos.z - startPos.z);
        float heightDiff = targetPos.y - startPos.y;

        for (float angle = angleMin; angle <= angleMax; angle += 1f)
        {
            for (float speed = speedMin; speed <= speedMax; speed += 1f)
            {
                Vector3 dir = horizontal.normalized;
                Vector3 launchDir = Quaternion.AngleAxis(angle, Vector3.Cross(dir, Vector3.up)) * dir;
                Vector3 velocity = launchDir * speed;

                if (SimulateTrajectory(startPos, velocity, out List<Vector3> points))
                {
                    rb.linearVelocity = velocity;
                    ShowTrajectory(points);
                    selectedAngle = angle;
                    selectedSpeed = speed;

                    // UI 텍스트 업데이트
                    angleText.text = $"{selectedAngle:F1}°";
                    speedText.text = $"{selectedSpeed:F1} m/s";

                    launched = true;
                    return;
                }
            }
        }

        Debug.Log("No valid trajectory found.");
    }

    bool SimulateTrajectory(Vector3 startPos, Vector3 velocity, out List<Vector3> trajectoryPoints)
    {
        Vector3 pos = startPos;
        Vector3 vel = velocity;
        trajectoryPoints = new List<Vector3> { pos };

        for (int i = 0; i < maxSteps; i++)
        {
            Vector3 dragDir = -vel.normalized;
            float speed = vel.magnitude;
            Vector3 dragForce = 0.5f * airDensity * dragCoefficient * crossSectionalArea * speed * speed * dragDir;

            Vector3 gravityForce = rocketMass * Vector3.down * gravity;
            Vector3 totalForce = gravityForce + dragForce;
            Vector3 acceleration = totalForce / rocketMass;

            vel += acceleration * simulationTimeStep;
            pos += vel * simulationTimeStep;
            trajectoryPoints.Add(pos);

            if (Vector3.Distance(pos, targetPos) < acceptableDistance)
                return true;

            if (pos.y < 0)
                break;
        }

        return false;
    }

    void ShowTrajectory(List<Vector3> points)
    {
        trajectoryLine.positionCount = points.Count;
        trajectoryLine.SetPositions(points.ToArray());
    }

    void FixedUpdate()
    {
        if (!launched) return;

        // 현재 속도
        Vector3 velocity = rb.linearVelocity;
        float speed = velocity.magnitude;

        if (speed <= 0.01f) return;

        // 항력 방향 = 속도 반대
        Vector3 dragDir = -velocity.normalized;

        // 항력 계산
        Vector3 dragForce = 0.5f * airDensity * dragCoefficient * crossSectionalArea * speed * speed * dragDir;

        // 항력 힘 적용
        rb.AddForce(dragForce);
    }

    float CalculateLaunchSpeed(Vector3 targetPos)
    {
        float distance = Vector3.Distance(targetPos, transform.position);
        return Mathf.Sqrt(distance * gravity); // 중력 기반 예시 속도 계산
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetLaunch();
        }
    }

    void ResetLaunch()
    {
        // 물리 속성 초기화
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 로켓 위치 초기화 (원하는 위치로 변경 가능)
        transform.position = Vector3.zero;

        // 상태 초기화
        launched = false;

        // 궤적 지우기
        if (trajectoryLine != null)
        {
            trajectoryLine.positionCount = 0;
        }

        // UI 초기화
        angleText.text = "각도:";
        speedText.text = "속력:";

        // (선택) 타겟 위치 초기화
        // targetTransform.position = new Vector3(기본_x, 기본_y, 기본_z);
    }
}
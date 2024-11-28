using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // 이동 속도
    public float directionChangeInterval = 3f; // 방향 전환 주기
    public Vector2 mapSize = new Vector2(10f, 10f); // 맵 크기 (x: 가로, y: 세로)
    public LayerMask groundLayer; // 타일 레이어 마스크

    private Vector2 targetPosition; // 목표 위치
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetNewTargetPosition(); // 시작 시 랜덤 위치 설정
        InvokeRepeating(nameof(SetNewTargetPosition), directionChangeInterval, directionChangeInterval);
    }

    void FixedUpdate()
    {
        MoveTowardsTarget(); // 목표 위치로 부드럽게 이동
    }

    // 목표 위치로 부드럽게 이동
    void MoveTowardsTarget()
    {
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

    // 새로운 랜덤 목표 위치 설정
    void SetNewTargetPosition()
    {
        // 맵 내에서 랜덤 위치를 설정
        float randomX = Random.Range(0.5f, mapSize.x - 0.5f); // 맵의 X 범위 내 랜덤 값
        float randomY = Random.Range(0.5f, mapSize.y - 0.5f); // 맵의 Y 범위 내 랜덤 값

        targetPosition = new Vector2(randomX, randomY); // 목표 위치 설정
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SetNewTargetPosition(); // 충돌 시 새로운 목표 위치 설정
    }
}
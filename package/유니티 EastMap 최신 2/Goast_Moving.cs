using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // 이동 속도
    public Vector2 mapSize = new Vector2(10f, 10f); // 맵 크기 (x: 가로, y: 세로)

    public int maxHP = 5; // 유령의 최대 체력
    private int currentHP; // 유령의 현재 체력

    public float attackRange = 1.5f; // 근접 공격 범위
    public int attackDamage = 1; // 근접 공격 데미지
    public float attackCooldown = 1f; // 공격 쿨타임

    private float lastAttackTime; // 마지막 공격 시간
    private Vector2 moveDirection; // 이동 방향
    private GameObject player; // 플레이어 참조
    private Animator animator; // 애니메이터 컴포넌트

    private bool isDying = false; // 죽는 중인지 확인

    private Vector2[] waypoints; // 유령이 맴돌 위치
    private int currentWaypointIndex = 0; // 현재 목표 지점 인덱스

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHP = maxHP; // 체력을 초기화

        // 경로 지점 설정 (맵의 네 구석을 따라 순환)
        waypoints = new Vector2[]
        {
            new Vector2(0.5f, 0.5f), // 왼쪽 아래
            new Vector2(0.5f, mapSize.y - 0.5f), // 왼쪽 위
            new Vector2(mapSize.x - 0.5f, mapSize.y - 0.5f), // 오른쪽 위
            new Vector2(mapSize.x - 0.5f, 0.5f) // 오른쪽 아래
        };

        moveDirection = (waypoints[0] - (Vector2)transform.position).normalized; // 첫 번째 방향 설정

        player = GameObject.FindGameObjectWithTag("Player"); // Player 태그를 가진 오브젝트를 찾음
        if (player == null)
        {
            Debug.LogWarning("Player 오브젝트가 씬에 없습니다. Player 태그를 확인하세요.");
        }
    }

    void Update()
    {
        if (!isDying) // 죽는 중이 아니면 이동
        {
            MoveToWaypoint();
            if (player != null && Time.time - lastAttackTime >= attackCooldown)
            {
                CheckAndAttackPlayer(); // 근접 공격 체크 및 실행
            }
        }
    }

    // 유령 체력을 감소시키는 함수
    public void TakeDamage(int damage)
    {
        if (isDying) return; // 죽는 중이면 무시
        currentHP -= damage;
        Debug.Log($"유령 체력: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die(); // 체력이 0 이하가 되면 제거
        }
    }

    // 유령 제거 함수
    void Die()
    {
        if (isDying) return; // 이미 죽는 중이라면 무시
        isDying = true;
        Debug.Log("유령이 사라졌습니다!");
        animator.SetTrigger("Die"); // Die 애니메이션 트리거 실행
        Invoke(nameof(DestroyGhost), 1.5f); // 애니메이션이 끝난 후 오브젝트 제거 (1.5초 딜레이)
    }

    // 유령 오브젝트 제거
    void DestroyGhost()
    {
        Destroy(gameObject);
    }

    // 경로 지점을 따라 이동
    void MoveToWaypoint()
    {
        Vector2 targetPosition = waypoints[currentWaypointIndex];
        Vector2 currentPosition = transform.position;

        // 목표 지점으로 이동
        Vector2 direction = (targetPosition - currentPosition).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        // 목표 지점에 도달했는지 확인
        if (Vector2.Distance(currentPosition, targetPosition) <= 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // 다음 목표 지점으로 변경
        }
    }

    // 트리거 충돌 처리 (OnTriggerEnter2D)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어와 충돌 시
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage); // 플레이어 체력 감소
                Debug.Log("유령이 플레이어에게 충돌하여 데미지를 입혔습니다!");
            }
            else
            {
                Debug.LogWarning("PlayerHealth 컴포넌트를 찾을 수 없습니다!");
            }
        }
    }

    // 플레이어와의 거리 확인 후 공격
    void CheckAndAttackPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer(); // 공격 범위 내에 있으면 공격
        }
    }

    // 플레이어를 공격하는 함수
    void AttackPlayer()
    {
        lastAttackTime = Time.time; // 마지막 공격 시간 갱신

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>(); // 플레이어의 체력 컴포넌트 가져오기
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage); // 플레이어에게 데미지 입히기
            Debug.Log("유령이 플레이어를 공격했습니다!");
        }
        else
        {
            Debug.LogWarning("PlayerHealth 스크립트를 찾을 수 없습니다.");
        }
    }
}

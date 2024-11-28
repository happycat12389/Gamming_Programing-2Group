using System.Collections;
using UnityEngine;

public class Monster3Moving : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float changeDirectionTime = 2f;
    public Vector3 moveArea = new Vector3(2f, 0f, 5f); // x, y, z 범위

    private Vector3 moveDirection;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        ChooseNewDirection();
        StartCoroutine(ChangeDirectionPeriodically());
    }

    void Update()
    {
        MoveMonster();
    }

    void MoveMonster()
    {
        // 이동 방향대로 몬스터 이동
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // 이동 범위 제한
        Vector3 newPosition = transform.position;

        // X, Z 범위 제한 (화면 밖으로 나가지 않도록)
        newPosition.x = Mathf.Clamp(newPosition.x, startPosition.x - moveArea.x, startPosition.x + moveArea.x);
        newPosition.z = Mathf.Clamp(newPosition.z, startPosition.z - moveArea.z, startPosition.z + moveArea.z);

        // Y값을 그대로 두고, X, Z만 조정
        transform.position = newPosition;
    }

    private IEnumerator ChangeDirectionPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeDirectionTime);
            ChooseNewDirection();
        }
    }

    void ChooseNewDirection()
    {
        // X, Z 방향으로 랜덤 값 생성
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);
        moveDirection = new Vector3(randomX, 0, randomZ).normalized;
    }
}

using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 5;            // 공격 데미지
    public float attackCooldown = 1f;      // 공격 쿨다운
    public Transform attackPoint;          // 공격 시작 지점
    public float attackRange = 1f;         // 공격 범위
    public LayerMask monsterLayers;        // 몬스터 레이어 필터 (Enemy에서 Monster로 변경)

    private bool isAttacking = false;      // 공격 중인지 확인

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // 공격 애니메이션 트리거 (필요 시 추가)
        Debug.Log("플레이어가 공격 중!");

        // 공격 범위 내의 몬스터 탐지
        Collider[] hitMonsters = Physics.OverlapSphere(attackPoint.position, attackRange, monsterLayers);

        // 몬스터가 공격 범위 내에 있는지 확인하는 로그
        if (hitMonsters.Length > 0)
        {
            Debug.Log("공격 범위 내 몬스터 감지됨!");
        }
        else
        {
            Debug.Log("공격 범위 내에 몬스터가 없습니다.");
        }

        foreach (Collider monster in hitMonsters)
        {
            Debug.Log($"몬스터 {monster.name}이(가) 공격당했습니다!");
            Monster monsterScript = monster.GetComponent<Monster>();
            if (monsterScript != null)
            {
                monsterScript.TakeDamage(attackDamage);
                Debug.Log($"{monster.name}이(가) {attackDamage}만큼 피해를 입었습니다.");
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    // 공격 범위를 시각적으로 표시 (디버그용)
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

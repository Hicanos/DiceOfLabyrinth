using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Orc : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private float healthBarWidth = 100f;

    private bool isDead = false;
    public bool IsDead => isDead;

    private Vector3 savedPosition;
    private Quaternion savedRotation;

    public enum EnemyState
    {
        Idle,
        Walk,
        Hit,
        Duck,
        Jump,
        Jump_Idle,
        Jump_Landing,
        Attack,
        Defend,
        Dead
    }
    private EnemyState currentState;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        UpdateHealthBar();
    }

    private void SetState(EnemyState state)
    {
        PlayAnimationByState(state);
    }

    // 상태 검사 후 점프 스킬 실행
    public void TryJumpSkill(Vector3 targetPosition, float jumpHeight = 2f, float duration = 1f)
    {
        // 예시: 죽었거나 이미 점프 중이면 실행 안 함
        if (currentState == EnemyState.Dead || currentState == EnemyState.Jump || currentState == EnemyState.Jump_Idle)
            return;

        UseJumpSkill(targetPosition, jumpHeight, duration);
    }

    // 점프 스킬 실행
    public void UseJumpSkill(Vector3 targetPosition, float jumpHeight = 2f, float duration = 1f)
    {
        savedPosition = transform.position;
        savedRotation = transform.rotation;
        StartCoroutine(JumpSkillRoutine(targetPosition, jumpHeight, duration));
    }

    private IEnumerator JumpSkillRoutine(Vector3 targetPosition, float jumpHeight, float duration)
    {
        // 1. 점프 시작 모션
        PlayAnimationByState(EnemyState.Jump);
        yield return new WaitForSeconds(0.2f);

        // 2. 공중 체공 모션
        PlayAnimationByState(EnemyState.Jump_Idle);

        // 3. 포물선 이동
        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 end = targetPosition;

        Vector3 jumpDir = (end - start).normalized;
        if (jumpDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(jumpDir);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float height = 4 * jumpHeight * t * (1 - t);
            transform.position = Vector3.Lerp(start, end, t) + Vector3.up * height;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;

        // 4. 착지 모션
        PlayAnimationByState(EnemyState.Jump_Landing);
        BattleManager.Instance.EnemyAttack.EnemyAttackDealDamage();
        yield return new WaitForSeconds(0.2f);

        // 5. 타겟 방향으로 회전 후 걷기
        PlayAnimationByState(EnemyState.Walk);
        yield return RotateTowards(savedPosition, 0.2f);

        // 6. 제자리로 걷기
        PlayAnimationByState(EnemyState.Walk);
        float walkBackDuration = 0.5f;
        elapsed = 0f;
        Vector3 walkStart = transform.position;
        Vector3 walkEnd = savedPosition;
        while (elapsed < walkBackDuration)
        {
            float t = elapsed / walkBackDuration;
            transform.position = Vector3.Lerp(walkStart, walkEnd, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = walkEnd;

        // 7. 원래 방향으로 회전
        yield return RotateToRotation(savedRotation, 0.2f);

        // 8. 대기 상태로 전환
        PlayAnimationByState(EnemyState.Idle);
    }

    private IEnumerator RotateTowards(Vector3 targetPos, float duration)
    {
        Quaternion startRot = transform.rotation;
        Vector3 dir = (targetPos - transform.position).normalized;
        if (dir == Vector3.zero) yield break;
        Quaternion endRot = Quaternion.LookRotation(dir);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRot;
    }

    private IEnumerator RotateToRotation(Quaternion targetRot, float duration)
    {
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRot;
    }

    private void UpdateHealthBar()
    {
        // 체력바 갱신 로직
    }

    // 상태에 따라 애니메이션 트리거 및 상태 변경
    private void PlayAnimationByState(EnemyState state)
    {
        if (currentState == state) return; // 중복 상태 방지

        currentState = state; // 상태 변경

        switch (state)
        {
            case EnemyState.Idle:
                animator.SetTrigger("Idle");
                break;
            case EnemyState.Walk:
                animator.SetTrigger("Walk");
                break;
            case EnemyState.Hit:
                animator.SetTrigger("Hit");
                break;
            case EnemyState.Duck:
                animator.SetTrigger("Duck");
                break;
            case EnemyState.Jump:
                animator.SetTrigger("Jump");
                break;
            case EnemyState.Jump_Idle:
                animator.SetTrigger("JumpIdle");
                break;
            case EnemyState.Jump_Landing:
                animator.SetTrigger("JumpLanding");
                break;
            case EnemyState.Attack:
                animator.SetTrigger("Attack");
                break;
            case EnemyState.Defend:
                animator.SetTrigger("Defend");
                break;
            case EnemyState.Dead:
                animator.SetTrigger("Dead");
                break;
        }
    }
}
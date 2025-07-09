using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Gnoll : MonoBehaviour, IGnoll // 테스트에너미 클래스는 모든 에너미 클래스들이 구현해야하는 메서드들의 디폴트를 제공합니다. IEnemy 인터페이스를 상속받고 구체적인 구현을 해주세요
{
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private float healthBarWidth = 100f; // 기본 너비 설정

    private IGnoll.EnemyState currentState;
    private bool isDead = false;
    public bool IsDead => isDead;

    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private Vector3 targetPosition => Vector3.zero;// 배틀 매니저에서 설정한 타겟 포지션을 사용합니다. 임시로 Vector3.zero로 설정

    public List<Action> PassiveSkills { get; private set; }
    public List<Action> ActiveSkills { get; private set; }


    private void Awake()
    {
        Init();
        DoJumpAttack(Vector3.forward * -5 + Vector3.right * -3); // 테스트용 점프 어택
        //DoRightAttack(Vector3.forward * -5 + Vector3.right * -3); // 테스트용 라이트 어택
        //DoLeftAttack(Vector3.forward * -5 + Vector3.right * -3); // 테스트용 레프트 어택
        //DoKickAttack(Vector3.forward * -5 + Vector3.right * -3); // 테스트용 킥 어택
        //DoSpinAttack(); // 테스트용 스핀 어택
    }
    public void Init()
    {
        UpdateHealthBar();
        InitActiveSkills();
        //InitPassiveSkills();
    }

    private void UpdateHealthBar()
    {
        // Implement health bar update logic here
    }
    private void InitActiveSkills()
    {
        ActiveSkills = new List<Action>(new Action[15]);
        ActiveSkills[0] += () => DoKickAttack(targetPosition);
        ActiveSkills[1] += () => DoRightAttack(targetPosition);
        ActiveSkills[4] += () => DoJumpAttack(targetPosition);
    }
    //private void InitPassiveSkills()
    //{
    //    PassiveSkills = new List<Action>(new Action[12]); // 0~11, 12개 null로 초기화

    //    // 예시: 0번 패시브에 재생 메서드 할당
    //    PassiveSkills[0] = DoPassiveRegeneration;
    //    // PassiveSkills[1] = DoPassiveSomething; // 다른 패시브가 있다면 추가
    //    // 나머지는 null 유지
    //}
    public void DoJumpAttack(Vector3 targetPosition)
    {
        StartCoroutine(JumpAttackRoutine(targetPosition));
    }

    private IEnumerator JumpAttackRoutine(Vector3 targetPosition)
    {
        // 현재 위치와 회전 저장
        savedPosition = transform.position;
        savedRotation = transform.rotation;

        // 목표 방향으로 회전
        Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 점프어택 애니메이션 실행
        PlayAnimationByState(IGnoll.EnemyState.JumpAttack);

        // 점프어택 애니메이션 길이(고정값 사용)
        float jumpAttackDuration = 1f;

        // 이동 벡터 계산 (y값 고정)
        Vector3 start = transform.position;
        Vector3 end = new Vector3(targetPosition.x, start.y, targetPosition.z);
        Vector3 moveVector = (end - start) / jumpAttackDuration;

        float elapsed = 0f;
        while (elapsed < jumpAttackDuration)
        {
            float delta = Time.deltaTime;
            // 마지막 프레임 보정
            if (elapsed + delta > jumpAttackDuration)
                delta = jumpAttackDuration - elapsed;

            transform.position += moveVector * delta;
            elapsed += delta;
            yield return null;
        }
        transform.position = end;

        // 원래 위치로 돌아가기 전, 세이브 포지션 방향으로 회전
        Vector3 returnDir = (savedPosition - transform.position).normalized;
        if (returnDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(returnDir);
            float rotElapsed = 0f;
            float rotDuration = 0.2f; // 회전 시간 (조정 가능)
            Quaternion rotStart = transform.rotation;
            while (rotElapsed < rotDuration)
            {
                float t = rotElapsed / rotDuration;
                transform.rotation = Quaternion.Slerp(rotStart, targetRot, t);
                rotElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = targetRot;
        }

        // 런 애니메이션 실행하며 복귀
        PlayAnimationByState(IGnoll.EnemyState.Run);
        float returnDuration = 0.8f;
        Vector3 returnStart = transform.position;
        Vector3 returnEnd = savedPosition;

        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            float t = elapsed / returnDuration;
            transform.position = Vector3.Lerp(returnStart, returnEnd, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = returnEnd;

        // 세이브 로테이션으로 회전
        {
            Quaternion rotStart = transform.rotation;
            Quaternion rotEnd = savedRotation;
            float rotElapsed = 0f;
            float rotDuration = 0.2f;
            while (rotElapsed < rotDuration)
            {
                float t = rotElapsed / rotDuration;
                transform.rotation = Quaternion.Slerp(rotStart, rotEnd, t);
                rotElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = rotEnd;
        }

        // Idle 상태로 전환
        PlayAnimationByState(IGnoll.EnemyState.Idle);
    }

    public void DoRightAttack(Vector3 targetPosition)
    {
        StartCoroutine(RightAttackRoutine(targetPosition));
    }

    private IEnumerator RightAttackRoutine(Vector3 targetPosition)
    {
        // 현재 위치와 회전 저장
        savedPosition = transform.position;
        savedRotation = transform.rotation;

        // 목표 방향으로 회전
        Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 런 애니메이션 실행하며 목표 위치로 이동
        PlayAnimationByState(IGnoll.EnemyState.Run);
        float runDuration = 0.8f; // 이동 시간 (조정 가능)
        Vector3 start = transform.position;
        Vector3 end = new Vector3(targetPosition.x, start.y, targetPosition.z);

        float elapsed = 0f;
        while (elapsed < runDuration)
        {
            float t = elapsed / runDuration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;

        // 라이트 어택 애니메이션 실행
        PlayAnimationByState(IGnoll.EnemyState.RightAttack);
        float attackDuration = 1f; // 실제 애니메이션 길이로 조정

        yield return new WaitForSeconds(attackDuration);

        // 원래 위치로 돌아가기 전, 세이브 포지션 방향으로 회전
        Vector3 returnDir = (savedPosition - transform.position).normalized;
        if (returnDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(returnDir);
            float rotElapsed = 0f;
            float rotDuration = 0.2f;
            Quaternion rotStart = transform.rotation;
            while (rotElapsed < rotDuration)
            {
                float t = rotElapsed / rotDuration;
                transform.rotation = Quaternion.Slerp(rotStart, targetRot, t);
                rotElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = targetRot;
        }

        // 런 애니메이션 실행하며 복귀
        PlayAnimationByState(IGnoll.EnemyState.Run);
        float returnDuration = 0.8f;
        Vector3 returnStart = transform.position;
        Vector3 returnEnd = savedPosition;

        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            float t = elapsed / returnDuration;
            transform.position = Vector3.Lerp(returnStart, returnEnd, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = returnEnd;

        // 세이브 로테이션으로 회전
        {
            Quaternion rotStart = transform.rotation;
            Quaternion rotEnd = savedRotation;
            float rotElapsed = 0f;
            float rotDuration = 0.2f;
            while (rotElapsed < rotDuration)
            {
                float t = rotElapsed / rotDuration;
                transform.rotation = Quaternion.Slerp(rotStart, rotEnd, t);
                rotElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = rotEnd;
        }

        // Idle 상태로 전환
        PlayAnimationByState(IGnoll.EnemyState.Idle);
    }

    public void DoLeftAttack(Vector3 targetPosition)
    {
        StartCoroutine(LeftAttackRoutine(targetPosition));
    }

    private IEnumerator LeftAttackRoutine(Vector3 targetPosition)
    {
        // 현재 위치와 회전 저장
        savedPosition = transform.position;
        savedRotation = transform.rotation;

        // 목표 방향으로 회전
        Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 런 애니메이션 실행하며 목표 위치로 이동
        PlayAnimationByState(IGnoll.EnemyState.Run);
        float runDuration = 0.8f; // 이동 시간 (조정 가능)
        Vector3 start = transform.position;
        Vector3 end = new Vector3(targetPosition.x, start.y, targetPosition.z);

        float elapsed = 0f;
        while (elapsed < runDuration)
        {
            float t = elapsed / runDuration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;

        // 레프트 어택 애니메이션 실행
        PlayAnimationByState(IGnoll.EnemyState.LeftAttack);
        float attackDuration = 1f; // 실제 애니메이션 길이로 조정

        yield return new WaitForSeconds(attackDuration);

        // 원래 위치로 돌아가기 전, 세이브 포지션 방향으로 회전
        Vector3 returnDir = (savedPosition - transform.position).normalized;
        if (returnDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(returnDir);
            float rotElapsed = 0f;
            float rotDuration = 0.2f;
            Quaternion rotStart = transform.rotation;
            while (rotElapsed < rotDuration)
            {
                float t = rotElapsed / rotDuration;
                transform.rotation = Quaternion.Slerp(rotStart, targetRot, t);
                rotElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = targetRot;
        }

        // 런 애니메이션 실행하며 복귀
        PlayAnimationByState(IGnoll.EnemyState.Run);
        float returnDuration = 0.8f;
        Vector3 returnStart = transform.position;
        Vector3 returnEnd = savedPosition;

        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            float t = elapsed / returnDuration;
            transform.position = Vector3.Lerp(returnStart, returnEnd, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = returnEnd;

        // 세이브 로테이션으로 회전
        {
            Quaternion rotStart = transform.rotation;
            Quaternion rotEnd = savedRotation;
            float rotElapsed = 0f;
            float rotDuration = 0.2f;
            while (rotElapsed < rotDuration)
            {
                float t = rotElapsed / rotDuration;
                transform.rotation = Quaternion.Slerp(rotStart, rotEnd, t);
                rotElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = rotEnd;
        }

        // Idle 상태로 전환
        PlayAnimationByState(IGnoll.EnemyState.Idle);
    }
    public void DoKickAttack(Vector3 targetPosition)
    {
        StartCoroutine(KickAttackRoutine(targetPosition));
    }

    private IEnumerator KickAttackRoutine(Vector3 targetPosition)
    {
        // 현재 위치와 회전 저장
        savedPosition = transform.position;
        savedRotation = transform.rotation;

        // 목표 방향으로 회전
        Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 런 애니메이션 실행하며 목표 위치로 이동
        PlayAnimationByState(IGnoll.EnemyState.Run);
        float runDuration = 0.8f; // 이동 시간 (조정 가능)
        Vector3 start = transform.position;
        Vector3 end = new Vector3(targetPosition.x, start.y, targetPosition.z);

        float elapsed = 0f;
        while (elapsed < runDuration)
        {
            float t = elapsed / runDuration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;

        // 킥 어택 애니메이션 실행
        PlayAnimationByState(IGnoll.EnemyState.Kick);
        float attackDuration = 1f; // 실제 애니메이션 길이로 조정

        yield return new WaitForSeconds(attackDuration);

        // 원래 위치로 돌아가기 전, 세이브 포지션 방향으로 회전
        Vector3 returnDir = (savedPosition - transform.position).normalized;
        if (returnDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(returnDir);
            float rotElapsed = 0f;
            float rotDuration = 0.2f;
            Quaternion rotStart = transform.rotation;
            while (rotElapsed < rotDuration)
            {
                float t = rotElapsed / rotDuration;
                transform.rotation = Quaternion.Slerp(rotStart, targetRot, t);
                rotElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = targetRot;
        }

        // 런 애니메이션 실행하며 복귀
        PlayAnimationByState(IGnoll.EnemyState.Run);
        float returnDuration = 0.8f;
        Vector3 returnStart = transform.position;
        Vector3 returnEnd = savedPosition;

        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            float t = elapsed / returnDuration;
            transform.position = Vector3.Lerp(returnStart, returnEnd, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = returnEnd;

        // 세이브 로테이션으로 회전
        {
            Quaternion rotStart = transform.rotation;
            Quaternion rotEnd = savedRotation;
            float rotElapsed = 0f;
            float rotDuration = 0.2f;
            while (rotElapsed < rotDuration)
            {
                float t = rotElapsed / rotDuration;
                transform.rotation = Quaternion.Slerp(rotStart, rotEnd, t);
                rotElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = rotEnd;
        }

        // Idle 상태로 전환
        PlayAnimationByState(IGnoll.EnemyState.Idle);
    }
    public void DoSpinAttack()
    {
        StartCoroutine(SpinAttackRoutine());
    }

    private IEnumerator SpinAttackRoutine()
    {
        // 스핀어택 애니메이션 실행
        PlayAnimationByState(IGnoll.EnemyState.SpinAttack);
        float spinAttackDuration = 1f; // 실제 애니메이션 길이로 조정

        yield return new WaitForSeconds(spinAttackDuration);

        // Idle 상태로 전환
        PlayAnimationByState(IGnoll.EnemyState.Idle);
    }
    public void PlayAnimationByState(IGnoll.EnemyState state)
    {
        if (currentState == state) return; // 중복 상태 방지


        currentState = state; // 상태 변경

        switch (state)
        {
            case IGnoll.EnemyState.Idle:
                animator.SetTrigger("Idle");
                break;
            case IGnoll.EnemyState.RightAttack:
                animator.SetTrigger("RightAttack");
                break;
            case IGnoll.EnemyState.LeftAttack:
                animator.SetTrigger("LeftAttack");
                break;
            case IGnoll.EnemyState.SpinAttack:
                animator.SetTrigger("SpinAttack");
                break;
            case IGnoll.EnemyState.Stun:
                animator.SetTrigger("Stun");
                break;
            case IGnoll.EnemyState.JumpAttack:
                animator.SetTrigger("JumpAttack");
                break;
            case IGnoll.EnemyState.Hit:
                animator.SetTrigger("Hit");
                break;
            case IGnoll.EnemyState.Howling:
                animator.SetTrigger("Howling");
                break;
            case IGnoll.EnemyState.Kick:
                animator.SetTrigger("Kick");
                break;
            case IGnoll.EnemyState.Dead:
                animator.SetTrigger("Dead");
                break;
            case IGnoll.EnemyState.Run:
                animator.SetTrigger("Run");
                break;
        }
    }
}


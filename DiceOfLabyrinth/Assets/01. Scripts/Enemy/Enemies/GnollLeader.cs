using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GnollLeader : MonoBehaviour, IEnemy // 테스트에너미 클래스는 모든 에너미 클래스들이 구현해야하는 메서드들의 디폴트를 제공합니다. IEnemy 인터페이스를 상속받고 구체적인 구현을 해주세요
{
    [SerializeField] private Animator animator;

    private Vector3 savedPosition; // 현재 위치 저장
    private Quaternion savedRotation; // 현재 회전 저장

    public enum EnemyState // 에너미의 상태를 정의하는 열거형
    {
        Idle, // 대기 상태
        RightAttack, // 오른손 공격 상태, 0번 스킬
        SpinAttack, // 회전 공격 상태, 14번 스킬
        Stun, // 기절 상태
        StrongAttack, // 강력한 공격 상태, 1번 스킬
        JumpAttack, // 점프 공격 상태, 4번 스킬
        SlashDown, // 내려찍기 공격 상태, 5번 스킬
        TripleAttack, // 삼연속 공격 상태, 3번 스킬
        Run, // 달리기 상태
        Hit, // 맞은 상태
        Howling, // 울부짖는 상태
        Dead, // 사망 상태
    }
    private EnemyState currentState;
    public bool IsDead => BattleManager.Instance.Enemy.IsDead;

    public List<Action<Vector3>> ActiveSkills { get; private set; }

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        // 필요시 테스트용 스킬 호출 위치
         UseActiveSkill(4, 1);
    }
    public void Init()
    {
        ActiveSkills = new List<Action<Vector3>>(new Action<Vector3>[15]);
        ActiveSkills[0] += (pos) => DoRightAttack(pos);
        ActiveSkills[1] += (pos) => DoStrongAttack(pos);
        ActiveSkills[3] += (pos) => DoTripleAttack(pos);
        ActiveSkills[4] += (pos) => DoJumpAttack(pos);
        //ActiveSkills[5] += (pos) => DoSlashDownAttack(pos);
        //ActiveSkills[14] += (pos) => DoSpinAttack();

        savedPosition = transform.position; // 현재 위치 저장
        savedRotation = transform.rotation;
    }
    private Vector3 GetTargetPositionByIndex(int index)
    {
        // 플레이어 포메이션 정보를 스테이지 데이터에서 가져옴
        var stageSaveData = StageManager.Instance?.stageSaveData;
        var chapterData = StageManager.Instance?.chapterData;

        int chapterIdx = stageSaveData.currentChapterIndex;
        int stageIdx = stageSaveData.currentStageIndex;
        int formationIdx = (int)stageSaveData.currentFormationType;

        // 방어적: null 체크 및 인덱스 범위 체크
        if (chapterData == null ||
            chapterData.chapterIndex == null ||
            chapterIdx < 0 || chapterIdx >= chapterData.chapterIndex.Count)
            return Vector3.zero;

        var chapter = chapterData.chapterIndex[chapterIdx];
        if (chapter.stageData == null ||
            chapter.stageData.PlayerFormations == null ||
            formationIdx < 0 || formationIdx >= chapter.stageData.PlayerFormations.Count)
            return Vector3.zero;

        var formation = chapter.stageData.PlayerFormations[formationIdx];
        if (formation.PlayerPositions == null ||
            index < 0 || index >= formation.PlayerPositions.Count)
            return Vector3.zero;

        // 실제 포지션 반환 (index로 접근)
        return formation.PlayerPositions[index].Position;
    }
    public void UseActiveSkill(int skillIndex, int targetIndex)
    {

        if (skillIndex < 0 || skillIndex >= ActiveSkills.Count)
        {
            Debug.LogError("Invalid skill index: " + skillIndex);
            return;
        }
        Vector3 targetPos = GetTargetPositionByIndex(targetIndex);
        ActiveSkills[skillIndex]?.Invoke(targetPos);
    }
    public void TakeDamage()
    {
        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        PlayAnimationByState(EnemyState.Hit);

        // Hit 애니메이션 길이만큼 대기 (예: 0.7초)
        float hitDuration = 1.5F; // 실제 애니메이션 길이로 조정
        yield return new WaitForSeconds(hitDuration);

        if (IsDead)
            PlayAnimationByState(EnemyState.Dead);
        else
            PlayAnimationByState(EnemyState.Idle);
    }
    public void PlayAnimationByState(EnemyState state)
    {
        if (currentState == state) return; // 현재 상태와 동일하면 아무 작업도 하지 않음
        currentState = state; // 상태 업데이트
        animator.SetTrigger(state.ToString()); // 애니메이션 트리거 설정

    }
    public void DoRightAttack(Vector3 targetPosition)
    {
        StartCoroutine(RightAttackRoutine(targetPosition));
    }

    private IEnumerator RightAttackRoutine(Vector3 targetPosition)
    {
        // 목표 방향으로 회전
        Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 런 애니메이션 실행하며 목표 위치로 이동
        PlayAnimationByState(EnemyState.Run);
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
        PlayAnimationByState(EnemyState.RightAttack);
        float attackDuration = 1.15f; // 실제 애니메이션 길이로 조정

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
        PlayAnimationByState(EnemyState.Run);
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
        PlayAnimationByState(EnemyState.Idle);
    }

    private void DoStrongAttack(Vector3 targetPosition)
    {

        StartCoroutine(StrongAttackRooutine(targetPosition));
    }

    private IEnumerator StrongAttackRooutine(Vector3 targetPosition)
    {
        // 목표 방향으로 회전
        Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        // 런 애니메이션 실행하며 목표 위치로 이동
        PlayAnimationByState(EnemyState.Run);
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
        // 스트롱 어택 애니메이션 실행
        PlayAnimationByState(EnemyState.StrongAttack);
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
        PlayAnimationByState(EnemyState.Run);
        elapsed = 0f;
        while (elapsed < runDuration)
        {
            float t = elapsed / runDuration;
            transform.position = Vector3.Lerp(end, savedPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = savedPosition;

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
        PlayAnimationByState(EnemyState.Idle); // Idle 상태로 전환
    }
    private void DoTripleAttack(Vector3 targetPosition)
    {
        StartCoroutine(TripleAttackRoutine(targetPosition));
    }

    private IEnumerator TripleAttackRoutine(Vector3 targetPosition)
    {
        // 목표 방향으로 회전
        Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        // 런 애니메이션 실행하며 목표 위치로 이동
        PlayAnimationByState(EnemyState.Run);
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
        // 트리플 어택 애니메이션 실행
        PlayAnimationByState(EnemyState.TripleAttack);
        float attackDuration = 4.16f; // 실제 애니메이션 길이로 조정
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
        PlayAnimationByState(EnemyState.Run);
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
        PlayAnimationByState(EnemyState.Idle); // Idle 상태로 전환
    }
    public void DoJumpAttack(Vector3 targetPosition)
    {
        StartCoroutine(JumpAttackRoutine(targetPosition));
    }

    private IEnumerator JumpAttackRoutine(Vector3 targetPosition)
    {
        // 목표 방향으로 회전
        Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 점프어택 애니메이션 실행
        PlayAnimationByState(EnemyState.JumpAttack);

        // 점프어택 애니메이션 길이(고정값 사용)
        float jumpAttackDuration = 2f;

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
        PlayAnimationByState(EnemyState.Run);
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
        PlayAnimationByState(EnemyState.Idle);
    }
}


using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
// 에너미 클래스들은 데이터를 다루지 않으니 애니메이션만 처리합니다. 실제 데미지 처리는 BattleManager에서 처리합니다.
public class Gnoll : MonoBehaviour, IEnemy // 테스트에너미 클래스는 모든 에너미 클래스들이 구현해야하는 메서드들의 디폴트를 제공합니다. IEnemy 인터페이스를 상속받고 구체적인 구현을 해주세요
{
    [SerializeField] private Animator animator;
    //[SerializeField] private RectTransform healthBarContainer;
    //[SerializeField] private float healthBarWidth = 100f; // 기본 너비 설정
    public enum EnemyState // 에너미의 상태를 정의하는 열거형
    {
        Idle, // 대기 상태
        RightAttack, // 오른손 공격 상태
        LeftAttack, // 왼손 공격 상태
        SpinAttack, // 회전 공격 상태
        Stun, // 기절 상태
        JumpAttack, // 점프 공격 상태
        Run, // 달리기 상태
        Hit, // 맞은 상태
        Howling, // 울부짖는 상태
        Kick, // 발차기 상태
        Dead, // 사망 상태
        // 추가적인 상태를 여기에 정의할 수 있습니다.
    }
    private EnemyState currentState;
    public bool IsDead => BattleManager.Instance.Enemy.IsDead;

    //public List<Action> PassiveSkills { get; private set; }//현재 기획에선 패시브 스킬을 구현할 필요가 없습니다. 추후에 필요시 구현해주세요.
    public List<Action<Vector3>> ActiveSkills { get; private set; }


    private Vector3 savedPosition;
    private Quaternion savedRotation;


    private void Awake()
    {
        Init();
    }
    private void Start()
    {

        //DoJumpAttack(Vector3.forward * -5 + Vector3.right * -3); // 테스트용 점프 어택, new Vector3(-5, 0, -3)와 같은 의미입니다.
        //DoRightAttack(Vector3.forward * -5 + Vector3.right * -3); // 테스트용 라이트 어택, new Vector3(-5, 0, -3)와 같은 의미입니다.
        //DoLeftAttack(Vector3.forward * -5 + Vector3.right * -3); // 테스트용 레프트 어택
        //DoKickAttack(Vector3.forward * -5 + Vector3.right * -3); // 테스트용 킥 어택
        //DoSpinAttack(); // 테스트용 스핀 어택
        //UseActiveSkill(0, 0); // 테스트용 킥 어택 사용
        //UseActiveSkill(4, 0); // 테스트용 점프 어택 사용
    }
    public void Init()
    {
        ActiveSkills = new List<Action<Vector3>>(new Action<Vector3>[15]);
        ActiveSkills[0] += (pos) => DoKickAttack(pos);
        ActiveSkills[1] += (pos) => DoRightAttack(pos);
        ActiveSkills[4] += (pos) => DoJumpAttack(pos);
    }
    private Vector3 GetTargetPositionByIndex(int index)
    {
        // 플레이어 포메이션 정보를 스테이지 데이터에서 가져옴
        var stageSaveData = StageManager.Instance.stageSaveData;
        var chapterData = StageManager.Instance.chapterData;

        int chapterIdx = stageSaveData.currentChapterIndex;
        int stageIdx = stageSaveData.currentStageIndex;
        int formationIdx = (int)stageSaveData.currentFormationType;

        // 방어적: null 체크 및 인덱스 범위 체크
        if (chapterData == null ||
            chapterData.chapterIndex == null ||
            chapterIdx < 0 || chapterIdx >= chapterData.chapterIndex.Count)
            return new Vector3 (-1,-1,-4);

        var chapter = chapterData.chapterIndex[chapterIdx];
        if (chapter.stageData == null ||
            chapter.stageData.PlayerFormations == null ||
            formationIdx < 0 || formationIdx >= chapter.stageData.PlayerFormations.Count)
            return new Vector3(-1, -1, -4);

        var formation = chapter.stageData.PlayerFormations[formationIdx];
        if (formation.PlayerPositions == null ||
            index < 0 || index >= formation.PlayerPositions.Count)
            return new Vector3(-1, -1, -4);

        // 실제 포지션 반환
        return formation.PlayerPositions[index].Position;
    }
    public void UseActiveSkill(int skillIndex, int targetIndex)
    {
        if (skillIndex < 0 || skillIndex >= ActiveSkills.Count)
        {
            Debug.LogWarning($"Invalid skill index: {skillIndex}");
            return;
        }
        Vector3 targetPos = GetTargetPositionByIndex(targetIndex);
        ActiveSkills[skillIndex]?.Invoke(targetPos);
    }
    public void DoJumpAttack(Vector3 targetPosition)
    {
        StartCoroutine(JumpAttackRoutine(targetPosition));
    }

    private IEnumerator JumpAttackRoutine(Vector3 targetPosition)
    {
        // 현재 위치와 회전 저장
        Vector3 savedPosition = transform.position;
        Quaternion savedRotation = transform.rotation;

        // 목표 방향으로 회전
        Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 점프어택 애니메이션 실행
        PlayAnimationByState(EnemyState.JumpAttack);

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

    public void DoRightAttack(Vector3 targetPosition)
    {
        StartCoroutine(RightAttackRoutine(targetPosition));
    }

    private IEnumerator RightAttackRoutine(Vector3 targetPosition)
    {
        // 현재 위치와 회전 저장
        Vector3 savedPosition = transform.position;
        Quaternion savedRotation = transform.rotation;

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

    public void DoLeftAttack(Vector3 targetPosition)
    {
        StartCoroutine(LeftAttackRoutine(targetPosition));
    }

    private IEnumerator LeftAttackRoutine(Vector3 targetPosition)
    {
        // 현재 위치와 회전 저장
        Vector3 savedPosition = transform.position;
        Quaternion savedRotation = transform.rotation;

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

        // 레프트 어택 애니메이션 실행
        PlayAnimationByState(EnemyState.LeftAttack);
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
    public void DoKickAttack(Vector3 targetPosition)
    {
        StartCoroutine(KickAttackRoutine(targetPosition));
    }

    private IEnumerator KickAttackRoutine(Vector3 targetPosition)
    {
        // Use field variables for saved position and rotation

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

        // 킥 어택 애니메이션 실행
        PlayAnimationByState(EnemyState.Kick);
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
    public void DoSpinAttack()
    {
        StartCoroutine(SpinAttackRoutine());
    }

    private IEnumerator SpinAttackRoutine()
    {
        // 스핀어택 애니메이션 실행
        PlayAnimationByState(EnemyState.SpinAttack);
        float spinAttackDuration = 1f; // 실제 애니메이션 길이로 조정

        yield return new WaitForSeconds(spinAttackDuration);

        // Idle 상태로 전환
        PlayAnimationByState(EnemyState.Idle);
    }
    public void TakeDamage()
    {
        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        PlayAnimationByState(EnemyState.Hit);

        // Hit 애니메이션 길이만큼 대기 (예: 0.7초)
        float hitDuration = 0.7f; // 실제 애니메이션 길이로 조정
        yield return new WaitForSeconds(hitDuration);

        if (IsDead)
            PlayAnimationByState(EnemyState.Dead);
        else
            PlayAnimationByState(EnemyState.Idle);
    }
    public void PlayAnimationByState(EnemyState state)
    {
        if (currentState == state) return; // 중복 상태 방지
        currentState = state; // 상태 변경
        animator.SetTrigger(state.ToString()); // 애니메이션 트리거 설정
    }
}


using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
// 에너미 클래스들은 데이터를 다루지 않으니 애니메이션만 처리합니다. 실제 데미지 처리는 BattleManager에서 처리합니다.
public class Goblin : MonoBehaviour, IEnemy
{
    [SerializeField] private Animator animator; // 애니메이션을 위한 Animator 컴포넌트
    //[SerializeField] private RectTransform healthBarContainer;
    //[SerializeField] private float healthBarWidth = 100f; // 기본 너비 설정

    public bool IsDead => BattleManager.Instance.Enemy.IsDead; // BattleManager에서 에너미의 죽음 상태를 가져옵니다.

    public List<Action<Vector3>> ActiveSkills { get; private set; }

    private Vector3 savedPosition;
    private Quaternion savedRotation;

    public enum EnemyState // 에너미의 상태를 정의하는 열거형
    {
        Idle, // 대기 상태
        RightAttack, // 오른손 공격 상태
        SlashDown, // 내려찍기 공격 상태
        SpinAttack, // 회전 공격 상태
        Stun, // 기절 상태
        Run, // 달리기 상태
        Hit, // 맞은 상태
        Dead, // 사망 상태
        // 추가적인 상태를 여기에 정의할 수 있습니다.
    }
    private EnemyState currentState;
    private void Awake()
    {
        Init();
        
    }

    private void Start() // 테스트용 스킬
    {
        //UseActiveSkill(0, 1); // 예시로 첫 번째 스킬을 사용합니다.
        //UseActiveSkill(1, 1); // 예시로 두 번째 스킬을 사용합니다.
        //UseActiveSkill(2, 1); // 예시로 세 번째 스킬을 사용합니다.
    }

    public void Init()
    {
        ActiveSkills = new List<Action<Vector3>>(new Action<Vector3>[15]);
        ActiveSkills[0] += (pos) => DoRightAttack(pos);
        ActiveSkills[1] += (pos) => DoSlashDown(pos);
        ActiveSkills[2] += (pos) => DoSpinAttack();

        savedPosition = transform.position;
        savedRotation = transform.rotation;
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

        // 실제 포지션 반환
        return formation.PlayerPositions[index].Position;
    }

    public void UseActiveSkill(int skillIndex, int targetIndex)
    {
        if (skillIndex < 0 || skillIndex >= ActiveSkills.Count)
        {
            Debug.LogWarning($"Goblin: Invalid skill index {skillIndex}");
            return;
        }
        Vector3 targetPos = GetTargetPositionByIndex(targetIndex);
        ActiveSkills[skillIndex]?.Invoke(targetPos);
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
            transform.rotation = Quaternion.LookRotation(direction);

        // 이동 애니메이션 및 이동
        PlayAnimationByState(EnemyState.Run);
        float runDuration = 0.8f;
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

        // 공격 애니메이션
        PlayAnimationByState(EnemyState.RightAttack);
        yield return new WaitForSeconds(1f);

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

        // 원래 위치로 복귀
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
        // 복귀 이동 후, 세이브 로테이션으로 회전 복원
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

    public void DoSlashDown(Vector3 targetPosition)
    {
        StartCoroutine(SlashDownRoutine(targetPosition));
    }

    private IEnumerator SlashDownRoutine(Vector3 targetPosition)
    {
        Vector3 direction = (new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(direction);

        PlayAnimationByState(EnemyState.Run);
        float runDuration = 0.8f;
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

        PlayAnimationByState(EnemyState.SlashDown);
        yield return new WaitForSeconds(1.5f);

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
        PlayAnimationByState(EnemyState.Idle);
    }

    public void DoSpinAttack()
    {
        StartCoroutine(SpinAttackRoutine());
    }

    private IEnumerator SpinAttackRoutine()
    {
        PlayAnimationByState(EnemyState.SpinAttack);
        yield return new WaitForSeconds(1f);
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
        if (animator == null)
        {
            Debug.LogError("Goblin: Animator component is not assigned.");
            return;
        }
        currentState = state;
        switch (state)
        {
            case EnemyState.Idle:
                animator.SetTrigger("Idle");
                break;
            case EnemyState.RightAttack:
                animator.SetTrigger("RightAttack");
                break;
            case EnemyState.SlashDown:
                animator.SetTrigger("SlashDown");
                break;
            case EnemyState.SpinAttack:
                animator.SetTrigger("SpinAttack");
                break;
            case EnemyState.Stun:
                animator.SetTrigger("Stun");
                break;
            case EnemyState.Run:
                animator.SetTrigger("Run");
                break;
            case EnemyState.Hit:
                animator.SetTrigger("Hit");
                break;
            case EnemyState.Dead:
                animator.SetTrigger("Dead");
                break;
            default:
                Debug.LogWarning($"Goblin: Unhandled state {state}");
                break;
        }
    }
}
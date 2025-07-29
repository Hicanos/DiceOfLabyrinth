using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Gnoll : MonoBehaviour, IEnemy
{
    [SerializeField] private Animator animator;
    [SerializeField] private Vector3 targetPositionOffset = new(2,0,0); // 타겟 포지션 보정치

    public enum EnemyState
    {
        Idle,
        RightAttack,
        LeftAttack,
        SpinAttack,
        JumpAttack,
        Run,
        Hit,
        Kick,
        Dead,
    }
    private EnemyState currentState;
    public bool IsDead => BattleManager.Instance.Enemy.IsDead;

    public List<Action<Vector3>> ActiveSkills { get; private set; }

    private Vector3 savedPosition;
    private Quaternion savedRotation;

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        // 필요시 테스트용 스킬 호출 위치
        // UseActiveSkill(0, 0); // 킥 어택
        // UseActiveSkill(1, 0); // 오른손 어택
        // UseActiveSkill(4, 0); // 점프 어택
    }
    public void Init()
    {
        ActiveSkills = new List<Action<Vector3>>(new Action<Vector3>[15]);
        ActiveSkills[0] += (pos) => DoKickAttack(pos);
        ActiveSkills[1] += (pos) => DoRightAttack(pos);
        ActiveSkills[2] += (pos) => DoLeftAttack(pos);
        ActiveSkills[4] += (pos) => DoJumpAttack(pos);

        savedPosition = transform.position;
        savedRotation = transform.rotation;
    }
    private Vector3 GetTargetPositionByIndex(int index)
    {
        var stageSaveData = StageManager.Instance?.stageSaveData;
        var chapterData = StageManager.Instance?.chapterData;

        int chapterIdx = stageSaveData.currentChapterIndex;
        int stageIdx = stageSaveData.currentStageIndex;
        int formationIdx = (int)stageSaveData.currentFormationType;

        if (chapterData == null ||
            chapterData.chapterIndex == null ||
            chapterIdx < 0 || chapterIdx >= chapterData.chapterIndex.Count)
            return new Vector3(-1, -1, -4);

        var chapter = chapterData.chapterIndex[chapterIdx];
        if (chapter.stageData == null ||
            chapter.stageData.PlayerFormations == null ||
            formationIdx < 0 || formationIdx >= chapter.stageData.PlayerFormations.Count)
            return new Vector3(-1, -1, -4);

        var formation = chapter.stageData.PlayerFormations[formationIdx];
        if (formation.PlayerPositions == null ||
            index < 0 || index >= formation.PlayerPositions.Count)
            return new Vector3(-1, -1, -4);

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

    // -------------------- 공격 루틴 --------------------

    public void DoJumpAttack(Vector3 targetPosition)
    {
        StartCoroutine(JumpAttackRoutine(targetPosition + targetPositionOffset));
    }
    private IEnumerator JumpAttackRoutine(Vector3 targetPosition)
    {
        yield return RotateTo(targetPosition);

        PlayAnimationByState(EnemyState.JumpAttack);

        yield return MoveToDuringAnimation(targetPosition, 1f);

        yield return RotateTo(savedPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(savedPosition, 0.8f);

        yield return RotateTo(savedRotation);

        PlayAnimationByState(EnemyState.Idle);
    }

    public void DoRightAttack(Vector3 targetPosition)
    {
        StartCoroutine(RightAttackRoutine(targetPosition + targetPositionOffset));
    }
    private IEnumerator RightAttackRoutine(Vector3 targetPosition)
    {
        yield return RotateTo(targetPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(targetPosition, 0.8f);

        PlayAnimationByState(EnemyState.RightAttack);
        yield return WaitAnimation(1f);

        yield return RotateTo(savedPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(savedPosition, 0.8f);

        yield return RotateTo(savedRotation);

        PlayAnimationByState(EnemyState.Idle);
    }

    public void DoLeftAttack(Vector3 targetPosition)
    {
        StartCoroutine(LeftAttackRoutine(targetPosition + targetPositionOffset));
    }
    private IEnumerator LeftAttackRoutine(Vector3 targetPosition)
    {
        yield return RotateTo(targetPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(targetPosition, 0.8f);

        PlayAnimationByState(EnemyState.LeftAttack);
        yield return WaitAnimation(1f);

        yield return RotateTo(savedPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(savedPosition, 0.8f);

        yield return RotateTo(savedRotation);

        PlayAnimationByState(EnemyState.Idle);
    }

    public void DoKickAttack(Vector3 targetPosition)
    {
        StartCoroutine(KickAttackRoutine(targetPosition + targetPositionOffset));
    }
    private IEnumerator KickAttackRoutine(Vector3 targetPosition)
    {
        yield return RotateTo(targetPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(targetPosition, 0.8f);

        PlayAnimationByState(EnemyState.Kick);
        yield return WaitAnimation(1f);

        yield return RotateTo(savedPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(savedPosition, 0.8f);

        yield return RotateTo(savedRotation);

        PlayAnimationByState(EnemyState.Idle);
    }

    public void DoSpinAttack()
    {
        StartCoroutine(SpinAttackRoutine());
    }
    private IEnumerator SpinAttackRoutine()
    {
        PlayAnimationByState(EnemyState.SpinAttack);
        yield return WaitAnimation(1f);
        PlayAnimationByState(EnemyState.Idle);
    }

    // -------------------- 공통 동작 메서드 --------------------

    private IEnumerator RotateTo(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            yield return RotateTo(targetRot, 0.2f);
        }
    }
    private IEnumerator RotateTo(Quaternion targetRotation, float duration = 0.2f)
    {
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
    }
    private IEnumerator MoveTo(Vector3 targetPosition, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(start, targetPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    private IEnumerator MoveToDuringAnimation(Vector3 targetPosition, float duration)
    {
        Vector3 start = transform.position;
        Vector3 end = new Vector3(targetPosition.x, start.y, targetPosition.z);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
    }
    private IEnumerator WaitAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    // -------------------- 기타 --------------------

    public void TakeDamage()
    {
        StartCoroutine(HitRoutine());
    }
    private IEnumerator HitRoutine()
    {
        PlayAnimationByState(EnemyState.Hit);
        float hitDuration = 0.7f;
        yield return new WaitForSeconds(hitDuration);

        if (IsDead)
            PlayAnimationByState(EnemyState.Dead);
        else
            PlayAnimationByState(EnemyState.Idle);
    }
    public void PlayAnimationByState(EnemyState state)
    {
        if (currentState == state) return;
        currentState = state;
        animator.SetTrigger(state.ToString());
    }
    public void AttackSound()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SoundType.SFX_Hit_Sword);
    }
    public void SpinAttackSound()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SoundType.SFX_Swing);
    }
    public void SlashDownSound()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SoundType.SFX_Swing2);
    }
    public void GrowlSound()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SoundType.SFX_Growl);
    }
    public void AttackKick()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SoundType.SFX_Hit_Heavy);
    }

    public void HitPlayer()
    {
        if (BattleManager.Instance == null || BattleManager.Instance.EnemyAttack == null)
        {
            return;
        }
        BattleManager.Instance.EnemyAttack.EnemyAttackDealDamage();
    }
}
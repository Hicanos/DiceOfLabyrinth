using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GnollLeader : MonoBehaviour, IEnemy
{
    [SerializeField] private Animator animator;

    [SerializeField] private Vector3 targetPositionOffset = new Vector3(2, 0, 0); // 타겟 위치 보정치

    private Vector3 savedPosition;
    private Quaternion savedRotation;

    public enum EnemyState
    {
        Idle,
        RightAttack,
        SpinAttack,
        Stun,
        StrongAttack,
        JumpAttack,
        SlashDown,
        TripleAttack,
        Run,
        Hit,
        Howling,
        Dead,
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
        // UseActiveSkill(0, 1); // 오른손 공격
        // UseActiveSkill(1, 1); // 강력한 공격
        // UseActiveSkill(3, 1); // 삼연속 공격
        // UseActiveSkill(4, 1); // 점프 공격
        // UseActiveSkill(5, 1); // 내려찍기 공격
        // UseActiveSkill(14, 1); // 회전 공격
    }
    public void Init()
    {
        ActiveSkills = new List<Action<Vector3>>(new Action<Vector3>[15]);
        ActiveSkills[0] += (pos) => DoRightAttack(pos);
        ActiveSkills[1] += (pos) => DoStrongAttack(pos);
        ActiveSkills[3] += (pos) => DoTripleAttack(pos);
        ActiveSkills[4] += (pos) => DoJumpAttack(pos);
        ActiveSkills[5] += (pos) => DoSlashDown(pos);
        ActiveSkills[14] += (pos) => DoSpinAttack();

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
            Debug.LogError("Invalid skill index: " + skillIndex);
            return;
        }
        Vector3 targetPos = GetTargetPositionByIndex(targetIndex);
        Debug.Log($"Using skill {skillIndex} on target index {targetIndex} at position {targetPos}");
        ActiveSkills[skillIndex]?.Invoke(targetPos);
    }
    public void PlayAnimationByState(EnemyState state)
    {
        if (currentState == state) return;
        currentState = state;
        animator.SetTrigger(state.ToString());
    }

    // -------------------- 공격 루틴 --------------------

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
        yield return WaitAnimation(1.15f);

        yield return RotateTo(savedPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(savedPosition, 0.8f);

        yield return RotateTo(savedRotation);

        PlayAnimationByState(EnemyState.Idle);
    }

    public void DoJumpAttack(Vector3 targetPosition)
    {
        StartCoroutine(JumpAttackRoutine(targetPosition + targetPositionOffset));
    }
    private IEnumerator JumpAttackRoutine(Vector3 targetPosition)
    {
        yield return RotateTo(targetPosition);

        PlayAnimationByState(EnemyState.JumpAttack);

        yield return MoveToDuringAnimation(targetPosition, 2f);

        yield return RotateTo(savedPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(savedPosition, 0.8f);

        yield return RotateTo(savedRotation);

        PlayAnimationByState(EnemyState.Idle);
    }



    private void DoStrongAttack(Vector3 targetPosition)
    {
        StartCoroutine(StrongAttackRoutine(targetPosition + targetPositionOffset));
    }
    private IEnumerator StrongAttackRoutine(Vector3 targetPosition)
    {
        yield return RotateTo(targetPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(targetPosition, 0.8f);

        PlayAnimationByState(EnemyState.StrongAttack);
        yield return WaitAnimation(1f);

        yield return RotateTo(savedPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(savedPosition, 0.8f);

        yield return RotateTo(savedRotation);

        PlayAnimationByState(EnemyState.Idle);
    }

    private void DoTripleAttack(Vector3 targetPosition)
    {
        StartCoroutine(TripleAttackRoutine(targetPosition + targetPositionOffset));
    }
    private IEnumerator TripleAttackRoutine(Vector3 targetPosition)
    {
        yield return RotateTo(targetPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(targetPosition, 0.8f);

        PlayAnimationByState(EnemyState.TripleAttack);
        yield return WaitAnimation(3f);

        yield return RotateTo(savedPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(savedPosition, 0.8f);

        yield return RotateTo(savedRotation);

        PlayAnimationByState(EnemyState.Idle);
    }

    public void DoSlashDown(Vector3 targetPosition)
    {
        StartCoroutine(SlashDownRoutine(targetPosition + targetPositionOffset));
    }
    private IEnumerator SlashDownRoutine(Vector3 targetPosition)
    {
        yield return RotateTo(targetPosition);

        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(targetPosition, 0.8f);

        PlayAnimationByState(EnemyState.SlashDown);
        yield return WaitAnimation(2.24f);

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
        yield return WaitAnimation(2.25f);
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
    private IEnumerator WaitAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
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


    // -------------------- 기타 메서드 --------------------
    public void TakeDamage()
    {
        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        PlayAnimationByState(EnemyState.Hit);
        float hitDuration = 1.5F;
        yield return new WaitForSeconds(hitDuration);

        if (IsDead)
            PlayAnimationByState(EnemyState.Dead);
        else
            PlayAnimationByState(EnemyState.Idle);
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

    public void HitPlayer()
    {
        if (BattleManager.Instance == null || BattleManager.Instance.EnemyAttack == null)
        {
            return;
        }
        BattleManager.Instance.EnemyAttack.EnemyAttackDealDamage();
    }
}
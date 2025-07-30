using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Goblin : MonoBehaviour, IEnemy
{
    [SerializeField] private Animator animator;
    [SerializeField] private Vector3 targetPositionOffset = new(2, 0, 0); // 타겟 포지션 보정치

    public bool IsDead => BattleManager.Instance.Enemy.IsDead;
    public List<Action<Vector3>> ActiveSkills { get; private set; }

    private Vector3 savedPosition;
    private Quaternion savedRotation;

    public enum EnemyState
    {
        Idle,
        RightAttack,
        SlashDown,
        SpinAttack,
        Stun,
        Run,
        Hit,
        Dead,
    }
    private EnemyState currentState;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        // 필요시 테스트용 스킬 호출 위치
        // UseActiveSkill(0, 1);
        // UseActiveSkill(1, 1);
        // UseActiveSkill(2, 1);
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
        var playerPos = BattleManager.Instance?.BattleSpawner?.formationVec;
        int playerFormation = (int)BattleManager.Instance?.BattleGroup?.CurrentFormationType;

        if (playerPos == null || playerFormation < 0 || playerFormation >= playerPos.Count)
            return new Vector3(-1, -1, -4);
        return playerPos[playerFormation].formationVec[index];
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
        yield return WaitAnimation(1f);
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
        yield return WaitAnimation(1.5f);
        yield return RotateTo(savedPosition);
        PlayAnimationByState(EnemyState.Run);
        yield return MoveTo(savedPosition, 0.8f);
        yield return RotateTo(savedRotation);
        PlayAnimationByState(EnemyState.Idle);
    }

    public void DoSpinAttack(Vector3 targetPosition = default)
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
        if (animator == null)
        {
            Debug.LogError("Goblin: Animator component is not assigned.");
            return;
        }
        currentState = state;
        animator.SetTrigger(state.ToString());
    }

    // -------------------- 사운드 메서드 --------------------
    public void AttackSound()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SoundType.SFX_Hit_Mace);
    }
    public void SpinAttackSound()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SoundType.SFX_Swing);
    }
    public void SlashDownSound()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SoundType.SFX_Swing2);
    }
    public void ScreamSound()
    {
               SoundManager.Instance.PlaySFX(SoundManager.SoundType.SFX_Scream);
    }

    // -------------------- 플레이어 히트 메서드 --------------------
    public void HitPlayer()
    {
        if (BattleManager.Instance == null || BattleManager.Instance.EnemyAttack == null)
        {
            return;
        }
        BattleManager.Instance.EnemyAttack.EnemyAttackDealDamage();
    }
}
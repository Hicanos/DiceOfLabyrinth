using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Gnole : MonoBehaviour, IEnemy
{
    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private float healthBarWidth = 100f;

    private bool isDead = false;
    public bool IsDead => isDead;

    public List<Action> ActiveSkills { get; private set; }
    public List<Action> PassiveSkills { get; private set; }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        UpdateHealthBar();
        InitActiveSkills();
        InitPassiveSkills();
    }

    // 15칸짜리 액티브 스킬 리스트, 0: 킥, 1: 라이트 어택, 2: 점프 어택
    private void InitActiveSkills()
    {
        ActiveSkills = new List<Action>(new Action[15]);
        ActiveSkills[0] = () => DoKickAttack(Vector3.zero);        // 예시: 타겟 위치는 실제 상황에 맞게 전달
        ActiveSkills[1] = () => DoLightAttack(Vector3.zero);
        ActiveSkills[2] = () => DoJumpAttack(Vector3.zero);
        // 나머지는 null
    }

    // 15칸짜리 패시브 스킬 리스트, 예시로 0번만 할당
    private void InitPassiveSkills()
    {
        PassiveSkills = new List<Action>(new Action[15]);
        PassiveSkills[0] = DoPassiveRegeneration;
        // 나머지는 null
    }

    // ----------------- 패시브 스킬 예시 -----------------
    private void DoPassiveRegeneration()
    {
        Debug.Log("패시브: 재생 효과 발동");
        // 체력 회복 등 실제 효과 구현
    }

    // ----------------- 액티브 스킬 예시 -----------------
    public void DoKickAttack(Vector3 targetPosition)
    {
        Debug.Log("킥 어택 발동");
        // 실제 공격 로직 및 애니메이션 실행
        StartCoroutine(KickAttackRoutine(targetPosition));
    }

    public void DoLightAttack(Vector3 targetPosition)
    {
        Debug.Log("라이트 어택 발동");
        StartCoroutine(LightAttackRoutine(targetPosition));
    }

    public void DoJumpAttack(Vector3 targetPosition)
    {
        Debug.Log("점프 어택 발동");
        StartCoroutine(JumpAttackRoutine(targetPosition));
    }

    // ----------------- 액티브 스킬 코루틴 예시 -----------------
    private IEnumerator KickAttackRoutine(Vector3 targetPosition)
    {
        // 애니메이션, 이동, 타격 등 실제 구현
        yield return new WaitForSeconds(1f);
        Debug.Log("킥 어택 종료");
    }

    private IEnumerator LightAttackRoutine(Vector3 targetPosition)
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("라이트 어택 종료");
    }

    private IEnumerator JumpAttackRoutine(Vector3 targetPosition)
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("점프 어택 종료");
    }

    // ----------------- 기타 유틸리티 -----------------
    private void UpdateHealthBar()
    {
        // 체력바 UI 갱신
    }

    // 상태 관리용 enum (예시)
    public enum EnemyState
    {
        Idle,
        Patrol,
        Attack,
        Defend,
        Dead,
        // 필요시 추가
    }

    private void SetState(EnemyState state)
    {
        // 상태 전환 로직
    }

    private void UpdateEnemyName()
    {
        // 이름 UI 갱신
    }
}
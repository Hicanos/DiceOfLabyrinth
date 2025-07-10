using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GnollLeader : MonoBehaviour,IEnemy // 테스트에너미 클래스는 모든 에너미 클래스들이 구현해야하는 메서드들의 디폴트를 제공합니다. IEnemy 인터페이스를 상속받고 구체적인 구현을 해주세요
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
    }
    private EnemyState currentState;
    public bool IsDead => BattleManager.Instance.Enemy.IsDead;

    public List<Action<Vector3>> ActiveSkills { get; private set; }

    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        ActiveSkills = new List<Action<Vector3>>(new Action<Vector3>[15]);
        //ActiveSkills[0] += (pos) =>
        //ActiveSkills[1] += (pos) =>
        //ActiveSkills[3] += (pos) =>
        //ActiveSkills[4] += (pos) =>
        //ActiveSkills[5] += (pos) =>
        //ActiveSkills[14] += (pos) =>
    }
    private Vector3 GetTargetPositionByIndex(int index)
    {
        // 예시: 미리 정의된 위치 배열
        Vector3[] positions = {
        new Vector3(0, 0, 0),
        new Vector3(-5, 0, -3),
        new Vector3(-2, 0, 2),
        new Vector3(0, 0, 5),
        new Vector3(5, 0, 0),
        // 필요에 따라 추가, 플레이어의 위치 바로앞을 타겟으로 하도록 수정 예정
        };

        if (index >= 0 && index < positions.Length)
            return positions[index];
        else
            return Vector3.zero; // 기본값 또는 예외 처리
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

    public void PlayAnimationByState(EnemyState state) { }
}


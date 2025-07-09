using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GnollLeader : MonoBehaviour // 테스트에너미 클래스는 모든 에너미 클래스들이 구현해야하는 메서드들의 디폴트를 제공합니다. IEnemy 인터페이스를 상속받고 구체적인 구현을 해주세요
{
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private float healthBarWidth = 100f; // 기본 너비 설정

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
    private bool isDead = false;
    public bool IsDead => isDead;

    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        UpdateHealthBar();
    }
    private void UpdateHealthBar()
    {
        // Implement health bar update logic here
    }

    public void PlayAnimationByState(EnemyState state) { }
}


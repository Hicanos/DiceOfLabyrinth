using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class OrcLord : MonoBehaviour, IEnemy // 테스트에너미 클래스는 모든 에너미 클래스들이 구현해야하는 메서드들의 디폴트를 제공합니다. IEnemy 인터페이스를 상속받고 구체적인 구현을 해주세요
{
    [SerializeField] private int currentHp;

    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private float healthBarWidth = 100f; // 기본 너비 설정

    //public int CurrentHp => //배틀 매니저에서 현재 체력을 가져올 수 있도록 프로퍼티를 정의합니다.

    private bool isDead = false;
    public bool IsDead => isDead;

    private void Awake()
    {
            UpdateHealthBar();
            UpdateEnemyName();
    }
    public void Init()
    {
        SetState(EnemyState.Patrol);
        UpdateHealthBar();
        UpdateEnemyName();
    }

    public enum EnemyState
    {
        Idle,// 대기 상태
        Patrol,// 순찰 상태
        Attack,// 공격 상태
        Defend,// 방어 상태
        Dead,// 사망 상태
        // 추가적인 상태를 여기에 정의할 수 있습니다.
    }

    private void SetState(EnemyState state)
    {
        // Implement state change logic here
    }
    private void UpdateHealthBar()
    {
        // Implement health bar update logic here
    }

    private void UpdateEnemyName()
    {

    }
}


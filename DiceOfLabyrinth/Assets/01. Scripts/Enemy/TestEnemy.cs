using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

[System.Serializable]
public class TestEnemy : MonoBehaviour, IEnemy, IDamagable // 테스트에너미 클래스는 모든 에너미 클래스들이 구현해야하는 메서드들의 디폴트를 제공합니다. IEnemy 인터페이스를 상속받고 구체적인 구현을 해주세요
{
    [field:SerializeField] private EnemyData enemyData;
    [SerializeField] private int currentHp;

    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private float healthBarWidth = 100f; // 기본 너비 설정

    private bool isDead = false;
    public bool IsDead => isDead;
    public EnemyData EnemyData
    {
        get => enemyData;
        set => enemyData = value;
    }

    private void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemyData is not assigned in the inspector!");
        }
        else
        {
            currentHp = enemyData.BaseMaxHp; // 초기 HP 설정
            UpdateHealthBar();
            UpdateEnemyName();
        }
    }
    public void Init()
    {
        currentHp = enemyData.MaxHp; // 초기 HP 설정
        Debug.Log($"Enemy Initialized: {enemyData.EnemyName}, Level: {enemyData.EnemyLevel}");
        isDead = false;
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
        Debug.Log($"Enemy state changed to: {state}");
    }
    private void UpdateHealthBar()
    {
        // Implement health bar update logic here
        Debug.Log($"Updating health bar for {enemyData.EnemyName} with base HP: {enemyData.BaseMaxHp}");
    }

    private void UpdateEnemyName()
    {
        Debug.Log($"Enemy Name: {enemyData.EnemyName}");
    }

    public void TakeDamage(int damage)
    {      
        currentHp = Mathf.Clamp(currentHp - damage, 0, currentHp);
        float hpRatio = (float)currentHp / enemyData.MaxHp;
        
        BattleManager.Instance.UIValueChanger.ChangeEnemyHpRatio(HPEnum.enemy, hpRatio);
        BattleManager.Instance.UIValueChanger.ChangeUIText(BattleTextUIEnum.MonsterHP, $"{currentHp} / {enemyData.MaxHp}");
        if (currentHp == 0)
        {
            isDead = true;
        }
    }

    public void Heal(int amount)
    {
        currentHp += amount;
    }
}


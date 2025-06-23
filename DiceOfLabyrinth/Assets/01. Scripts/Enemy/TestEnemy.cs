using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TestEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private int currentHp;

    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private float healthBarWidth = 100f; // 기본 너비 설정

    public EnemyData EnemyData
    {
        get => enemyData;
        set => enemyData = value;
    }
    public void EnemyInit(EnemyData enemyData)
    {
        this.enemyData = enemyData;
        Debug.Log($"Enemy Initialized: {enemyData.EnemyName}, Level: {enemyData.EnemyLevel}");

        SetState(EnemyState.Idle);
        UpdateHealthBar();
        UpdateEnemyName();
    }

    public enum EnemyState
    {
        Idle,
        Patrol,
        Attack,
        Defend,
        Dead
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
}


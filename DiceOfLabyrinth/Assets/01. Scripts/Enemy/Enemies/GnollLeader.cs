using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GnollLeader : MonoBehaviour, IGnoll // 테스트에너미 클래스는 모든 에너미 클래스들이 구현해야하는 메서드들의 디폴트를 제공합니다. IEnemy 인터페이스를 상속받고 구체적인 구현을 해주세요
{
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private float healthBarWidth = 100f; // 기본 너비 설정

    private IGnoll.EnemyState currentState;
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

    public void PlayAnimationByState(IGnoll.EnemyState state) { }
}


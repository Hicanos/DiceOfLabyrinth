using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Orge : MonoBehaviour, IEnemy // 테스트에너미 클래스는 모든 에너미 클래스들이 구현해야하는 메서드들의 디폴트를 제공합니다. IEnemy 인터페이스를 상속받고 구체적인 구현을 해주세요
{
    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private float healthBarWidth = 100f; // 기본 너비 설정
    public List<Action> PassiveSkills { get; private set; }
    public List<Action> ActiveSkills { get; private set; }

    private bool isDead = false;
    public bool IsDead => isDead;

    private void Awake()
    {
        Init();
    }
    public void Init()
    {

    }
}


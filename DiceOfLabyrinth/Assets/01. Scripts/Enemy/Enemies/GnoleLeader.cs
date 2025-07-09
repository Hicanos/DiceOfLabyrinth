using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GnoleLeader : MonoBehaviour//, IEnemy // 테스트에너미 클래스는 모든 에너미 클래스들이 구현해야하는 메서드들의 디폴트를 제공합니다. IEnemy 인터페이스를 상속받고 구체적인 구현을 해주세요
{
    [SerializeField] private int currentHp;

    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private float healthBarWidth = 100f; // 기본 너비 설정
    [SerializeField] private List<Action> ActiveSkills; // 에너미가 사용할 수 있는 기술 목록을 정의합니다.


    //public int CurrentHp => //배틀 매니저에서 현재 체력을 가져올 수 있도록 프로퍼티를 정의합니다.

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


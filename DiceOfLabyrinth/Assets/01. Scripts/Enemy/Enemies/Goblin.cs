using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Goblin : MonoBehaviour, IEnemy
{
    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private float healthBarWidth = 100f; // 기본 너비 설정

    private bool isDead = false;
    public bool IsDead => isDead;

    public List<Action<Vector3>> ActiveSkills { get; private set; }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        ActiveSkills = new List<Action<Vector3>>(new Action<Vector3>[15]);
        //ActiveSkills[0] += (pos) => 스킬이 만들어지면 할당할 예정
        //ActiveSkills[1] += (pos) =>
        //ActiveSkills[2] += (pos) =>
    }

    public void UseActiveSkill(int skillIndex, int targetIndex)
    {
        // 임시 구현: 실제 스킬 로직은 추후 작성
        if (ActiveSkills == null || skillIndex < 0 || skillIndex >= ActiveSkills.Count || ActiveSkills[skillIndex] == null)
        {
            Debug.LogWarning($"Goblin: Invalid skill index {skillIndex}");
            return;
        }
        // 예시: targetIndex를 좌표로 변환하는 로직 필요시 추가
        Vector3 targetPos = Vector3.zero;
        ActiveSkills[skillIndex].Invoke(targetPos);
    }
}
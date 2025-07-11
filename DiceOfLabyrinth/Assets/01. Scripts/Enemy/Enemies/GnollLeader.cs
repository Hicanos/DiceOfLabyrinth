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
        SpinAttack, // 회전 공격 상태
        Stun, // 기절 상태
        JumpAttack, // 점프 공격 상태
        Run, // 달리기 상태
        Hit, // 맞은 상태
        Howling, // 울부짖는 상태
        Dead, // 사망 상태
    }
    private EnemyState currentState;
    public bool IsDead => BattleManager.Instance.Enemy.IsDead;

    public List<Action<Vector3>> ActiveSkills { get; private set; }

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        // 필요시 테스트용 스킬 호출 위치
        // UseActiveSkill(0, 0);
    }
    public void Init()
    {
        ActiveSkills = new List<Action<Vector3>>(new Action<Vector3>[15]);
        //ActiveSkills[0] += (pos) => DoRightAttack(pos);
        //ActiveSkills[1] += (pos) =>
        //ActiveSkills[3] += (pos) =>
        //ActiveSkills[4] += (pos) =>
        //ActiveSkills[5] += (pos) =>
        //ActiveSkills[14] += (pos) =>
    }
    private Vector3 GetTargetPositionByIndex(int index)
    {
        // 플레이어 포메이션 정보를 스테이지 데이터에서 가져옴
        var stageSaveData = StageManager.Instance?.stageSaveData;
        var chapterData = StageManager.Instance?.chapterData;

        int chapterIdx = stageSaveData.currentChapterIndex;
        int stageIdx = stageSaveData.currentStageIndex;
        int formationIdx = (int)stageSaveData.currentFormationType;

        // 방어적: null 체크 및 인덱스 범위 체크
        if (chapterData == null ||
            chapterData.chapterIndex == null ||
            chapterIdx < 0 || chapterIdx >= chapterData.chapterIndex.Count)
            return Vector3.zero;

        var chapter = chapterData.chapterIndex[chapterIdx];
        if (chapter.stageData == null ||
            chapter.stageData.PlayerFormations == null ||
            formationIdx < 0 || formationIdx >= chapter.stageData.PlayerFormations.Count)
            return Vector3.zero;

        var formation = chapter.stageData.PlayerFormations[formationIdx];
        if (formation.PlayerPositions == null ||
            index < 0 || index >= formation.PlayerPositions.Count)
            return Vector3.zero;

        // 실제 포지션 반환 (index로 접근)
        return formation.PlayerPositions[index].Position;
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


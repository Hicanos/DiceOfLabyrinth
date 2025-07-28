using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIHP : MonoBehaviour
{
    Quaternion enemyHPQuaternion;
    GameObject enemyHPBar;

    [Header("Character HP UIs")]
    public GameObject CharacterHPCanvas;
    public GameObject CharacterHPBack;
    public GameObject CharacterHPFront;
    public GameObject CharacterHPBarrier;
    public GameObject CharacterHPBlank;
    public GameObject CharacterHPText;

    [Header("HP UI Scale Value")]
    [SerializeField] Vector3 CharacterHPVec;
    [SerializeField] Vector3 CharacterPos;
    [SerializeField] Vector3 EnemyHPVec;
    [SerializeField] Vector3 EnemyPos;

    void Update()
    {
        if(BattleManager.Instance.StateMachine.currentState == BattleManager.Instance.I_EnemyTurnState)
        {
            GetEnmeyHPRotation(BattleManager.Instance.Enemy);
            enemyHPBar.transform.rotation = enemyHPQuaternion;
        }

        if(BattleManager.Instance.CharacterAttack.isCharacterAttacking)
        {
            //캐릭터 회전값에 따른 hp회전
        }
    }    

    public void SpawnCharacterHP()
    {
        BattleCharGroup battleGroup = BattleManager.Instance.BattleGroup;
        GameObject go;
        GameObject temp;
        RectTransform rect;
        Transform layoutGroupTransform;

        for (int i = 0; i < 5; i++)
        {
            go = Instantiate(CharacterHPCanvas, battleGroup.CharacterPrefabs[i].transform);
            battleGroup.CharacterHPBars[i] = go;

            go = Instantiate(CharacterHPBack, battleGroup.CharacterHPBars[i].transform);
            rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = CharacterHPVec;
            rect.localPosition = CharacterPos;
            battleGroup.LayoutGroups[i] = go.GetComponentInChildren<HorizontalLayoutGroup>();
            rect = battleGroup.LayoutGroups[i].GetComponent<RectTransform>();
            rect.sizeDelta = CharacterHPVec;
            layoutGroupTransform = battleGroup.LayoutGroups[i].transform;

            battleGroup.CharacterHPs[i] = Instantiate(CharacterHPFront, layoutGroupTransform).GetComponent<RectTransform>();
            battleGroup.CharacterBarriers[i] = Instantiate(CharacterHPBarrier, layoutGroupTransform).GetComponent<RectTransform>();
            battleGroup.CharacterBlank[i] = Instantiate(CharacterHPBlank, layoutGroupTransform).GetComponent<RectTransform>();

            temp = Instantiate(CharacterHPText, go.transform);
            temp.GetComponent<RectTransform>().sizeDelta = CharacterHPVec;
            battleGroup.CharacterHPTexts[i] = temp.GetComponent<TextMeshProUGUI>();

            BattleManager.Instance.UIValueChanger.ChangeCharacterHp((HPEnumCharacter)i);
        }
    }

    public void SpawnMonsterHP()
    {
        BattleManager battleManager = BattleManager.Instance;
        BattleEnemy enemy = battleManager.Enemy;
        Transform layoutGroupTransform;
        RectTransform rect;
        GameObject go;
        GetEnmeyHPRotation(enemy);

        go = Instantiate(CharacterHPCanvas, enemy.EnemyPrefab.transform);
        enemy.EnemyHPBars = go;

        go = Instantiate(CharacterHPBack, enemy.EnemyHPBars.transform);
        enemyHPBar = go;
        rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = EnemyHPVec;
        rect.localPosition = EnemyPos;
        rect.rotation = enemyHPQuaternion;

        enemy.LayoutGroups = go.GetComponentInChildren<HorizontalLayoutGroup>();
        rect = enemy.LayoutGroups.GetComponentInChildren<RectTransform>();
        rect.sizeDelta = EnemyHPVec;
        layoutGroupTransform = enemy.LayoutGroups.transform;

        enemy.EnemyHPs = Instantiate(CharacterHPFront, layoutGroupTransform).GetComponent<RectTransform>();
        enemy.EnemyBarriers = Instantiate(CharacterHPBarrier, layoutGroupTransform).GetComponent<RectTransform>();
        enemy.EnemyBlank = Instantiate(CharacterHPBlank, layoutGroupTransform).GetComponent<RectTransform>();

        go = Instantiate(CharacterHPText, go.transform);
        go.GetComponent<RectTransform>().sizeDelta = EnemyHPVec;
        enemy.EnemyHPTexts = go.GetComponent<TextMeshProUGUI>();

        BattleManager.Instance.UIValueChanger.ChangeEnemyHpUI(HPEnumEnemy.enemy);
    }

    public void GetEnmeyHPRotation(BattleEnemy enemy)
    {
        enemyHPQuaternion = Quaternion.Euler(0, -enemy.Data.EnemySpawnRotation.y, 0);
    }
}

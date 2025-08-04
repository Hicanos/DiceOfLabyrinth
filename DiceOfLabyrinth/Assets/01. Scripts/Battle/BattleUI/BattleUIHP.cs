using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIHP : MonoBehaviour
{
    const int numFIve = 5;

    Quaternion[] playerHPRotation = new Quaternion[numFIve];
    Quaternion enemyHPQuaternion;
    GameObject enemyHPBar;

    [Header("Character HP UIs")]
    public GameObject CharacterHPCanvas;
    public GameObject CharacterHPBack;
    public GameObject CharacterHPFront;
    public GameObject CharacterHPBarrier;
    public GameObject CharacterHPBlank;
    public GameObject CharacterHPText;
    [SerializeField] GameObject patternDisplayer;
    public GameObject EnemyHPCanvas;

    [Header("HP UI Scale Value")]
    [SerializeField] Vector3 CharacterHPVec;
    [SerializeField] Vector3 CharacterPos;
    [SerializeField] Vector3 EnemyHPVec;
    [SerializeField] Vector3 EnemyPos;

    void LateUpdate()
    {
        if(BattleManager.Instance.StateMachine.currentState == BattleManager.Instance.I_EnemyTurnState)
        {
            GetEnmeyHPRotation(BattleManager.Instance.Enemy);
            enemyHPBar.transform.rotation = enemyHPQuaternion;
        }

        //if(BattleManager.Instance.CharacterAttack.isCharacterAttacking)
        //{
        //    //캐릭터 회전값에 따른 hp회전
        //    GetPlayerHPRotation();
        //}
    }    

    public void SpawnCharacterHP()
    {
        BattleCharacterInBattle character;
        GameObject go;
        GameObject temp;
        RectTransform rect;
        Transform layoutGroupTransform;

        for (int i = 0; i < 5; i++)
        {
            character = BattleManager.Instance.PartyData.Characters[i];

            go = Instantiate(CharacterHPCanvas, character.Prefab.transform);
            character.CharacterHPBars = go;

            go = Instantiate(CharacterHPBack, character.CharacterHPBars.transform);
            rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = CharacterHPVec;
            rect.localPosition = CharacterPos;
            character.LayoutGroups = go.GetComponentInChildren<HorizontalLayoutGroup>();
            rect = character.LayoutGroups.GetComponent<RectTransform>();
            rect.sizeDelta = CharacterHPVec;
            layoutGroupTransform = character.LayoutGroups.transform;

            character.CharacterHPs = Instantiate(CharacterHPFront, layoutGroupTransform).GetComponent<RectTransform>();
            character.CharacterBarriers = Instantiate(CharacterHPBarrier, layoutGroupTransform).GetComponent<RectTransform>();
            character.CharacterBlank = Instantiate(CharacterHPBlank, layoutGroupTransform).GetComponent<RectTransform>();

            temp = Instantiate(CharacterHPText, go.transform);
            temp.GetComponent<RectTransform>().sizeDelta = CharacterHPVec;
            character.CharacterHPTexts = temp.GetComponent<TextMeshProUGUI>();

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
        //GameObject pD;
        GetEnmeyHPRotation(enemy);

        go = Instantiate(EnemyHPCanvas, enemy.EnemyPrefab.transform);
        enemy.EnemyHPBars = go;

        //pD = Instantiate(patternDisplayer, enemy.EnemyHPBars.transform);
        go = Instantiate(CharacterHPBack, enemy.EnemyHPBars.transform);
        enemyHPBar = go;
        rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = EnemyHPVec;
        rect.localPosition = EnemyPos;
        rect.rotation = enemyHPQuaternion;

        //rect = pD.GetComponent<RectTransform>();
        //rect.rotation = enemyHPQuaternion;

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

    private void GetEnmeyHPRotation(BattleEnemy enemy)
    {
        enemyHPQuaternion = Quaternion.Euler(0, -enemy.Data.EnemySpawnRotation.y, 0);
    }

    private void GetPlayerHPRotation()
    {
        BattleManager battleManager = BattleManager.Instance;
        BattleCharacterInBattle character;

        float rotationY;

        for(int i = 0; i < numFIve; i++)
        {
            character = BattleManager.Instance.PartyData.Characters[i];
            rotationY = character.Prefab.transform.rotation.y;
            playerHPRotation[i] = Quaternion.Euler(0, -rotationY, 0);
        }
    }
}

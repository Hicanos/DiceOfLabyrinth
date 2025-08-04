using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleCharacterInBattle : IDamagable
{
    public static int index; //전투 종료 후 index를 0으로 초기화 해야함

    private string  charNameKr;
    private string  charNameEn;
    private int     maxHP;
    private int     currentHP;
    private int     currentATK;
    private int     currentDEF;
    private int     currentBarrier;
    private float   currentCritChance;
    private float   currentCritDamage;
    private float   currentPenetration;

    private int     myIndex;
    private int     defaultAttackCount;
    private int     additionalAttackCount;
    private bool    isBarrierOn;
    private bool    isDead;

    public GameObject   Prefab;

    public GameObject       CharacterHPBars;
    public RectTransform    CharacterHPs;
    public RectTransform    CharacterBarriers;
    public RectTransform    CharacterBlank;
    public TextMeshProUGUI  CharacterHPTexts;
    public HorizontalLayoutGroup LayoutGroups;

    private BattlePartyData partyData;
    public BattleCharacter character;
    #region 스텟의 public getter
    public string   CharNameKr => charNameKr;
    public string   CharNameEn => charNameEn;
    public int      MaxHP => maxHP;
    public int      CurrentHP => currentHP;
    public int      CurrentATK => currentATK;
    public int      CurrentDEF => currentDEF;
    public int      CurrentBarrier => currentBarrier;
    public float    CurrentCritChance => currentCritChance;
    public float    CurrentCritDamage => currentCritDamage;
    public float    CurrentPenetration => currentPenetration;
    public int      AttackCount => defaultAttackCount + additionalAttackCount;
    public bool     IsDead => isDead;
    public bool     IsBarrierOn => isBarrierOn;
    #endregion

    public BattleCharacterInBattle(BattleCharacter character, BattlePartyData partyData)
    {
        myIndex = index;
        index++;

        this.partyData = partyData;
        this.character = character;
        defaultAttackCount = 1;

        charNameKr = character.CharNameKr;
        charNameEn = character.CharNameEn;
        maxHP       = character.RegularHP;
        currentHP   = character.CurrentHP;
        currentATK  = character.CurrentATK;
        currentDEF  = character.CurrentDEF;
        currentCritChance   = character.CurrentCritChance;
        currentCritDamage   = character.CurrentCritDamage;
        currentPenetration  = character.CurrentPenetration;

        if (character.IsDied == true)
        {
            isDead = true;
            partyData.CharacterDead(myIndex);
        }
    }

    public void UpdateCharacterData(BattleCharacter character)
    {
        currentHP = character.CurrentHP;
        currentATK = character.CurrentATK;
        currentDEF = character.CurrentDEF;
        currentCritChance = character.CurrentCritChance;
        currentCritDamage = character.CurrentCritDamage;
        currentPenetration = character.CurrentPenetration;

        if (isDead == false && character.IsDied == true)
        {
            Dead();
        }
        else if (isDead == true && character.IsDied == false)
        {
            Revive();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isBarrierOn)
        {
            TakeDamageBarrier(damage);
        }
        else
        {
            TakeDamageHP(damage);
        }
        UpdateHPBar();
    }
    private void TakeDamageHP(int damage)
    {        
        currentHP = Mathf.Clamp(currentHP - damage, 0, currentHP);

        if(currentHP == 0)
        {
            Dead();
        }

        partyData.CharacterHit(myIndex, damage);
    }
    private void TakeDamageBarrier(int damage)
    {
        if(currentBarrier >= damage)
        {
            currentBarrier -= damage;
        }
        else
        {
            isBarrierOn = false;
            currentBarrier = 0;
            TakeDamageHP(damage - currentBarrier);
        }
    }

    public void Heal(int amount)
    {        
        Mathf.Clamp(currentHP + amount, currentHP, maxHP);

        UpdateHPBar();
    }

    public void GetBarrier(int amount)
    {
        if (IsDead) return;

        isBarrierOn = true;
        currentBarrier += amount;

        UpdateHPBar();
    }

    public void Revive()
    {
        isDead = false;

        partyData.CharacterRevive(myIndex);
    }

    private void Dead()
    {
        isDead = true;
        UIManager.Instance.BattleUI.BattleUILog.WriteBattleLog(CharNameKr);
        partyData.CharacterDead(myIndex);
    }

    private void UpdateHPBar()
    {
        LayoutGroups.childControlWidth = false;

        BattleManager.Instance.UIValueChanger.ChangeCharacterHp((HPEnumCharacter)myIndex);
        LayoutGroups.childControlWidth = true;
    }

    public void GetAttackCount(int value)
    {
        additionalAttackCount = value;
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class BattlePartyData
{
    const int numFive = 5;
    BattleManager battleManager;

    private List<BattleCharacter> defaultCharacters;
    private BattleCharacterInBattle[] characters;
    private List<ArtifactData> artifacts;
    private List<EngravingData> engravings;

    public List<BattleCharacter> DefaultCharacters => defaultCharacters;
    public BattleCharacterInBattle[] Characters => characters;
    public List<ArtifactData> Artifacts => artifacts;
    public List<EngravingData> Engravings => engravings;

    public StageSaveData.CurrentFormationType CurrentFormationType;

    public List<int> FrontLine;
    public List<int> BackLine;
    private int frontLineNum;

    public List<int> DeadIndex;

    private bool isAllDead => DeadIndex.Count() == numFive;

    private int currentHitIndex;
    private int currentHitDamage;
    private int currentDeadIndex;
    public int CurrentDeadIndex => currentDeadIndex;

    public BattlePartyData(List<BattleCharacter> characters, List<ArtifactData> artifacts, List<EngravingData> engravings)
    {
        battleManager = BattleManager.Instance;
        BattleCharacterInBattle.index = 0;        

        this.artifacts = artifacts; this.engravings = engravings;

        DeadIndex = new List<int>();
        FrontLine = new List<int>();
        BackLine = new List<int>();

        CurrentFormationType = StageManager.Instance.stageSaveData.currentFormationType;
        frontLineNum = (int)CurrentFormationType;

        for(int i = 0; i < frontLineNum + 1; i++)
        {
            FrontLine.Add(i);
        }
        for(int i = frontLineNum + 1; i < numFive; i++)
        {
            BackLine.Add(i);
        }

        this.characters = new BattleCharacterInBattle[numFive];
        defaultCharacters = characters;

        for (int i = 0; i < numFive; i++)
        {
            this.characters[i] = new BattleCharacterInBattle(characters[i], this);
        }
    }

    public void CharacterHit(int index, int damage)
    {
        currentHitIndex = index;
        currentHitDamage = damage;

        battleManager.ArtifactBuffs.ActionCharacterHit();
    }

    public void CharacterDead(int index)
    {
        currentDeadIndex = index;

        if(DeadIndex.Contains(index))
        {
            UnityEngine.Debug.Log("이미 사망한 아군입니다.");
            return;
        }
        else
        {
            DeadIndex.Add(index);
        }

        battleManager.ArtifactBuffs.ActionCharacterDie();

        if (FrontLine.Contains<int>(index))
        {
            FrontLine.Remove(index);
        }
        else if (BackLine.Contains<int>(index))
        {
            BackLine.Remove(index);
        }

        if (isAllDead)
        {
            battleManager.EndBattle(false);
        }
    }

    public void CharacterRevive(int index)
    {
        if(DeadIndex.Contains(index))
        {
            DeadIndex.Remove(index);
        }
        else
        {
            UnityEngine.Debug.Log("이미 살아있는 아군입니다.");
            return;
        }

        if (index <= frontLineNum)
        {
            FrontLine.Add(index);
            FrontLine.Sort();
            characters[index].Revive();
        }
        else if (index > frontLineNum)
        {
            BackLine.Add(index);
            BackLine.Sort();
            characters[index].Revive();
        }
    }

    public void CharacterGetBarrier(float value)
    {
        float amount = value * currentHitDamage;

        characters[currentHitIndex].GetBarrier((int)amount);
    }

    public List<BattleCharacter> GetEndBattleCharacter()
    {
        for(int i = 0; i < numFive; i++)
        {
            defaultCharacters[i].CurrentHP = characters[i].CurrentHP;
            defaultCharacters[i].IsDied = characters[i].IsDead;
        }

        return defaultCharacters;
    }
}

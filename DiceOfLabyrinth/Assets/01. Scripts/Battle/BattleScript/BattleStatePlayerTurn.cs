public class BattleStatePlayerTurn : IBattleTurnState
{
    const int numFive = 5;
    BattleManager battleManager = BattleManager.Instance;
    UseBuff useBuff = new UseBuff();
    public void Enter()
    {
        UnityEngine.Debug.Log("enter");
        battleManager.BattleTurn++;
        
        battleManager.GetCost(AlivedCharacter());
        
        battleManager.EngravingBuffs.ReduceDuration();
        
      

        // 턴 종료 시 모든 캐릭터의 스킬 쿨타임 감소
        foreach (var character in BattleManager.Instance.PartyData.Characters)
        {
            if (character.character.UsingSkill && character.character.GetBonusAttack)
            {
                // 스킬 사용 중이며 추가타가 있는 캐릭터인 경우, 추가타 유지
                character.character.GetBonusAttack = true;
                
            }
            else
            {
                // 스킬 사용 중이 아니거나 추가타가 없는 캐릭터인 경우, 스킬 사용 상태 초기화
                character.character.GetBonusAttack = false;
            }
            character.character.UsingSkill = false;
            character.character.ReduceSkillCooldown(); // 스킬 사용 상태 초기화
        }

        UIManager.Instance.BattleUI.BattleUILog.WriteBattleLog(true);
        battleManager.CostSpendedInTurn = 0;
        ChangeDetailedTurnState(DetailedTurnState.Enter);
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {        
        DiceManager.Instance.DiceRankBefore = DiceManager.Instance.DiceRank;
    }

    public void ChangeDetailedTurnState(DetailedTurnState state)
    {
        battleManager.BattleTutorial.StartTutorial((int)state);

        battleManager.CurrentDetailedState = state;
        battleManager.ArtifactBuffs.Action();
        useBuff.UseBuffs(state);
        OnOffButton();
    }

    public void EndPlayerTurn()
    {
        battleManager.StateMachine.currentState = battleManager.I_EnemyTurnState;
    }

    private void OnOffButton()
    {
        foreach (AbstractBattleButton button in UIManager.Instance.BattleUI.Buttons)
        {
            button.OnOffButton(battleManager.CurrentDetailedState);
        }
    }

    private int AlivedCharacter()
    {
        int num = numFive - battleManager.PartyData.DeadIndex.Count;

        return num;
    }
}

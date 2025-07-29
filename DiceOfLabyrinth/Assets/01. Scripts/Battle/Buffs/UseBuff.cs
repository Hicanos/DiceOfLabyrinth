public class UseBuff
{
    public void UseBuffs(DetailedTurnState state)
    {
        BattleManager battleManager = BattleManager.Instance;
        switch (state)
        {
            case DetailedTurnState.Enter:
                battleManager.ArtifactBuffs.ActionTurnEnter();
                battleManager.EngravingBuffs.ActionTurnEnter();
                break;
            case DetailedTurnState.Attack:
                battleManager.ArtifactBuffs.ActionCharacterAttack();
                battleManager.EngravingBuffs.ActionCharacterAttack();
                break;
            case DetailedTurnState.EndTurn:
                battleManager.EngravingBuffs.ActionTurnEnd();
                break;
        }
    }
}

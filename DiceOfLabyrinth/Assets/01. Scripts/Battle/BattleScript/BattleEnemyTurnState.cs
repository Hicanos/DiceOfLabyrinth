public class BattleEnemyTurnState : IBattleTurnState
{
    public void Enter()
    {
        Attack();        
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {
        BattleManager.Instance.EnemyAttack.EnemyAttackEnd();
        DiceManager.Instance.ResetSetting();
    }

    private void Attack()
    {
        BattleManager battleManager = BattleManager.Instance;

        battleManager.EnemyAttack.EnemyAttack();
    }
}

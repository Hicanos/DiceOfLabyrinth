public class BattleStateEnemyTurn : IBattleTurnState
{
    public void Enter()
    {
        UIManager.Instance.BattleUI.BattleUILog.WriteBattleLog(false);
        Attack();        
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {
        BattleManager.Instance.Enemy.PassiveContainer.ActionPassiveEnemyAttack();
        BattleManager.Instance.EnemyAttack.EnemyAttackEnd();
        DiceManager.Instance.ResetSetting();
    }

    private void Attack()
    {
        BattleManager battleManager = BattleManager.Instance;

        battleManager.EnemyAttack.EnemyAttack();
    }
}

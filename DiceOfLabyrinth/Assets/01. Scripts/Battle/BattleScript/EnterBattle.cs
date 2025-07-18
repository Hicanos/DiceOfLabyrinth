public class EnterBattle
{
    public void BattleStart() //전투 시작시 호출해야할 메서드
    {
        BattleManager battleManager = BattleManager.Instance;
        if (UIManager.Instance.BattleUI.battleCanvas.worldCamera == null)
        {
            UIManager.Instance.BattleUI.battleCanvas.worldCamera = DiceManager.Instance.DiceCamera;
        }        
        DiceManager.Instance.LoadDiceData();

        battleManager.BattleSpawner.SpawnCharacters();
        battleManager.BattleSpawner.SpawnEnemy();

        DiceManager.Instance.DiceSettingForBattle();
    }
}

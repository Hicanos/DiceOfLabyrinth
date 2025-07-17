public class EnterBattle
{
    public void BattleStart() //전투 시작시 호출해야할 메서드
    {
        BattleManager battleManager = BattleManager.Instance;
        if (UIManager.Instance.BattleUI.battleCanvas.worldCamera == null)
        {
            UIManager.Instance.BattleUI.battleCanvas.worldCamera = DiceManager.Instance.diceCamera;
        }
        DiceManager.Instance.DiceSettingForBattle();
        DiceManager.Instance.LoadDiceData();

        battleManager.battleSpawner.SpawnCharacters();
        battleManager.battleSpawner.SpawnEnemy();
    }
}

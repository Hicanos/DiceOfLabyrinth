using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleButtonRollDice : AbstractBattleButton
{
    [SerializeField] Button rollButton;
    [SerializeField] TextMeshProUGUI text;
    bool isRollOver = false;

    public override void Setting()
    {

    }

    public override void OnOffButton(DetailedTurnState state)
    {
        switch (state)
        {
            case DetailedTurnState.Enter:
                ChangeEndTurnToRoll();
                ChangeRollUI();
                rollButton.interactable = true;
                break;
            case DetailedTurnState.Roll:
                rollButton.interactable = false;
                UIManager.Instance.BattleUI.TempButton(false);
                break;
            case DetailedTurnState.RollEnd:
                if (DiceManager.Instance.RollRemain == 0)
                {
                    rollButton.interactable = false;
                }
                else
                {
                    rollButton.interactable = true;
                }
                break;
            case DetailedTurnState.Attack:
                rollButton.interactable = false;
                UIManager.Instance.BattleUI.TempButton(true);
                break;
            case DetailedTurnState.AttackEnd:
                rollButton.interactable = true;
                ChangeRollToEndTurn();
                break;
        }
    }

    public override void OnPush()
    {
        BattleManager battleManager = BattleManager.Instance;
        DiceManager diceManager = DiceManager.Instance;

        diceManager.DiceHolding.isCantFix = true;
        if (isRollOver == false)
        {
            diceManager.RollDice();

            // 캐릭터 PrepareAttack() 실행
            var battleCharacters = BattleManager.Instance.PartyData.Characters;
            for (int i = 0; i < battleCharacters.Length; i++)
            {
                if (battleCharacters[i].IsDead) continue;
                if (battleManager.PartyData.DeadIndex.Contains(i)) continue;

                var characterPrefab = battleCharacters[i].Prefab;
                characterPrefab.GetComponent<SpawnedCharacter>().PrepareAttack();
            }

            ChangeRollUI();
            diceManager.DiceHolding.GetFixedList();
            
            battleManager.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.Roll);
        }
        else
        {
            rollButton.interactable = false;
            battleManager.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.EndTurn);
            battleManager.EndPlayerTurn();
        }
    }

    private void ChangeRollUI()
    {
        string st = $"({DiceManager.Instance.RollRemain.ToString()})";
        BattleManager.Instance.UIValueChanger.ChangeUIText(BattleTextUIEnum.Reroll, st);
    }

    private void ChangeRollToEndTurn()
    {
        isRollOver = true;
        text.text = "턴 종료";
    }

    private void ChangeEndTurnToRoll()
    {
        isRollOver = false;
        text.text = "굴리기";
    }
}

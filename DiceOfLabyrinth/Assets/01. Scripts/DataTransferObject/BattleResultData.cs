using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class BattleResultData // Data Transfer Object (DTO) for battle result data
{
    public bool isVictory;
    public List<BattleCharacter> battleCharacters;

    public BattleResultData(bool isVictory, List<BattleCharacter> battleCharacters)
    {
        this.isVictory = isVictory;
        this.battleCharacters = battleCharacters;
    }
}

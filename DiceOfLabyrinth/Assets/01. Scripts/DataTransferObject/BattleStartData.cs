using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleStartData //Data Transfer Object (DTO) for battle start data
{
    public List<BattleCharacter> battleCharacters;
    public EnemyData selectedEnemy;

    public BattleStartData(List<BattleCharacter> battleCharacters, EnemyData selectedEnemy)
    {
        this.battleCharacters = battleCharacters;
        this.selectedEnemy = selectedEnemy;
    }
}

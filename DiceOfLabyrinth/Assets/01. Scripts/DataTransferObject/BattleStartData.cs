using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleStartData //Data Transfer Object (DTO) for battle start data
{
    public List<BattleCharacter> battleCharacters;
    public EnemyData selectedEnemy;
    public List<ArtifactData> artifacts;
    public List<EngravingData> engravings;
    public StageSaveData.CurrentFormationType currentFormationType;
    public int manaStone;

    public BattleStartData(StageSaveData stageSaveData, int manaStone)
    {
        this.battleCharacters = new List<BattleCharacter>(stageSaveData.battleCharacters);
        this.selectedEnemy = stageSaveData.selectedEnemy;
        this.artifacts = new List<ArtifactData>(stageSaveData.artifacts);
        this.engravings = new List<EngravingData>(stageSaveData.engravings);
        this.currentFormationType = stageSaveData.currentFormationType;
        this.manaStone = manaStone;
    }
}

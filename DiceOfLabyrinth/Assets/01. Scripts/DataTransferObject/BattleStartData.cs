using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleStartData //Data Transfer Object (DTO) for battle start data
{
    public List<BattleCharacter> battleCharacters;
    public EnemyData selectedEnemy;
    public List<ArtifactData> artifacts;
    public List<StagmaData> stagmas;

    public BattleStartData(StageSaveData stageSaveData)
    {
        this.battleCharacters = new List<BattleCharacter>(stageSaveData.battleCharacters);
        this.selectedEnemy = stageSaveData.selectedEnemy;
        this.artifacts = new List<ArtifactData>(stageSaveData.artifacts);
        this.stagmas = new List<StagmaData>(stageSaveData.stagmas);
    }
}

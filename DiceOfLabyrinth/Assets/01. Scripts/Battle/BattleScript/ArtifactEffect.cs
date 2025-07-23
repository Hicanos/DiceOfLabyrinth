using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ArtifactEffect
{
    public Dictionary<ArtifactEffectData, Action<ArtifactEffectData>> ArtifactEffectAttack = new Dictionary<ArtifactEffectData, Action<ArtifactEffectData>>();
    public Dictionary<ArtifactEffectData, Action<ArtifactEffectData>> ArtifactEffectFirstTurn = new Dictionary<ArtifactEffectData, Action<ArtifactEffectData>>();
    public Dictionary<ArtifactEffectData, Action<ArtifactEffectData>> ArtifactEffectPerTurn = new Dictionary<ArtifactEffectData, Action<ArtifactEffectData>>();
    public Dictionary<ArtifactEffectData, Action> ArtifactEffectUpdate = new Dictionary<ArtifactEffectData, Action>();    
    ArtifactEffectData.EffectType[][] effectTypes;
    List<ArtifactEffectData> effectDataAttack = new List<ArtifactEffectData>();
    List<ArtifactEffectData> effectDataFirstTurn = new List<ArtifactEffectData>();
    List<ArtifactEffectData> effectDataUpdate = new List<ArtifactEffectData>();
    List<ArtifactEffectData> effectDataPerTurn = new List<ArtifactEffectData>();

    private bool haveArtifact;
    public ArtifactEffect(List<ArtifactData> artifactDatas)
    {
        if (artifactDatas == null) return;
        
        effectTypes = new ArtifactEffectData.EffectType[artifactDatas.Count][];

        for(int i = 0; i < artifactDatas.Count; i++)
        {
            if (artifactDatas[i] == null) continue;
            for (int j = 0; j < artifactDatas[i].ArtifactEffects.Count; j++)
            {
                effectTypes[i][j] = artifactDatas[i].ArtifactEffects[j].Type;

                switch (artifactDatas[i].ArtifactEffects[j].Type)
                {
                    case ArtifactEffectData.EffectType.AdditionalElementDamage: //속성 추가 피해
                        effectDataFirstTurn.Add(artifactDatas[i].ArtifactEffects[j]);
                        ArtifactEffectFirstTurn.Add(artifactDatas[i].ArtifactEffects[j], AdditionalElementDamage);                        
                        break;
                    case ArtifactEffectData.EffectType.AdditionalDamage: // 추가 피해
                        effectDataFirstTurn.Add(artifactDatas[i].ArtifactEffects[j]);
                        ArtifactEffectFirstTurn.Add(artifactDatas[i].ArtifactEffects[j], AdditionalDamage);
                        break;
                    case ArtifactEffectData.EffectType.AdditionalDamageToBoss: // 보스에게 추가 피해
                        effectDataFirstTurn.Add(artifactDatas[i].ArtifactEffects[j]);
                        ArtifactEffectFirstTurn.Add(artifactDatas[i].ArtifactEffects[j], AdditionalBossDamage);
                        break;
                    case ArtifactEffectData.EffectType.HealingWhenStartBattle: // 전투 시작 시 회복
                        effectDataFirstTurn.Add(artifactDatas[i].ArtifactEffects[j]);
                        ArtifactEffectFirstTurn.Add(artifactDatas[i].ArtifactEffects[j], HealWhenRoomEnter);
                        break;
                    case ArtifactEffectData.EffectType.DebuffToEnemyAtFirstTurn: // 첫 턴 적에게 디버프
                        effectDataFirstTurn.Add(artifactDatas[i].ArtifactEffects[j]);
                        ArtifactEffectFirstTurn.Add(artifactDatas[i].ArtifactEffects[j], DebuffToEnemy);
                        break;

                    case ArtifactEffectData.EffectType.AdditionalDamageIfHaveSignitureDice: // 시그니처 주사위가 있을 때 추가 피해
                        effectDataAttack.Add(artifactDatas[i].ArtifactEffects[j]);
                        ArtifactEffectAttack.Add(artifactDatas[i].ArtifactEffects[j], AdditionalDamageWhenHaveSigniture);
                        break;

                    case ArtifactEffectData.EffectType.RemoveDebuffPerTurn: // 턴당 디버프 제거
                        effectDataPerTurn.Add(artifactDatas[i].ArtifactEffects[j]);
                        ArtifactEffectPerTurn.Add(artifactDatas[i].ArtifactEffects[j], DebuffToEnemy);
                        break;
                    case ArtifactEffectData.EffectType.CostRegenerationEveryTurn: // 턴마다 비용 재생
                        effectDataPerTurn.Add(artifactDatas[i].ArtifactEffects[j]);
                        ArtifactEffectPerTurn.Add(artifactDatas[i].ArtifactEffects[j], DebuffToEnemy);
                        break;

                    case ArtifactEffectData.EffectType.ReviveWhenDie:
                        isReviveArtifactActive = true;
                        effectDataUpdate.Add(artifactDatas[i].ArtifactEffects[j]);
                        ArtifactEffectUpdate.Add(artifactDatas[i].ArtifactEffects[j], ArtifactEffectRevive);
                        break;
                    case ArtifactEffectData.EffectType.GenerateBarrier:
                        isBarrierArtifactActive = true;
                        effectDataUpdate.Add(artifactDatas[i].ArtifactEffects[j]);
                        ArtifactEffectUpdate.Add(artifactDatas[i].ArtifactEffects[j], ArtifactEffectGenerateBarrier);
                        break;
                }
                haveArtifact = true;
            }
        }
    }

    public void EffectWhenCharacterAttack()
    {
        if (!haveArtifact) return;

        for (int i = 0; i < effectDataAttack.Count; i++)
        {
            ArtifactEffectAttack[effectDataAttack[i]](effectDataAttack[i]);
        }
    }

    #region Methods used in EffectWhenCharacterAttack
    private void AdditionalDamageWhenHaveSigniture(ArtifactEffectData data)
    {
        if(DiceManager.Instance.SignitureAmount > 0)
        {
            BattleManager.Instance.ArtifactAdditionalValue.AdditionalDamage += data.Value;
        }
    }
    #endregion

    public void EffectWhenFirstTurn()
    {
        if (!haveArtifact) return;

        for (int i = 0; i < effectDataFirstTurn.Count; i++)
        {
            ArtifactEffectFirstTurn[effectDataFirstTurn[i]](effectDataFirstTurn[i]);
        }
    }

    #region Mehtods used in EffectWhenFirstTurn 
    private void AdditionalElementDamage(ArtifactEffectData data)
    {
        BattleManager.Instance.ArtifactAdditionalValue.AdditionalElementDamage += data.Value;
    }

    private void AdditionalDamage(ArtifactEffectData data)
    {
        BattleManager.Instance.ArtifactAdditionalValue.AdditionalDamage += data.Value;
    }

    private void AdditionalBossDamage(ArtifactEffectData data)
    {
        BattleManager.Instance.ArtifactAdditionalValue.GetAdditionalDamageToBoss += data.Value;
    }

    private void HealWhenRoomEnter(ArtifactEffectData data)
    {
        List<BattleCharacter> characters = BattleManager.Instance.BattleGroup.BattleCharacters;
        float healAmount;
        for(int i = 0; i < characters.Count; i++)
        {
            if (characters[i].IsDied) continue;

            healAmount = characters[i].RegularHP * data.Value;
            characters[i].Heal((int)healAmount);
        }        
    }

    private void DebuffToEnemy(ArtifactEffectData data)
    {

    }
    #endregion

    public void EffectPerTurn()
    {
        if (!haveArtifact) return;

        for (int i = 0; i < effectDataPerTurn.Count; i++)
        {
            ArtifactEffectPerTurn[effectDataPerTurn[i]](effectDataPerTurn[i]);
        }
    }

    

    public void EffectWhenUpdate()
    {
        if (!haveArtifact) return;

        for (int i = 0; i < effectDataUpdate.Count; i++)
        {
            ArtifactEffectUpdate[effectDataUpdate[i]]();
        }
    }

    #region Methods used in EffectWhenUpdate
    public bool isReviveArtifactActive;
    public bool isReviveArtifactUsed;
    private void ArtifactEffectRevive()
    {
        if (!isReviveArtifactActive) return;
        if (isReviveArtifactUsed) return;
        if (BattleManager.Instance.StateMachine.currentState != BattleManager.Instance.I_EnemyTurnState) return;

        if(BattleManager.Instance.BattleGroup.DeadCount > 0)
        {
            BattleManager.Instance.BattleGroup.CharacterRevive(BattleManager.Instance.BattleGroup.DeadIndex.Last());
            Debug.Log("Revive Artifact Active");
            isReviveArtifactUsed = true;
        }
    }

    public bool isBarrierArtifactActive;
    public bool isBarrierActive;
    public bool isCharacterHit;
    private void ArtifactEffectGenerateBarrier()
    {
        if (!isBarrierArtifactActive) return;
        if (BattleManager.Instance.StateMachine.currentState != BattleManager.Instance.I_EnemyTurnState) return;
        if (isBarrierActive) return;

        if(isCharacterHit)
        {
            GenerateBarrier();
        }

        isBarrierActive = true;
    }

    private void GenerateBarrier()
    {
        Debug.Log("Generate Barrier Artifact Active");
    }
    #endregion
}

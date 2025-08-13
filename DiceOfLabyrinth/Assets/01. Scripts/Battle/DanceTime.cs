using UnityEngine;

public class DanceTime
{    
    static public void Dance(bool isWon)
    {
        BattleCharacterInBattle[] characters = BattleManager.Instance.PartyData.Characters;
        GameObject go;

        for(int i = 0; i < characters.Length; i++)
        {
            go = characters[i].Prefab;
            BattleManager.Instance.BattleSpawner.DeactiveCharacterHP(characters);
            go.transform.localPosition = BattleManager.Instance.BattleSpawner.DancePosition[i];
            characters[i].CharRotationObject.transform.rotation = Quaternion.Euler(BattleManager.Instance.BattleSpawner.DanceRotation[i]);
        }

        for (int i = 0; i < characters.Length; i++)
        {           
            if(isWon)
            {
                characters[i].SpawnedCharacter.Victory();
            }
            else
            {
                characters[i].SpawnedCharacter.Defeat();
            }
        }
    }

    static public void StopDance()
    {
        BattleManager.Instance.BattleSpawner.CharacterDeActive();
    }
}

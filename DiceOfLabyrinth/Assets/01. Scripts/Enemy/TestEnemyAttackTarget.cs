using UnityEngine;

public class TestEnemyAttackTarget
{
    
    public BattleCharacter SingleTarget(float front = 80, float back = 20) //아니면 인덱스를 반환하는 것으로 해도 될듯
    {
        BattleCharacter[] frontCharacters = new BattleCharacter[2]; //임시, 엔트리에 있는 캐릭터 배열 받아오도록 수정
        BattleCharacter[] backtCharacters = new BattleCharacter[3]; //임시, 엔트리에 있는 캐릭터 배열 받아오도록 수정
        int iNum;

        if (Random.Range(1, 101) <= front)
        {
            iNum = Random.Range(0,frontCharacters.Length);
            return frontCharacters[iNum];
        }
        else
        {
            iNum = Random.Range(0, backtCharacters.Length);
            return backtCharacters[iNum];
        }
    }
}

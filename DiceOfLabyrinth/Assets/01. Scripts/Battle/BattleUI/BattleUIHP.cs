using UnityEngine;

public class BattleUIHP : MonoBehaviour
{
    Quaternion enemyHPQuaternion;
    GameObject enemyHPBar;

    void Start()
    {
        
    }
    
    void Update()
    {
        if(BattleManager.Instance.EnemyAttack.isEnemyAttacking)
        {
            enemyHPBar.transform.rotation = enemyHPQuaternion;
        }

        if(BattleManager.Instance.CharacterAttack.isCharacterAttacking)
        {
            //캐릭터 회전값에 따른 hp회전
        }
    }

    public void GetEnmeyHPRotation(BattleEnemy enemy)
    {
        enemyHPBar = enemy.EnemyHPBar;
        enemyHPQuaternion = Quaternion.Euler(0, -enemy.Data.EnemySpawnRotation.y, 0);
    }
}

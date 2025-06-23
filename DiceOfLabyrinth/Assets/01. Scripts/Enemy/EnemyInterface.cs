using UnityEngine;

public interface IEnemy
{
    public EnemyData EnemyData { get; set; }

    public void EnemyInit(EnemyData enemyData);
}

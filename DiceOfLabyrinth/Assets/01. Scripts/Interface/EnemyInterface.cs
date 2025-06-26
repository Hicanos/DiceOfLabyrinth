using UnityEngine;

public interface IEnemy //모든 에너미 클래스는 이 인터페이스를 구현해야 합니다.
{
    public EnemyData EnemyData { get; set; }

    public void Init();
}


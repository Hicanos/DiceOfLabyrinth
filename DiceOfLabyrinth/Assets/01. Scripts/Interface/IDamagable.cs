using UnityEngine;

/// <summary>
/// 대미지를 받을 수 있는 객체를 정의하는 인터페이스
/// </summary>
public interface IDamagable
{
    void TakeDamage(int damage);
    void Heal(int amount);

}

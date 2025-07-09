using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;

public interface IEnemy //모든 에너미 클래스는 이 인터페이스를 구현해야 합니다.
{
<<<<<<< Updated upstream
    public void Init();
}

=======
    
    void Init(); // 초기화 메서드
    bool IsDead { get; } // 에너미가 죽었는지 여부를 반환하는 프로퍼티
    List<Action> ActiveSkills { get; } // 에너미가 사용할 수 있는 기술 목록을 반환하는 프로퍼티
    //List<Action> PassiveSkills { get; } // 에너미가 보유한 패시브 기술 목록을 반환하는 프로퍼티

}
public interface IGnoll : IEnemy // 이 인터페이스는 Gnoll 
{
    public enum EnemyState // 에너미의 상태를 정의하는 열거형
    {
        Idle, // 대기 상태
        RightAttack, // 오른손 공격 상태
        LeftAttack, // 왼손 공격 상태
        SpinAttack, // 회전 공격 상태
        Stun, // 기절 상태
        JumpAttack, // 점프 공격 상태
        Run, // 달리기 상태
        Hit, // 맞은 상태
        Howling, // 울부짖는 상태
        Kick, // 발차기 상태
        Dead, // 사망 상태
        // 추가적인 상태를 여기에 정의할 수 있습니다.
    }
    void PlayAnimationByState(EnemyState state); // 상태에 따른 애니메이션 재생 메서드
}
>>>>>>> Stashed changes

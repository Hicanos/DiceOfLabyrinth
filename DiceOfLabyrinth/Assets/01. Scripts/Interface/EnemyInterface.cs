using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;

public interface IEnemy //모든 에너미 클래스는 이 인터페이스를 구현해야 합니다.
{
    
    void Init(); // 초기화 메서드
    bool IsDead { get; } // 에너미가 죽었는지 여부를 반환하는 프로퍼티
    List<Action<Vector3>> ActiveSkills { get; } // 에너미가 사용할 수 있는 기술 목록을 반환하는 프로퍼티
    //List<Action<Vector3>> PassiveSkills { get; } // 에너미가 보유한 패시브 기술 목록을 반환하는 프로퍼티, 현재 기획에선 에너미 패시브는 데이터에서만 처리
    public void UseActiveSkill(int skillIndex, int targetIndex);
}

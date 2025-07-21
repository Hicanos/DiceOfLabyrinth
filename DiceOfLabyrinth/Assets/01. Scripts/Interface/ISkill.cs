/// <summary>
/// 스킬의 기능을 정의하는 인터페이스
/// </summary>
public interface ISkill
{
    void StartSkill(); // 스킬 시작 메소드 - 해당 스킬을 시작할 때(사용 시) 호출

    void SkillEffect(); // 스킬 효과 메소드 - 각 스킬의 효과를 적용하는 내용

    void EndSkill(); // 스킬 종료 메소드 - 스킬의 효과가 끝날 때 호출

}

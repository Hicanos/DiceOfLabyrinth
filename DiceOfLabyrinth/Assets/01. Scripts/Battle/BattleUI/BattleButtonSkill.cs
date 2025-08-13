using UnityEngine;
using UnityEngine.UI;

public class BattleButtonSkill : AbstractBattleButton
{
    BattleCharacterInBattle character;
    Button button;
    [SerializeField] Image skillImage;
    [SerializeField] Image charImage;
    [SerializeField] int index;
    GameObject characterPrefab;


    public override void Setting()
    {
        button = GetComponent<Button>();
        character = BattleManager.Instance.PartyData.Characters[index];
        characterPrefab = BattleManager.Instance.PartyData.Characters[index].Prefab;
        SpriteSetting();
    }

    private void SpriteSetting()
    {
        if (character != null && character.character.CharacterData != null && character.character.CharacterData.icon != null)
        {
            skillImage.sprite = character.character.CharacterData.activeSO.SkillIcon;
            charImage.sprite = character.character.CharacterData.icon;
        }
    }

    public override void OnOffButton(DetailedTurnState state)
    {
        switch (state)
        {
            case DetailedTurnState.Enter:
                button.interactable = true;
                break;
            case DetailedTurnState.Roll:
                button.interactable = false;
                break;
            case DetailedTurnState.Attack:
                button.interactable = false;
                break;
            case DetailedTurnState.AttackEnd:
                button.interactable = true;
                break;
            case DetailedTurnState.EndTurn:
                button.interactable = false;
                break;
        }
    }

    public override void OnPush()
    {
        if (!character.character.CanUseSkill)
        {
            Debug.Log(character.CharNameKr + "의 스킬 쿨타임이 남아있어 사용할 수 없습니다. (남은 쿨타임: " + character.character.CurrentSkillCooldown + ")");
            return;
        }
        if (character.character.UsingSkill)
        {
            Debug.Log(character.CharNameKr+"는 이미 스킬을 사용 중입니다.");
            return;
        }

        character.character.UsingSkill = true;
        Debug.Log(character.CharNameKr + " 스킬 사용");
        characterPrefab.GetComponent<SpawnedCharacter>().PrepareAttack();
        character.character.UseActiveSkill(BattleManager.Instance.PartyData.DefaultCharacters, BattleManager.Instance.Enemy);        
    }

    //private void GetIndex()
    //{
    //    index = gameObject.transform.GetSiblingIndex();
    //}
}

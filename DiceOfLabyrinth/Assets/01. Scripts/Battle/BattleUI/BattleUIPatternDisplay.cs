using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BattleUIPatternDisplay : AbstractBattleButton
{
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] Image image_PatternName;
    [SerializeField] TextMeshProUGUI text_SkillDescription;
    [SerializeField] Button button;

    public override void Setting()
    {

    }

    public override void OnOffButton(DetailedTurnState state)
    {
        switch (state)
        {
            case DetailedTurnState.Enter:
                descriptionPanel.SetActive(false);
                StartCoroutine(BlinkUI());
                break;
            case DetailedTurnState.Roll:
                button.interactable = false;
                break;
            case DetailedTurnState.Attack:
                button.interactable = true;
                break;
        }
    }

    public override void OnPush()
    {        
        descriptionPanel.gameObject.SetActive(true);
    }

    IEnumerator BlinkUI()
    {
        BattleManager battleManager = BattleManager.Instance;
        button.interactable = false;
        Color color = image_PatternName.color;
        Color textColor = text_SkillDescription.color;
        string skillName, skillDescription;

        for (float f = 1; f > 0.25f; f -= Time.deltaTime)
        {
            color.a = f;
            textColor.a = f;
            image_PatternName.color = color;
            text_SkillDescription.color = textColor;
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);

        battleManager.EnemyPatternContainer.PrepareSkill();
        skillName = battleManager.EnemyPatternContainer.GetSkillNameText();
        skillDescription = battleManager.EnemyPatternContainer.GetSkillDescriptionText();

        BattleManager.Instance.UIValueChanger.ChangeUIText(BattleTextUIEnum.MonsterSkillName, $"{skillName} 준비중");
        BattleManager.Instance.UIValueChanger.ChangeUIText(BattleTextUIEnum.MonsterSkillDescription, skillDescription);
        

        button.interactable = true;
        text_SkillDescription.text = battleManager.Enemy.currentSkill.SkillDescription;
        yield return new WaitForSeconds(0.15f);

        for (float f = 0.25f; f <= 1; f += Time.deltaTime)
        {
            color.a = f;
            textColor.a = f;
            image_PatternName.color = color;
            text_SkillDescription.color = textColor;
            yield return null;
        }
    }

    public void OnClickCloseDisplayer()
    {
        descriptionPanel.SetActive(false);
    }
}

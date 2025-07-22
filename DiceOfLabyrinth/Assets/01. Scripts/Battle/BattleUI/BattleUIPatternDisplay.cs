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

    public override void OnOffButton(PlayerTurnState state)
    {
        switch (state)
        {
            case PlayerTurnState.BattleStart:
                gameObject.SetActive(true);
                descriptionPanel.gameObject.SetActive(false);
                break;
            case PlayerTurnState.Enter:
                StartCoroutine(BlinkUI());
                break;
            case PlayerTurnState.Roll:
                button.interactable = false;
                break;
            case PlayerTurnState.Confirm:
                button.interactable = true;
                break;
            case PlayerTurnState.BattleEnd:
                descriptionPanel.gameObject.SetActive(true);
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnPush()
    {
        descriptionPanel.gameObject.SetActive(true);
    }

    IEnumerator BlinkUI()
    {
        Color color = image_PatternName.color;
        Color textColor = text_SkillDescription.color;

        for (float f = 1; f > 0.25f; f -= Time.deltaTime)
        {
            color.a = f;
            textColor.a = f;
            image_PatternName.color = color;
            text_SkillDescription.color = textColor;
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);
        BattleManager.Instance.EnemyPatternContainer.PrepareSkill();
        text_SkillDescription.text = BattleManager.Instance.Enemy.currentSkill.SkillDescription;
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

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BattleUIPatternDisplay : AbstractBattleButton
{
    Image[] images;
    TextMeshProUGUI text;   
    Button Button;

    public override void GetButtonComponent()
    {
        images = GetComponentsInChildren<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();        
        Button = GetComponentInChildren<Button>();
    }

    public override void OnOffButton(PlayerTurnState state)
    {
        switch (state)
        {
            case PlayerTurnState.BattleStart:
                gameObject.SetActive(true);
                images[1].gameObject.SetActive(false);
                break;
            case PlayerTurnState.Enter:
                StartCoroutine(BlinkUI());
                break;
            case PlayerTurnState.Roll:
                Button.interactable = false;
                break;
            case PlayerTurnState.Confirm:
                Button.interactable = true;
                break;
            case PlayerTurnState.BattleEnd:
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnPush()
    {
        if (images[1].IsActive())
        {
            images[1].gameObject.SetActive(false);
        }
        else
        {
            images[1].gameObject.SetActive(true);
        }        
    }

    IEnumerator BlinkUI()
    {
        Color color = images[0].color;
        Color textColor = text.color;

        for (float f = 1; f > 0.25f; f -= Time.deltaTime)
        {
            color.a = f;
            textColor.a = f;
            images[0].color = color;
            text.color = textColor;
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);
        BattleManager.Instance.LoadMonsterPattern.PrepareSkill();
        yield return new WaitForSeconds(0.15f);

        for (float f = 0.25f; f <= 1; f += Time.deltaTime)
        {
            color.a = f;
            textColor.a = f;
            images[0].color = color;
            text.color = textColor;
            yield return null;
        }
    }
}

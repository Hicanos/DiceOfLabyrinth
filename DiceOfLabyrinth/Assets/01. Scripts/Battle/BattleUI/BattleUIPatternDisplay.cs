using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BattleUIPatternDisplay : AbstractBattleButton
{
    [SerializeField] GameObject panel;
    Image image;
    TextMeshProUGUI text;   
    Button Button;

    public override void Setting()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();        
        Button = GetComponentInChildren<Button>();
    }

    public override void OnOffButton(PlayerTurnState state)
    {
        switch (state)
        {
            case PlayerTurnState.BattleStart:
                gameObject.SetActive(true);
                panel.gameObject.SetActive(false);
                break;
            case PlayerTurnState.Enter:
                if(BattleManager.Instance.isWon == false)
                {
                    StartCoroutine(BlinkUI());
                }
                break;
            case PlayerTurnState.Roll:
                Button.interactable = false;
                break;
            case PlayerTurnState.Confirm:
                Button.interactable = true;
                break;
            case PlayerTurnState.BattleEnd:
                panel.gameObject.SetActive(true);
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnPush()
    {
        panel.gameObject.SetActive(true);
    }

    IEnumerator BlinkUI()
    {
        Color color = image.color;
        Color textColor = text.color;

        for (float f = 1; f > 0.25f; f -= Time.deltaTime)
        {
            color.a = f;
            textColor.a = f;
            image.color = color;
            text.color = textColor;
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);
        BattleManager.Instance.EnemyPatternContainer.PrepareSkill();
        yield return new WaitForSeconds(0.15f);

        for (float f = 0.25f; f <= 1; f += Time.deltaTime)
        {
            color.a = f;
            textColor.a = f;
            image.color = color;
            text.color = textColor;
            yield return null;
        }
    }

    public void OnClickCloseDisplayer()
    {
        panel.SetActive(false);
    }
}

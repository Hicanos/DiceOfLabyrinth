using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoButton : AbstractBattleButton
{
    [SerializeField] int index;
    [SerializeField] Button button;
    [SerializeField] RectTransform rectTransform;
    public override void Setting()
    {
        SetPosition();
    }

    public override void OnOffButton(DetailedTurnState state)
    {
        switch(state)
        {
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
        UIManager.Instance.BattleUI.OpenCharacterInfo(index);
    }

    private void SetPosition()
    {
        Vector3 result;

        Vector3 charVec = BattleManager.Instance.PartyData.Characters[index].Prefab.transform.position;
        
        result = Camera.main.WorldToViewportPoint(charVec);

        //Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, charVec);
        rectTransform.anchoredPosition = result;
    }
}

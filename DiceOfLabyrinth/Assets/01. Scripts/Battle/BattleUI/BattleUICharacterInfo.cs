using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUICharacterInfo : MonoBehaviour
{
    [SerializeField] Image portrait;
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] TextMeshProUGUI textPosition;
    [SerializeField] TextMeshProUGUI textAtk;
    [SerializeField] TextMeshProUGUI textDef;
    [SerializeField] TextMeshProUGUI textHP;
    [SerializeField] TextMeshProUGUI textCNo;

    public void UpdateCharacterInfo(int index)
    {
        BattleCharacter character = BattleManager.Instance.BattleGroup.BattleCharacters[index];

        textName.text = character.CharNameKr;
        textPosition.text = "포지션";
        textAtk.text = character.CurrentATK.ToString();
        textDef.text = character.CurrentDEF.ToString();
        textHP.text = character.RegularHP.ToString();
        textCNo.text = character.CharacterData.charDiceData.CignatureNo.ToString();
    }

    public void OnClickClosePanel()
    {
        gameObject.SetActive(false);
    }
}

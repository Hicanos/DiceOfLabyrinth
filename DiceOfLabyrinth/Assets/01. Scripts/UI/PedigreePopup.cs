using UnityEngine;
using UnityEngine.UI;

public class PedigreePopup : MonoBehaviour
{
    [SerializeField] private Button closeBg;
    [SerializeField] private Button closeButton;

    [SerializeField] private Button diceCommonPedigreeButton;
    [SerializeField] private Button characterPassivePedigreeButton;

    [SerializeField] private GameObject selectDiceCommonPedigree;
    [SerializeField] private GameObject selectCharacterPassivePedigree;

    [SerializeField] private GameObject diceCommonPedigreeUI;
    [SerializeField] private GameObject characterPassivePedigreeUI;

    private void Awake()
    {
        selectDiceCommonPedigree.SetActive(true);
        selectCharacterPassivePedigree.SetActive(false);
        diceCommonPedigreeUI.SetActive(true);
        characterPassivePedigreeUI.SetActive(false);
    }

    public void OnClickCloseButton()
    {
        OnClickDiceCommonPedigreeButton();
        gameObject.SetActive(false);
    }

    public void OnClickDiceCommonPedigreeButton()
    {
        selectCharacterPassivePedigree.SetActive(false);
        selectDiceCommonPedigree.SetActive(true);
        characterPassivePedigreeUI.SetActive(false);
        diceCommonPedigreeUI.SetActive(true);
    }

    public void OnClickCharacterPassivePedigreeButton()
    {
        selectCharacterPassivePedigree.SetActive(true);
        selectDiceCommonPedigree.SetActive(false);
        characterPassivePedigreeUI.SetActive(true);
        diceCommonPedigreeUI.SetActive(false);
    }
}

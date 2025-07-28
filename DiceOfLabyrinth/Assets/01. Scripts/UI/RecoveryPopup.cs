using Newtonsoft.Json.Bson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecoveryPopup : MonoBehaviour
{
    public static RecoveryPopup Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject shopPopup;
    [SerializeField] private MessagePopup messagePopup;

    [Header("Character Viewers")]
    [SerializeField] private GameObject[] characterViewers;
    [SerializeField] private GameObject[] characterImages;
    [SerializeField] private GameObject[] characterHealthBars;
    [SerializeField] private TMP_Text[] characterHealthBarTexts;

    [Header("Viewer Refresh Colors")]
    [SerializeField, Range(0f, 1f)] private float selectedAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float unselectedAlpha = 0.5f;

    [Header("Recovery Costs")]
    [SerializeField] private int focusedRecoveryCost = 100;
    [SerializeField] private int allRecoveryCost = 150;
    [SerializeField] private int reviveCost = 100;

    private int selectedCharacterIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;// Don't destroy on load는 필요하지 않음
        }
    }
    private void OnEnable()
    {
        StartRecoveryPopup();
    }

    private void StartRecoveryPopup()
    {
        for (int i = 0; i < characterViewers.Length; i++)
        {
            PlayerViewerRefresh(i);
        }
        OnClickCharacterViewer(0); // 기본적으로 첫 번째 캐릭터 선택
    }
    private void PlayerViewerRefresh(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= characterViewers.Length)
        {
            return;
        }
        var character = StageManager.Instance.stageSaveData.battleCharacters[characterIndex];
        characterImages[characterIndex].GetComponent<Image>().sprite = character.CharacterData.Upper;
        characterHealthBars[characterIndex].GetComponent<Slider>().value = character.CurrentHP / (float)character.RegularHP;
        characterHealthBarTexts[characterIndex].text = $"{character.CurrentHP}/{character.RegularHP}";
        for (int i = 0; i < characterViewers.Length; i++)
        {
            if (characterViewers[i].GetComponent<CanvasGroup>() == null)
            {
                characterViewers[i].AddComponent<CanvasGroup>();
            }
            characterViewers[i].GetComponent<CanvasGroup>().alpha = (i == characterIndex) ? selectedAlpha : unselectedAlpha;
        }
    }

    public void OnClickCharacterViewer(int characterIndex)
    {
        selectedCharacterIndex = characterIndex;
        PlayerViewerRefresh(characterIndex);
    }

    public void OnClickFocusedRecovery()
    {
        
        if(StageManager.Instance.stageSaveData.battleCharacters[selectedCharacterIndex] == null)
        {
            messagePopup.Open("선택한 캐릭터가 존재하지 않습니다.");
            return;
        }
        if (selectedCharacterIndex < 0)
        {
            messagePopup.Open("회복할 캐릭터를 선택해주세요.");
            return;
        }
        if (StageManager.Instance.stageSaveData.battleCharacters[selectedCharacterIndex].IsDied)
        {
            messagePopup.Open("선택한 캐릭터는 이미 사망했습니다.");
            return;
        }
        if (StageManager.Instance.stageSaveData.battleCharacters[selectedCharacterIndex].CurrentHP >= StageManager.Instance.stageSaveData.battleCharacters[selectedCharacterIndex].RegularHP)
        {
            messagePopup.Open("선택한 캐릭터는 이미 최대 체력입니다.");
            return;
        }
        if (StageManager.Instance.stageSaveData.manaStone < focusedRecoveryCost)
        {
            messagePopup.Open($"마나스톤이 부족합니다. 회복을 위해서는 최소 {focusedRecoveryCost}의 마나스톤이 필요합니다.");
            return;
        }
        var character = StageManager.Instance.stageSaveData.battleCharacters[selectedCharacterIndex];
        if (character.CurrentHP >= character.RegularHP)
        {
            messagePopup.Open($"{character.CharNameKr}은(는) 이미 최대 체력입니다.");
            return;
        }
        character.CurrentHP = Mathf.Min(character.RegularHP, character.CurrentHP + (int)(0.8f * character.RegularHP));
        StageManager.Instance.stageSaveData.manaStone -= focusedRecoveryCost;
        messagePopup.Open($"{character.CharNameKr}의 체력을 80% 회복했습니다.");
        PlayerViewerRefresh(selectedCharacterIndex);
    }

    public void OnClickAllRecovery()
    {
        if (StageManager.Instance.stageSaveData.battleCharacters.Count == 0)
        {
            messagePopup.Open("회복할 캐릭터가 없습니다.");
            return;
        }
        if (StageManager.Instance.stageSaveData.manaStone < allRecoveryCost)
        {
            messagePopup.Open($"마나스톤이 부족합니다. 모든 캐릭터를 회복하기 위해서는 최소 {allRecoveryCost}의 마나스톤이 필요합니다.");
            return;
        }
        foreach (var character in StageManager.Instance.stageSaveData.battleCharacters)
        {
            if (character.CurrentHP < character.RegularHP && !character.IsDied)
            {
                character.CurrentHP = Mathf.Min(character.RegularHP, character.CurrentHP + (int)(0.3f * character.RegularHP));
            }
        }
        StageManager.Instance.stageSaveData.manaStone -= allRecoveryCost;
        messagePopup.Open("모든 캐릭터의 체력을 30% 회복했습니다.");
        for (int i = 0; i < characterViewers.Length; i++)
        {
            PlayerViewerRefresh(i);
        }
    }

    public void OnClickRevive()
    {
        if (StageManager.Instance.stageSaveData.battleCharacters[selectedCharacterIndex] == null)
        {
            messagePopup.Open("부활할 캐릭터가 없습니다.");
            return;
        }
        if (selectedCharacterIndex < 0)
        {
            messagePopup.Open("부활할 캐릭터를 선택해주세요.");
            return;
        }
        if (!StageManager.Instance.stageSaveData.battleCharacters[selectedCharacterIndex].IsDied)
        {
            messagePopup.Open("선택한 캐릭터는 이미 살아있습니다.");
            return;
        }
        if (StageManager.Instance.stageSaveData.manaStone < reviveCost)
        {
            messagePopup.Open($"마나스톤이 부족합니다. 부활을 위해서는 최소 {reviveCost}의 마나스톤이 필요합니다.");
            return;
        }
        var character = StageManager.Instance.stageSaveData.battleCharacters[selectedCharacterIndex];
        character.Revive();
        StageManager.Instance.stageSaveData.manaStone -= reviveCost;
        messagePopup.Open($"{character.CharNameKr}이(가) 부활했습니다.");
        PlayerViewerRefresh(selectedCharacterIndex);
    }
    public void OnClickClose()
    {
        gameObject.SetActive(false);
        StageManager.Instance.stageSaveData.currentPhaseIndex = 5;
        StageManager.Instance.battleUIController.OpenStagePanel(5);
    }
    public void OnClickShop()
    {
        shopPopup.SetActive(true);
        gameObject.SetActive(false);
    }
}

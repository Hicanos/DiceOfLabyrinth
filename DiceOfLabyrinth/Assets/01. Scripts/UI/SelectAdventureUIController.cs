using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using System.Linq;

public class SelectAdventureUIController : MonoBehaviour
{
    public ChapterData chapterData;
    public MessagePopup messagePopup; // 체크 패널, 챕터가 잠겨있을 때 팝업을 띄우기 위해 사용합니다.

    [SerializeField] private int selectedChapterIndex = 0; // 선택된 챕터 인덱스
    [SerializeField] private int selectedChapterButtonIndex = -1; // 선택된 챕터 버튼 인덱스

    [SerializeField] private CanvasGroup scarceStaminaFade;

    [Header("DOTWeen")]
    [SerializeField] float fadeIn = .25f;
    [SerializeField] float fadeOut = .20f;
    [SerializeField] float popScale = 1.1f;

    [SerializeField] int jewelCost = 50; // 스테미나를 구매하는데 드는 쥬얼의 비용

    [Header("Panels")]
    [SerializeField] private GameObject selectChapterPanel;
    //[SerializeField] private GameObject selectDungeonPanel; //배틀씬으로 패널을 옮겼으므로 주석 처리
    [SerializeField] private GameObject scarceStaminaPanel;
    //[SerializeField] private GameObject teamFormationPanel; // 배틀씬으로 패널을 옮겼으므로 주석 처리

    [Header("Difficulty Toggles")]
    [SerializeField] private GameObject NormalDifficultyUnselect;
    [SerializeField] private GameObject NormalDifficultySelect;
    [SerializeField] private GameObject HardDifficultyUnselect;
    [SerializeField] private GameObject HardDifficultySelect;

    [Header("Selected Chapter")]
    [SerializeField] private Image chapterIconSelected; // 선택된 챕터 아이콘
    //[SerializeField] private List<TMP_Text> selectedChapterNameText = new List<TMP_Text>(); // 선택된 챕터 이름 텍스트, 여러 개의 챕터 이름을 표시할 수 있도록 리스트로 변경
    [SerializeField] private TMP_Text selectedChapterDescriptionText;

    [Header("Selected Chapter Group")]
    [SerializeField] private TMP_Text selectedChapterGroupNumberText; // 선택된 챕터 그룹 번호 텍스트
    private int selectedChapterGroupNumber = 0; // 선택된 챕터 그룹 번호, 노말과 하드를 합쳐서 챕터 10개씩 그룹화합니다.
    [SerializeField] private List<TMP_Text> chapterTexts = new List<TMP_Text>(5); // 챕터 텍스트 리스트, 각 그룹의 챕터 이름을 표시합니다.

    [Header("Direct Complete Multiplier")]
    [SerializeField] private TMP_Text directCompleteMultiplierText; // 직접 완료 배수 텍스트
    [SerializeField] private TMP_Text actualCostText; // 직접 완료 비용 텍스트
    private int directCompleteMultiplier; // 직접 완료 배수 값

    [Header("스태미나 부족 패널")]
    [SerializeField] private TMP_Text beforeStaminaText; // 지불 전 스태미너를 보여주는 텍스트
    [SerializeField] private TMP_Text afterStaminaText; // 지불 후 스태미너를 보여주는 텍스트
    [SerializeField] private TMP_Text jewelCostText; // 지불 후 스태미너를 보여주는 텍스트

    [Header("즉시 완료 리워드")]
    [SerializeField] private TMP_Text directCompletePotionRewardText;
    [SerializeField] private TMP_Text directCompleteExpRewardText;
    [SerializeField] private TMP_Text directCompleteGoldRewardText;
    public bool isDifficulty = false; // 챕터 난이도 선택 여부

    [Header("Chapter Buttons Images")]
    [SerializeField] private List<Image> chapterButtonsImages = new List<Image>(5); // 챕터 버튼 이미지 리스트, 각 그룹의 챕터 버튼 이미지를 표시합니다.
    [Header("Chapter Buttons Rock Icons")]
    [SerializeField] private List<GameObject> chapterButtonsRockIcons = new List<GameObject>(5); // 챕터 버튼 리스트, 각 그룹의 챕터 버튼을 표시합니다.

    [Header("Chapter Buttons Select Color")]
    [SerializeField] private Color selectedButtonColor = new(1f,1f,1f,1f); // 선택된 챕터 버튼 색상
    [SerializeField] private Color unselectedButtonColor = new(156/255f, 156 / 255f, 156 / 255f, 1f); // 선택되지 않은 챕터 버튼 색상
    [SerializeField] private Color unselectedButtonTextColor = new(35/255f, 35 / 255f, 35 / 255f, 1f); // 선택되지 않은 챕터 버튼 텍스트 색상
    [SerializeField] private Color selectedButtonTextColor = new(217/255f, 217 / 255f, 217 / 255f, 1f); // 선택된 챕터 버튼 텍스트 색상


    private void OnEnable()
    {
        selectChapterPanel.SetActive(true);
        scarceStaminaPanel.SetActive(false);
        DifficultyToggleRefresh(); // 초기 난이도 토글 상태 설정
        OnClickMultipleMinButton(); // 직접 완료 배수 초기화
        OnClickChapterButton(0); // 첫 번째 챕터 버튼 클릭하여 초기화
    }

    public void OnClickChapterButton(int normalChapterIndex) // 챕터 버튼
    {
        int chapterIndex = normalChapterIndex + selectedChapterGroupNumber * 10 + (isDifficulty ? 1 : 0); // 현재 그룹 번호와 난이도에 따라 챕터 인덱스 계산

        if (chapterData == null)
        {
            messagePopup.Open("chapterData is null");
            return;
        }
        if (chapterData.chapterIndex == null)
        {
            messagePopup.Open("chapterData.chapterIndex is null");
            return;
        }
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count) // 유효하지 않은 챕터 인덱스일 때
        {
            messagePopup.Open("선택한 챕터가 유효하지 않습니다. 다시 시도해 주세요.");
            return;
        }
        if (StageManager.Instance.stageSaveData.chapterStates == null ||
        StageManager.Instance.stageSaveData == null ||
        StageManager.Instance.stageSaveData.chapterStates.Count == 0) // 챕터 상태가 초기화되지 않았을 때
        {
            StageManager.Instance.stageSaveData = DataSaver.Instance.SaveData.stageData.ToStageSaveData();
            StageManager.Instance.InitializeStageStates(StageManager.Instance.chapterData);
        }
        if (StageManager.Instance.stageSaveData.chapterStates[chapterIndex].isUnLocked == false) // 챕터가 잠겨있을 때
        {
            messagePopup.Open($"챕터({chapterData.GetNameAndDifficulty(chapterIndex)})가 잠겨 있습니다. 다른 챕터를 완료한 후 다시 시도해 주세요.");
            return;
        }
        selectedChapterButtonIndex = normalChapterIndex / 2; // 선택된 챕터 버튼 인덱스 설정
        selectedChapterIndex = chapterIndex; // 선택된 챕터 인덱스 설정
        UpdateSelectedChapterUI(chapterIndex); // 선택된 챕터 UI 업데이트
        RefreshChapterButton(); // 챕터 버튼 텍스트 업데이트
    }

    public void OnClickChapterGroupPreviousButton() // 챕터 그룹 버튼
    {
        if(selectedChapterGroupNumber > 0) // 현재 그룹 번호가 0보다 클 때
        {
            selectedChapterGroupNumber--;
            selectedChapterGroupNumberText.text = $"{selectedChapterGroupNumber + 1}"; // 그룹 번호 텍스트 업데이트
            selectedChapterButtonIndex = -1; // 선택 초기화
            RefreshChapterButton(); // 챕터 버튼 텍스트 업데이트
            OnClickChapterButton(0); // 첫 번째 챕터 버튼을 클릭하여 초기화
        }
        else if (selectedChapterGroupNumber == 0) // 현재 그룹 번호가 0일 때
        {
            messagePopup.Open("첫 번째 그룹입니다. 이전 그룹이 없습니다.");
        }
        else
        {
            messagePopup.Open("챕터 그룹 번호가 잘못되었습니다.");
        }
    }
    public void OnClickChapterGroupNextButton() // 챕터 그룹 버튼
    {
        if (selectedChapterGroupNumber < chapterData.chapterIndex.Count / 10 - 1) // 10개씩 그룹화했으므로, 최대 그룹 번호는 총 챕터 수 / 10 - 1입니다.
        {
            selectedChapterGroupNumber++;
            selectedChapterGroupNumberText.text = $"{selectedChapterGroupNumber + 1}"; // 그룹 번호 텍스트 업데이트
            selectedChapterButtonIndex = -1; // 선택 초기화
            RefreshChapterButton(); // 챕터 버튼 텍스트 업데이트
            OnClickChapterButton(0); // 첫 번째 챕터 버튼을 클릭하여 초기화
        }
        else if (selectedChapterGroupNumber == chapterData.chapterIndex.Count / 10 - 1) // 현재 그룹 번호가 최대 그룹 번호일 때
        {
            messagePopup.Open("마지막 그룹입니다. 다음 그룹이 없습니다.");
        }
        else
        {
            messagePopup.Open("챕터 그룹 번호가 잘못되었습니다.");
        }
    }
    private void RefreshChapterButton()
    {
        for (int i = 0; i < 5; i++)
        {
            int chapterIndex = i * 2 + selectedChapterGroupNumber * 10 + (isDifficulty ? 1 : 0);

            if (chapterIndex < chapterData.chapterIndex.Count)
            {
                chapterTexts[i].text = $"{chapterData.chapterIndex[chapterIndex].ChapterName}";
            }
            else
            {
                chapterTexts[i].text = "N/A";
            }

            bool selectableChapterState = StageManager.Instance.stageSaveData.chapterStates[chapterIndex].isUnLocked;
            chapterButtonsRockIcons[i].SetActive(!selectableChapterState);

            // 선택된 버튼만 강조, 나머지는 비선택
            if (selectedChapterButtonIndex == i)
            {
                chapterButtonsImages[i].color = selectedButtonColor;
                chapterTexts[i].color = selectedButtonTextColor;
            }
            else
            {
                chapterButtonsImages[i].color = unselectedButtonColor;
                chapterTexts[i].color = unselectedButtonTextColor;
            }
        }
    }
    public void OnClickStartButton() // 코스트 지불 UI의 시작 버튼
    {
        int chapterIndex = selectedChapterIndex; // 선택된 챕터 인덱스 가져오기
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            messagePopup.Open("선택한 챕터가 유효하지 않습니다. 다시 시도해 주세요.");
            return;
        }
        else if (CharacterManager.Instance.OwnedCharacters.Count < 5) // 캐릭터가 5개 미만일 때
        {
            messagePopup.Open("캐릭터를 5개 이상 보유해야 챕터를 선택할 수 있습니다.");
            return;
        }
        else if (!StageManager.Instance.stageSaveData.chapterStates[chapterIndex].isUnLocked)
        {
            messagePopup.Open("이 챕터는 아직 잠겨 있습니다. 다른 챕터를 완료한 후 다시 시도해 주세요.");

            return;
        }
        else if (StageManager.Instance.stageSaveData.currentChapterIndex == -1) // -1은 진행중인 챕터가 없음을 의미합니다.
        {
            if (StageManager.Instance.stageSaveData.chapterStates[chapterIndex].isCompleted)
            {
                messagePopup.Open(
                $"해당 챕터는 이미 완료되었습니다. 다시 시작하시겠습니까?.",
                 () => EnterDungeon(), // 확인(Yes) 버튼 클릭 시 입장
                 () => messagePopup.Close() // 취소(No) 버튼 클릭 시
            );
                return;
            }
            EnterDungeon(); // 입장
        }
        else if (chapterIndex != StageManager.Instance.stageSaveData.currentChapterIndex) // 현재 챕터와 선택한 챕터가 다를 때엔 이전 챕터의 종료를 먼저 하라는 팝업을 띄워야 합니다.
        {
            messagePopup.Open($"진행 중인 챕터({StageManager.Instance.chapterData.GetNameAndDifficulty(StageManager.Instance.stageSaveData.currentChapterIndex)})가 있습니다. 먼저 해당 챕터를 종료한 후 다시 시도해 주세요.");
            return;
        }
        else if (StageManager.Instance.stageSaveData.currentChapterIndex == chapterIndex) // 현재 진행 중인 챕터가 선택한 챕터와 같을 때
        {
            
            {
                // 코스트 지불 없이 바로 배틀 씬으로 이동할 수 있도록 처리합니다.
                // 진행중이던 챕터를 다시 시작했다는 팝업을 띄우는 로직을 추가할 수 있습니다.
                messagePopup.Open($"진행 중이던 챕터({StageManager.Instance.chapterData.GetNameAndDifficulty(StageManager.Instance.stageSaveData.currentChapterIndex)})를 다시 시작하시겠습니까?",
                () => {
                    selectChapterPanel.SetActive(false);
                    scarceStaminaPanel.SetActive(false);
                    SceneManagerEx.Instance.LoadScene("BattleScene"); // 배틀 씬으로 이동
                    StageManager.Instance.RestoreStageState();
                }, // 확인(Yes) 버튼 클릭 시
                () =>
                {
                    messagePopup.Close(); // 취소(No) 버튼 클릭 시
                    messagePopup.Open("진행 중이던 챕터를 정산하시겠습니까? \n" +
                        "정산을 하게 되면 현재 진행 중인 챕터의 상태가 초기화됩니다. \n" +
                        "정산 후에는 다시 진행할 수 없습니다.",
                        () => {
                            StageManager.Instance.EndChapterEarly(chapterIndex); // 현재 진행 중인 챕터를 정산합니다.
                        }, // 확인(Yes) 버튼 클릭 시
                        () => messagePopup.Close() // 취소(No) 버튼 클릭 시
                    );
                }
                );
                return;
            }
        }
    }

    public void EnterDungeon()
    {
        int chapterIndex = selectedChapterIndex; // 선택된 챕터 인덱스 가져오기
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            messagePopup.Open("선택한 챕터가 유효하지 않습니다. 다시 시도해 주세요.");
            return;
        }
        var selectedChapter = chapterData.chapterIndex[chapterIndex];
        if (!UserDataManager.Instance.UseStamina(selectedChapter.ChapterCost)) // 코스트를 지불하지 못하면,
        {
            OpenScaresStaminaPanel();
            //return;
        }
        else
        {
            selectedChapterIndex = 0; // 선택된 챕터 인덱스 초기화
            StageManager.Instance.stageSaveData.currentChapterIndex = chapterIndex; // 현재 챕터 인덱스 설정
            StageManager.Instance.stageSaveData.ResetToDefault(chapterIndex); // 스테이지 상태 초기화
            Debug.Log($"Starting battle for chapter: {selectedChapter.ChapterName} (Index: {chapterIndex})");
            SceneManagerEx.Instance.LoadScene("BattleScene"); // 배틀 씬으로 이동
            //costCalculationPanel.SetActive(false);
            StageManager.Instance.RestoreStageState(); // 현재 스테이지 상태 복원
        }
    }

    private void UpdateDirectCompleteMultiplierText() // 직접 완료 배수 텍스트 업데이트
    {
        directCompleteMultiplierText.text = $"{directCompleteMultiplier}";
        int actualCost = chapterData.chapterIndex[selectedChapterIndex].ChapterCost * directCompleteMultiplier; // 직접 완료 비용 계산
        actualCostText.text = $"{actualCost}"; // 실제 비용 텍스트 업데이트
        int directCompleteExpReward = chapterData.chapterIndex[selectedChapterIndex].stageData.DirectCompleteExpReward; // 직접 완료 경험치 보상
        int directCompleteGoldReward = chapterData.chapterIndex[selectedChapterIndex].stageData.DirectCompleteGoldReward; // 직접 완료 골드 보상
        int directCompletePotionReward = chapterData.chapterIndex[selectedChapterIndex].stageData.DirectCompletePotionReward; // 직접 완료 포션 보상
        directCompletePotionRewardText.text = $"{directCompletePotionReward * directCompleteMultiplier}"; // 포션 보상 텍스트 업데이트
        directCompleteExpRewardText.text = $"{directCompleteExpReward * directCompleteMultiplier}"; // 경험치 보상 텍스트 업데이트
        directCompleteGoldRewardText.text = $"{directCompleteGoldReward * directCompleteMultiplier}"; // 골드 보상 텍스트 업데이트
    }

    public void OnClickMultiplePlusButton() // 직접 완료 배수 증가 버튼
    {
        if (!StageManager.Instance.stageSaveData.chapterStates[selectedChapterIndex].isCompleted || 
            StageManager.Instance.stageSaveData.chapterStates[selectedChapterIndex] == null)
        {
            OnClickMultipleMinButton();
            return; // 완료되지 않은 챕터를 선택했을 때는 1배수로 설정합니다.
        }
        if (directCompleteMultiplier < 5) // 최대 배수는 5로 설정
        {
            directCompleteMultiplier++;
            UpdateDirectCompleteMultiplierText();
        }
    }
    public void OnClickMultipleMinusButton() // 직접 완료 배수 감소 버튼
    {
        if (!StageManager.Instance.stageSaveData.chapterStates[selectedChapterIndex].isCompleted || 
            StageManager.Instance.stageSaveData.chapterStates[selectedChapterIndex] == null)
        {
            OnClickMultipleMaxButton();
            return; // 완료되지 않은 챕터를 선택했을 때는 1배수로 설정합니다.
        }
        if (directCompleteMultiplier > 1) // 최소 배수는 1로 설정
        {
            directCompleteMultiplier--;
            UpdateDirectCompleteMultiplierText();
        }
    }
    public void OnClickMultipleMaxButton()
    {
        if (!StageManager.Instance.stageSaveData.chapterStates[selectedChapterIndex].isCompleted || 
            StageManager.Instance.stageSaveData.chapterStates[selectedChapterIndex] == null)
        {
            OnClickMultipleMinButton();
            return; // 완료되지 않은 챕터를 선택했을 때는 1배수로 설정합니다.
        }
        directCompleteMultiplier = 5; // 최대 배수로 설정
        UpdateDirectCompleteMultiplierText();
    }
    public void OnClickMultipleMinButton()
    {
        directCompleteMultiplier = 1; // 최소 배수로 설정
        UpdateDirectCompleteMultiplierText();
    }
    public void OnClickDirectCompliteButton()
    {
        int actualCost = chapterData.chapterIndex[selectedChapterIndex].ChapterCost * directCompleteMultiplier; // 직접 완료 비용 계산, 스태미나를 코스트로 사용합니다.
        if (selectedChapterIndex < 0 || selectedChapterIndex >= chapterData.chapterIndex.Count) // 유효하지 않은 챕터 인덱스일 때
        {
            messagePopup.Open("선택한 챕터가 유효하지 않습니다. 다시 시도해 주세요.");
            return;
        }
        else if (!StageManager.Instance.stageSaveData.chapterStates[selectedChapterIndex].isCompleted) // 완료되지 않은 챕터를 할 때
        {
            messagePopup.Open($"한 번 이상 완료된 챕터만 소탕할 수 있습니다.\n현재 챕터는 완료되지 않았습니다.");
        }
        else if (StageManager.Instance.stageSaveData.currentChapterIndex != -1) // 현재 진행 중인 챕터가 있을 때
        {
            messagePopup.Open($"진행 중인 챕터({StageManager.Instance.chapterData.GetNameAndDifficulty(StageManager.Instance.stageSaveData.currentChapterIndex)})가 있습니다. 먼저 해당 챕터를 종료한 후 다시 시도해 주세요.");
        }
        else if (UserDataManager.Instance.userdata.stamina < actualCost) // 스태미나가 부족할 때
        {
            messagePopup.Open($"직접 완료를 하려면 {actualCost} 스태미나가 필요합니다. 충전하시겠습니까?",
                () => OpenScaresStaminaPanel(), // 스태미나 부족 UI 열기
                () => messagePopup.Close() // 취소 버튼 클릭 시
            );
        }
        else
        {
            messagePopup.Open(
                $"직접 완료를 하시겠습니까?\n{actualCost} 스태미나가 소모됩니다.",
                () => 
                {
                    DirectComplite(actualCost); // 직접 완료 처리
                },
                () => messagePopup.Close() // 취소 버튼 클릭 시
            );
        }
    }

    private void DirectComplite(int actualCost)
    {
        var stage = chapterData.chapterIndex[selectedChapterIndex].stageData;
        int directCompleteExpReward = stage.DirectCompleteExpReward;
        int directCompleteGoldReward = stage.DirectCompleteGoldReward;
        int directCompletePotionReward = stage.DirectCompletePotionReward;
        UserDataManager.Instance.UseStamina(actualCost); // 스태미나 사용
        UserDataManager.Instance.AddExp(directCompleteExpReward * directCompleteMultiplier); // 경험치 보상 추가
        UserDataManager.Instance.AddGold(directCompleteGoldReward * directCompleteMultiplier); // 골드 보상 추가
        Dictionary<EXPpotion, int> potionResults = new();

        int remainingExp = directCompletePotionReward * directCompleteMultiplier; // 포션 보상 경험치 총합

        var potionList = ItemManager.Instance.AllItems.Values.OfType<EXPpotion>().OrderByDescending(p => p.ExpAmount).ToList();
        foreach (var potion in potionList)
        {
            if (potion.ExpAmount <= 0) continue;

            int count = remainingExp / potion.ExpAmount;
            if (count > 0)
            {
                potionResults[potion] = count;
                remainingExp %= potion.ExpAmount;
            }
        }

        string potionRewardText = "";
        foreach (var kvp in potionResults)
        {
            ItemManager.Instance.GetItem(kvp.Key.ItemID, kvp.Value);
            potionRewardText += $"{kvp.Key.NameKr}: {kvp.Value}개\n"; // 포션 보상 텍스트 생성
        }

        messagePopup.Open(
        $"직접 완료가 완료되었습니다!\n" +
        $"경험치: {directCompleteExpReward * directCompleteMultiplier}exp\n" +
        $"골드: {directCompleteGoldReward * directCompleteMultiplier}골드\n" +
        potionRewardText
        );
    }

    private void OpenScaresStaminaPanel() // 스태미나 부족 UI 열기
    {
        selectChapterPanel.SetActive(true);
        scarceStaminaPanel.SetActive(true);
        UpdateStaminaUI(); // 스태미나 UI 업데이트

#if DOTWEEN
        scarceStaminaFade.alpha = 0;
        scarceStaminaFade.DOFade(1, fadeIn);

        transform.localScale = Vector3.one * popScale;
        transform.DOScale(1, fadeIn).SetEase(Ease.OutBack);
#endif
    }

    public void OnClickScarceStaminaPanelBackButton() // 스태미나 부족 UI 닫기
    {
        selectChapterPanel.SetActive(true);

#if DOTWEEN
        scarceStaminaFade.DOFade(0, fadeOut).OnComplete(() => scarceStaminaPanel.SetActive(false));
#endif
        scarceStaminaPanel.SetActive(false);
    }

    public void OnClickRechargeStaminaButton() // 스태미나 충전 버튼
    {
        Debug.Log("Recharge Stamina Button Clicked");
        if (!UserDataManager.Instance.UseJewel(50)) // 스태미나 충전 비용이 50 쥬얼이므로, 쥬얼이 부족할 경우
        {
            messagePopup.Open("스태미나를 충전하려면 50 쥬얼이 필요합니다.");
        }
        else
        { 
            UserDataManager.Instance.AddStamina(50); // 스태미나 50 증가
        }
        scarceStaminaPanel.SetActive(false);
    }

    public void OnClickDifficulty(bool DifficultyToggle)
    {
        isDifficulty = DifficultyToggle; // 토글 상태에 따라 난이도 설정
        selectedChapterButtonIndex = -1; // 선택 초기화
        DifficultyToggleRefresh();
        OnClickChapterButton(0); // 첫 번째 챕터 버튼 클릭하여 초기화
    }


    private void DifficultyToggleRefresh()
    {
        if (isDifficulty)
        {
            NormalDifficultyUnselect.SetActive(true);
            NormalDifficultySelect.SetActive(false);
            HardDifficultyUnselect.SetActive(false);
            HardDifficultySelect.SetActive(true);
        }
        else
        {
            NormalDifficultyUnselect.SetActive(false);
            NormalDifficultySelect.SetActive(true);
            HardDifficultyUnselect.SetActive(true);
            HardDifficultySelect.SetActive(false);
        }
        RefreshChapterButton(); // 챕터 버튼 텍스트 업데이트
    }

    private void UpdateSelectedChapterUI(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Cannot update UI.");
            return;
        }
        var selectedChapter = chapterData.chapterIndex[chapterIndex];
        //foreach (var text in selectedChapterNameText)// 현재 기획에서 챕터 이름을 표시할 필요가 없으므로 주석 처리
        //{
        //    text.text = selectedChapter.ChapterName;
        //}
        chapterIconSelected.sprite = chapterData.chapterIndex[selectedChapterIndex].Sprite;
        selectedChapterDescriptionText.text = selectedChapter.Description;
        int expReward = chapterData.chapterIndex[chapterIndex].stageData.DirectCompleteExpReward;
        int goldReward = chapterData.chapterIndex[chapterIndex].stageData.DirectCompleteGoldReward;
        int potionReward = chapterData.chapterIndex[chapterIndex].stageData.DirectCompletePotionReward;
        directCompletePotionRewardText.text = $"{potionReward * directCompleteMultiplier}";
        directCompleteExpRewardText.text = $"{expReward * directCompleteMultiplier}";
        directCompleteGoldRewardText.text = $"{goldReward * directCompleteMultiplier}";
    }

    private void UpdateStaminaUI() // 스태미나 부족 UI를 업데이트
    {
        // 스태미나 충전을 하게 되었을 때의 상태를 보여줍니다.
        beforeStaminaText.text = $"{UserDataManager.Instance.userdata.stamina}";
        afterStaminaText.text = $"{UserDataManager.Instance.userdata.stamina + 50}";
        jewelCostText.text = $"{jewelCost}"; // 추후 구매 할 때마다 가격 증가하도록 변경
    }
}
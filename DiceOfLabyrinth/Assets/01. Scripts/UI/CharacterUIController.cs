using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class CharacterUIController : MonoBehaviour
{
    public static CharacterUIController Instance;
    [Header("Panels")]
    [SerializeField] private GameObject characterPanel;
    [Header("Popups")]
    [SerializeField] private GameObject characterListPopup;
    [SerializeField] private GameObject characterInfoPopup;

    [Header("Character Viewers")]
    [SerializeField] private List<GameObject> ownedCharacterViewers = new List<GameObject>(); // 소유한 캐릭터 뷰어 목록
    [SerializeField] private Transform ownedCharacterViewerParent; // 캐릭터 뷰어가 배치될 부모 오브젝트
    [SerializeField] private List<GameObject> unownedCharacterViewers = new List<GameObject>(); // 소유하지 않은 캐릭터 뷰어 목록
    [SerializeField] private Transform unownedCharacterViewerParent; // 소유하지 않은 캐릭터 뷰어가 배치될 부모 오브젝트

    [Header("Info Popup")]
    [SerializeField] private GameObject basicInfoPopup; // 캐릭터 정보 팝업
    [SerializeField] private GameObject levelUpPopup; // 캐릭터 레벨업 팝업
    [SerializeField] private GameObject skillInfoPopup; // 캐릭터 스킬 정보 팝업
    [SerializeField, Range(0, 2)] private int popupState = 0; // 팝업 상태 (0: 기본 정보, 1: 레벨업, 2: 스킬 정보)
    [SerializeField] private GameObject[] popupButtons = new GameObject[3]; // 팝업 상태를 전환하는 버튼들
    [SerializeField] private Image characterStandingImage; // 캐릭터 풀바디 이미지 표시용 이미지

    [Header("Basic Info")]
    [SerializeField] private TMP_Text characterNameText; // 캐릭터 이름 표시용 텍스트
    [SerializeField] private TMP_Text characterlevelText; // 캐릭터 레벨 표시용 텍스트
    [SerializeField] private Image characterClassTypeImage; // 캐릭터 클래스 타입 표시용 이미지
    [SerializeField] private TMP_Text characterClassText; // 캐릭터 클래스 타입 표시용 텍스트
    [SerializeField] private Image characterElementTypeImage; // 캐릭터 엘리먼트 타입 표시용 이미지
    [SerializeField] private TMP_Text characterElementText; // 캐릭터 엘리먼트 타입 표시용 텍스트
    [SerializeField] private Image characterSignatureDiceImage; // 캐릭터 시그니처 주사위 이미지 표시용 이미지
    [SerializeField] private TMP_Text characterSignatureDiceText; // 캐릭터 시그니처 주사위 표시용 텍스트
    [SerializeField] private TMP_Text characterAffectionText; // 캐릭터 애정도 표시용 텍스트
    [SerializeField] private TMP_Text characterHp; // 캐릭터 HP 표시용 텍스트
    [SerializeField] private TMP_Text characterAtk; // 캐릭터 공격력 표시용 텍스트
    [SerializeField] private TMP_Text characterDef; // 캐릭터 방어력 표시용 텍스트
    [SerializeField] private TMP_Text characterCritChance; // 캐릭터 크리티컬 확률 표시용 텍스트
    [SerializeField] private TMP_Text characterCritDamage; // 캐릭터 크리티컬 데미지 표시용 텍스트
    [SerializeField] private TMP_Text characterElimentalDamage; // 캐릭터 엘리먼트 피해 표시용 텍스트

    [Header("Level Up Popup")]
    [SerializeField] private TMP_Text currentLevelText; // 현재 레벨 표시용 텍스트
    [SerializeField] private TMP_Text virtualLevelText; // 증가된 레벨 표시용 텍스트
    [SerializeField] private TMP_Text currentHPText; // 현재 HP 표시용 텍스트
    [SerializeField] private TMP_Text virtualHPText; // 증가된 HP 표시용 텍스트
    [SerializeField] private TMP_Text currentATKText; // 현재 공격력 표시용 텍스트
    [SerializeField] private TMP_Text virtualATKText; // 증가된 공격력 표시용 텍스트
    [SerializeField] private TMP_Text currentDEFText; // 현재 방어력 표시용 텍스트
    [SerializeField] private TMP_Text virtualDEFText; // 증가된 방어력 표시용 텍스트
    [SerializeField] private TMP_Text virtualExpText; // 캐릭터 경험치 표시용 텍스트
    [SerializeField] private Slider characterExpSlider; // 캐릭터 경험치 슬라이더

    [SerializeField] private TMP_Text lowLevelUpPotionAmountText; // 낮은 레벨업 포션 개수 표시용 텍스트
    [SerializeField] private TMP_Text lowLevelUpPotionConsumeText; // 낮은 레벨업 포션 소모량 표시용 텍스트
    [SerializeField] private GameObject lowLevelUpPotionCountDownButton; // 낮은 레벨업 포션 개수 감소 버튼
    [SerializeField] private TMP_Text midLevelUpPotionAmountText; // 중간 레벨업 포션 개수 표시용 텍스트
    [SerializeField] private TMP_Text midLevelUpPotionConsumeText; // 중간 레벨업 포션 소모량 표시용 텍스트
    [SerializeField] private GameObject midLevelUpPotionCountDownButton; // 중간 레벨업 포션 개수 감소 버튼
    [SerializeField] private TMP_Text highLevelUpPotionAmountText; // 높은 레벨업 포션 개수 표시용 텍스트
    [SerializeField] private TMP_Text highLevelUpPotionConsumeText; // 높은 레벨업 포션 소모량 표시용 텍스트
    [SerializeField] private GameObject highLevelUpPotionCountDownButton; // 높은 레벨업 포션 개수 감소 버튼
    [SerializeField] private TMP_Text royalLevelUpPotionAmountText; // 로열 레벨업 포션 개수 표시용 텍스트
    [SerializeField] private TMP_Text royalLevelUpPotionConsumeText; // 로열 레벨업 포션 소모량 표시용 텍스트
    [SerializeField] private GameObject royalLevelUpPotionCountDownButton; // 로열 레벨업 포션 개수 감소 버튼
    [SerializeField] private EXPpotion lowLevelUpPotion; // 낮은 레벨업 포션 데이터
    [SerializeField] private EXPpotion midLevelUpPotion; // 중간 레벨업 포션 데이터
    [SerializeField] private EXPpotion highLevelUpPotion; // 높은 레벨업 포션 데이터
    [SerializeField] private EXPpotion royalLevelUpPotion; // 로열 레벨업 포션 데이터


    private int lowLevelUpPotionConsumeAmount = 0; // 선택된 낮은 레벨업 포션 개수
    private int midLevelUpPotionConsumeAmount = 0; // 선택된 중간 레벨업 포션 개수
    private int highLevelUpPotionConsumeAmount = 0; // 선택된 높은 레벨업 포션 개수
    private int royalLevelUpPotionConsumeAmount = 0; // 선택된 로열 레벨업 포션 개수
    private int virtualLevel; // 가상 레벨
    private int virtualATK; // 가상 공격력
    private int virtualDEF; // 가상 방어력
    private int virtualHP; // 가상 HP
    private int virtualCurrentExp; // 가상 현재 경험치
    private int virtualExp => 
        (lowLevelUpPotionConsumeAmount * lowLevelUpPotion.ExpAmount) +
        (midLevelUpPotionConsumeAmount * midLevelUpPotion.ExpAmount) +
        (highLevelUpPotionConsumeAmount * highLevelUpPotion.ExpAmount) +
        (royalLevelUpPotionConsumeAmount * royalLevelUpPotion.ExpAmount); // 가상 경험치 계산


    private int skillInfoState = 0; // 스킬 정보 상태 (0: 액티브, 1: 패시브)
    [Header("Skill Info Popup")]
    [SerializeField] private TMP_Text[] skillNameText = new TMP_Text[2]; // 스킬 이름 표시용 텍스트
    [SerializeField] private TMP_Text[] skillCooldownText = new TMP_Text[2]; // 스킬 쿨타임 표시용 텍스트
    [SerializeField] private TMP_Text skillBeforeDescriptionText; // 스킬 설명 표시용 텍스트 (레벨업 전)
    [SerializeField] private TMP_Text skillAfterDescriptionText; // 스킬 설명 표시용 텍스트 (레벨업 후)
    [SerializeField] private TMP_Text skillBeforeLevelText; // 스킬 레벨 표시용 텍스트 (레벨업 전)
    [SerializeField] private TMP_Text skillAfterLevelText; // 스킬 레벨 표시용 텍스트 (레벨업 후)
    [SerializeField] private TMP_Text skillBeforeCostText; // 스킬 비용 표시용 텍스트 (레벨업 전)
    [SerializeField] private TMP_Text skillAfterCostText; // 스킬 비용 표시용 텍스트 (레벨업 후)
    [SerializeField] private Image passiveSkillIconImage; // 패시브 스킬 아이콘 이미지
    [SerializeField] private Image activeSkillIconImage; // 액티브 스킬 아이콘 이미지
    [SerializeField] private Image[] selectedSkillIconImage = new Image[2]; // 선택된 스킬 아이콘 이미지
    [SerializeField] private TMP_Text skillLevelUpGoldCostText; // 스킬 레벨업 비용 표시용 텍스트
    [SerializeField] private TMP_Text lowSkillBookCostText; // 낮은 스킬북 비용 표시용 텍스트 (소유 개수 / 필요한 개수)
    [SerializeField] private TMP_Text midSkillBookCostText; // 중간 스킬북 비용 표시용 텍스트 (소유 개수 / 필요한 개수)
    [SerializeField] private TMP_Text highSkillBookCostText; // 높은 스킬북 비용 표시용 텍스트 (소유 개수 / 필요한 개수)

    [SerializeField] private GameObject selectPassiveButton; // 패시브 스킬 선택 버튼
    [SerializeField] private GameObject selectActiveButton; // 액티브 스킬 선택 버튼

    [Header("Skill Book")]
    [SerializeField] private SkillBook lowSkillBook; // 낮은 스킬북 데이터
    [SerializeField] private SkillBook midSkillBook; // 중간 스킬북 데이터
    [SerializeField] private SkillBook highSkillBook; // 높은 스킬북 데이터

    [Header("Skill Level Up Gold Cost")]
    [SerializeField] private int[] skillLevelUpGoldCost = new int[4] {
        600, // 레벨 1 -> 2
        1200, // 레벨 2 -> 3
        2400, // 레벨 3 -> 4
        4800 // 레벨 4 -> 5
    }; // 스킬 레벨업 비용 배열
    [Header("Button Colors")]
    [SerializeField] private Color selectedButtonColor = new(170/255f,140/255f,100/255f,1); // 선택된 버튼 색상
    [SerializeField] private Color unselectedButtonColor = new(1,220/255f,170/255f,1); // 선택되지 않은 버튼 색상

    [Header("CharacterList")]
    [SerializeField] private List<CharacterSO> unownedCharacters = new List<CharacterSO>(); // 소유하지 않은 캐릭터 목록
    [SerializeField] private LobbyCharacter selectedCharacter; // 현재 선택된 캐릭터


    [Header("Potion Button Hold Settings")]
    private bool isPotionButtonHeld = false;
    private float potionButtonHoldTime = 0f;
    private float holdThreshold = 0.3f; // 홀드로 인식할 최소 시간(초)
    private float repeatRate = 0.05f;   // 연속 입력 간격(초)
    private float repeatTimer = 0f;
    private int holdPotionType = -1; // 현재 홀드 중인 포션 타입 (0: 낮은, 1: 중간, 2: 높은, 3: 로열, -1: 없음)
    private bool isCountUp = true;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        characterPanel.SetActive(true);
        OpenCharacterListPopup();
    }
    private void OnDisable()
    {
        characterPanel.SetActive(false);
        characterListPopup.SetActive(false);
        characterInfoPopup.SetActive(false);
        foreach (var viewer in ownedCharacterViewers)
        {
            viewer.SetActive(false);
        }
        foreach (var viewer in unownedCharacterViewers)
        {
            viewer.SetActive(false);
        }
    }
    private void Update()
    {
        if (isPotionButtonHeld)
        {
            potionButtonHoldTime += Time.unscaledDeltaTime;
            if (potionButtonHoldTime >= holdThreshold)
            {
                repeatTimer += Time.unscaledDeltaTime;
                if (repeatTimer >= repeatRate)
                {
                    repeatTimer = 0f;
                    if (isCountUp)
                        OnClickLevelUpPotionCountUpButton(holdPotionType);
                    else
                        OnClickLevelUpPotionCountDownButton(holdPotionType);
                }
            }
        }
    }
    public void OpenCharacterListPopup()
    {
        characterListPopup.SetActive(true);
        characterInfoPopup.SetActive(false);
        ListPopupRefresh();
    }


    private void ListPopupRefresh()
    {
        UnownedCharactersCounting();
        CreateOwnedCharacterViewers();
        CreateUnownedCharacterViewers();
    }
    // 소유하지 않은 캐릭터 목록을 갱신하는 메서드
    private void UnownedCharactersCounting()
    {
        unownedCharacters.Clear();

        var ownedCharIDs = new HashSet<string>(
            CharacterManager.Instance.OwnedCharacters.Select(oc => oc.CharacterData.charID)
        );

        foreach (var pair in CharacterManager.Instance.AllCharacters)
        {
            if (!ownedCharIDs.Contains(pair.Key))
            {
                unownedCharacters.Add(pair.Value);
            }
        }
    }
    // 소유한 캐릭터 뷰어를 생성하는 메서드
    private void CreateOwnedCharacterViewers()
    {
        while (CharacterManager.Instance.OwnedCharacters.Count > ownedCharacterViewers.Count) // 개수가 모자라면 채움
        {
            GameObject viewer = Instantiate(ownedCharacterViewers[0], ownedCharacterViewerParent);
            ownedCharacterViewers.Add(viewer);
        }
        for (int i = 0; i < ownedCharacterViewers.Count; i++)
        {
            if (i < CharacterManager.Instance.OwnedCharacters.Count)
            {
                var characterData = CharacterManager.Instance.OwnedCharacters[i].CharacterData;
                ownedCharacterViewers[i].GetComponent<CharacterViewer>().SetCharacterData(characterData);
                ownedCharacterViewers[i].SetActive(true);
            }
            else
            {
                ownedCharacterViewers[i].SetActive(false);
            }
        }
    }
    // 소유하지 않은 캐릭터 뷰어를 생성하는 메서드
    private void CreateUnownedCharacterViewers()
    {
        while (unownedCharacters.Count > unownedCharacterViewers.Count) // 개수가 모자라면 채움
        {
            GameObject viewer = Instantiate(unownedCharacterViewers[0], unownedCharacterViewerParent);
            unownedCharacterViewers.Add(viewer);
        }
        for (int i = 0; i < unownedCharacterViewers.Count; i++)
        {
            if (i < unownedCharacters.Count)
            {
                var characterData = unownedCharacters[i];
                unownedCharacterViewers[i].GetComponent<CharacterViewer>().SetCharacterData(characterData);
                unownedCharacterViewers[i].SetActive(true);
            }
            else
            {
                unownedCharacterViewers[i].SetActive(false);
            }
        }
    }
    public void OpenCharacterInfoPopup(LobbyCharacter character)
    {
        characterListPopup.SetActive(false);
        characterInfoPopup.SetActive(true);
        characterStandingImage.sprite = character.CharacterData.Standing; // 캐릭터 풀바디 이미지 설정
        OnClickSelectPopup(popupState);
        InfoPopupRefresh(character);
        LevelUpPopupRefresh(character);
        SkillInfoPopupRefresh(character);
    }
    
    public void OnClickSelectPopup(int state)
    {
        popupState = state;
        // 버튼 색상 변경
        for (int i = 0; i < popupButtons.Length; i++)
        {
            if (i == state)
            {
                popupButtons[i].GetComponent<Image>().color = selectedButtonColor;
            }
            else
            {
                popupButtons[i].GetComponent<Image>().color = unselectedButtonColor;
            }
        }
        basicInfoPopup.SetActive(state == 0);
        levelUpPopup.SetActive(state == 1);
        skillInfoPopup.SetActive(state == 2);
    }
    // ---------- 캐릭터 정보 팝업 관련 메서드---------------
    private void InfoPopupRefresh(LobbyCharacter character)
    {
        selectedCharacter = character;
        characterNameText.text = character.CharacterData.nameKr;
        characterlevelText.text = "Lv." + character.Level.ToString();
        characterClassTypeImage.sprite = character.CharacterData.RoleIcons;
        characterClassText.text = character.CharacterData.classType.ToString();
        characterElementTypeImage.sprite = character.CharacterData.elementIcon;
        characterElementText.text = character.CharacterData.elementType.ToString();
        characterSignatureDiceImage.sprite = character.CharacterData.DiceNumIcon;
        characterSignatureDiceText.text = character.CharacterData.charDiceData.CignatureFace.ToString();
        characterAffectionText.text = "1";
        characterHp.text = character.RegularHP.ToString();
        characterAtk.text = character.RegularATK.ToString();
        characterDef.text = character.RegularDEF.ToString();
        characterCritChance.text = (character.CritChance * 100).ToString("F1") + "%";
        characterCritDamage.text = (character.CritDamage * 100).ToString("F1") + "%";
        characterElimentalDamage.text = (character.CharacterData.elementDMG * 100).ToString("F1") + "%";
    }
    // ------------ 캐릭터 레벨업 팝업 관련 메서드------------
    private void LevelUpPopupRefresh(LobbyCharacter character)
    {
        RefreshPotionButtons();
        virtualLevel = 0; // 가상 레벨 초기화
        AddVirtualExp(character); // 가상 경험치 추가
        virtualATK = character.RegularATK + (virtualLevel * character.CharacterData.plusATK);
        virtualDEF = character.RegularDEF + (virtualLevel * character.CharacterData.plusDEF);
        virtualHP = character.RegularHP + (virtualLevel * character.CharacterData.plusHP);

        currentLevelText.text = $"Lv. {character.Level}";
        virtualLevelText.text = $"Lv. {character.Level + virtualLevel}";
        currentHPText.text = $"{character.RegularHP}";
        virtualHPText.text = $"{virtualHP}";
        currentATKText.text = $"{character.RegularATK}";
        virtualATKText.text = $"{virtualATK}";
        currentDEFText.text = $"{character.RegularDEF}";
        virtualDEFText.text = $"{virtualDEF}";

        // 레벨업 포션 개수 초기화
        lowLevelUpPotionAmountText.text = ItemManager.Instance.OwnedItems.
            TryGetValue(lowLevelUpPotion.ItemID, out var lowAmount) ? "x" + lowAmount.ToString() : "x0";
        lowLevelUpPotionConsumeText.text = lowLevelUpPotionConsumeAmount.ToString();
        midLevelUpPotionAmountText.text = ItemManager.Instance.OwnedItems.
            TryGetValue(midLevelUpPotion.ItemID, out var midAmount) ? "x" + midAmount.ToString() : "x0";
        midLevelUpPotionConsumeText.text = midLevelUpPotionConsumeAmount.ToString();
        highLevelUpPotionAmountText.text = ItemManager.Instance.OwnedItems.
            TryGetValue(highLevelUpPotion.ItemID, out var highAmount) ? "x" + highAmount.ToString() : "x0";
        highLevelUpPotionConsumeText.text = highLevelUpPotionConsumeAmount.ToString();
        royalLevelUpPotionAmountText.text = ItemManager.Instance.OwnedItems.
            TryGetValue(royalLevelUpPotion.ItemID, out var royalAmount) ? "x" + royalAmount.ToString() : "x0";
        royalLevelUpPotionConsumeText.text = royalLevelUpPotionConsumeAmount.ToString();
    }
    private void AddVirtualExp(LobbyCharacter character)
    {
        virtualCurrentExp = character.CurrentExp + virtualExp; // 캐릭터 현재 경험치 + 가상 경험치
        while (virtualCurrentExp >= GetExpToNextLevel(character))
        {
            virtualCurrentExp -= GetExpToNextLevel(character);
            virtualLevel++;
        }
        // 가상 경험치 슬라이더 갱신
        int expToNextLevel = GetExpToNextLevel(character);
        characterExpSlider.value = (float)virtualCurrentExp / expToNextLevel;
        virtualExpText.text = $"{virtualCurrentExp} / {expToNextLevel}";
    }
    private int GetExpToNextLevel(LobbyCharacter character)
    {
        return 250 * (character.Level + virtualLevel + 1);
    }

    public void OnClickLevelUpPotionCountUpButton(int potionType)
    {
        // 포션 타입에 따라 개수 감소
        switch (potionType)
        {
            case 0: // 낮은 레벨업 포션
                lowLevelUpPotionConsumeAmount++;
                if (ItemManager.Instance.OwnedItems.TryGetValue(lowLevelUpPotion.ItemID, out var lowAmount))
                {
                    if (lowLevelUpPotionConsumeAmount >= lowAmount)
                    {
                        lowLevelUpPotionConsumeAmount = lowAmount;
                    }
                }
                else
                {
                    lowLevelUpPotionConsumeAmount = 0;
                }
                break;
            case 1: // 중간 레벨업 포션
                midLevelUpPotionConsumeAmount++;
                if (ItemManager.Instance.OwnedItems.TryGetValue(midLevelUpPotion.ItemID, out var midAmount))
                {
                    if (midLevelUpPotionConsumeAmount >= midAmount)
                    {
                        midLevelUpPotionConsumeAmount = midAmount;
                    }
                }
                else
                {
                    midLevelUpPotionConsumeAmount = 0;
                }
                break;
            case 2: // 높은 레벨업 포션
                highLevelUpPotionConsumeAmount++;
                if (ItemManager.Instance.OwnedItems.TryGetValue(highLevelUpPotion.ItemID, out var highAmount))
                {
                    if (highLevelUpPotionConsumeAmount >= highAmount)
                    {
                        highLevelUpPotionConsumeAmount = highAmount;
                    }
                }
                else
                {
                    highLevelUpPotionConsumeAmount = 0;
                }
                break;
            case 3: // 로열 레벨업 포션
                royalLevelUpPotionConsumeAmount++;
                if (ItemManager.Instance.OwnedItems.TryGetValue(royalLevelUpPotion.ItemID, out var royalAmount))
                {
                    if (royalLevelUpPotionConsumeAmount >= royalAmount)
                    {
                        royalLevelUpPotionConsumeAmount = royalAmount;
                    }
                }
                else
                {
                    royalLevelUpPotionConsumeAmount = 0;
                }
                break;
        }
        // 레벨업 팝업 갱신
        RefreshPotionButtons();
        LevelUpPopupRefresh(selectedCharacter);
    }

    public void OnClickLevelUpPotionCountDownButton(int potionType)
    {
        // 포션 타입에 따라 개수 감소
        switch (potionType)
        {
            case 0: // 낮은 레벨업 포션
                if (lowLevelUpPotionConsumeAmount > 0)
                {
                    lowLevelUpPotionConsumeAmount--;
                }
                break;
            case 1: // 중간 레벨업 포션
                if (midLevelUpPotionConsumeAmount > 0)
                {
                    midLevelUpPotionConsumeAmount--;
                }
                break;
            case 2: // 높은 레벨업 포션
                if (highLevelUpPotionConsumeAmount > 0)
                {
                    highLevelUpPotionConsumeAmount--;
                }
                break;
            case 3: // 로열 레벨업 포션
                if (royalLevelUpPotionConsumeAmount > 0)
                {
                    royalLevelUpPotionConsumeAmount--;
                }
                break;
        }
        // 레벨업 팝업 갱신
        RefreshPotionButtons();
        LevelUpPopupRefresh(selectedCharacter);
    }
    private void RefreshPotionButtons()
    {
        if (lowLevelUpPotionConsumeAmount > 0)
        {
            lowLevelUpPotionCountDownButton.SetActive(true);
            if(lowLevelUpPotionConsumeAmount > (ItemManager.Instance.OwnedItems.TryGetValue(lowLevelUpPotion.ItemID, out var lowLevelAmount) ? lowLevelAmount : 0))
            {
                lowLevelUpPotionConsumeAmount = (ItemManager.Instance.OwnedItems.TryGetValue(lowLevelUpPotion.ItemID, out var amount) ? amount : 0);
                if (lowLevelUpPotionConsumeAmount == 0)
                {
                    lowLevelUpPotionCountDownButton.SetActive(false); // 개수가 0이면 버튼 비활성화
                }
            }
        }
        else
        {
            lowLevelUpPotionCountDownButton.SetActive(false);
            lowLevelUpPotionConsumeAmount = 0; // 개수가 0이면 초기화
        }

        if (midLevelUpPotionConsumeAmount > 0)
        {
            midLevelUpPotionCountDownButton.SetActive(true);
            if (midLevelUpPotionConsumeAmount > (ItemManager.Instance.OwnedItems.TryGetValue(midLevelUpPotion.ItemID, out var midLevelAmount) ? midLevelAmount : 0))
            {
                midLevelUpPotionConsumeAmount = (ItemManager.Instance.OwnedItems.TryGetValue(midLevelUpPotion.ItemID, out var amount) ? amount : 0);
                if (midLevelUpPotionConsumeAmount == 0)
                {
                    midLevelUpPotionCountDownButton.SetActive(false); // 개수가 0이면 버튼 비활성화
                }
            }
        }
        else
        {
            midLevelUpPotionCountDownButton.SetActive(false);
            midLevelUpPotionConsumeAmount = 0; // 개수가 0이면 초기화
        }

        if (highLevelUpPotionConsumeAmount > 0)
        {
            highLevelUpPotionCountDownButton.SetActive(true);
            if (highLevelUpPotionConsumeAmount > (ItemManager.Instance.OwnedItems.TryGetValue(highLevelUpPotion.ItemID, out var highLevelAmount) ? highLevelAmount : 0))
            {
                highLevelUpPotionConsumeAmount = ItemManager.Instance.OwnedItems.TryGetValue(highLevelUpPotion.ItemID, out var amount) ? amount : 0;
                if (highLevelUpPotionConsumeAmount == 0)
                {
                    highLevelUpPotionCountDownButton.SetActive(false); // 개수가 0이면 버튼 비활성화
                }
            }
        }
        else
        {
            highLevelUpPotionCountDownButton.SetActive(false);
            highLevelUpPotionConsumeAmount = 0; // 개수가 0이면 초기화
        }

        if (royalLevelUpPotionConsumeAmount > 0)
        {
            royalLevelUpPotionCountDownButton.SetActive(true);
            if (royalLevelUpPotionConsumeAmount > (ItemManager.Instance.OwnedItems.TryGetValue(royalLevelUpPotion.ItemID, out var royalLevelAmount) ? royalLevelAmount : 0))
            {
                royalLevelUpPotionConsumeAmount = ItemManager.Instance.OwnedItems.TryGetValue(royalLevelUpPotion.ItemID, out var amount) ? amount : 0;
                if (royalLevelUpPotionConsumeAmount == 0)
                {
                    royalLevelUpPotionCountDownButton.SetActive(false); // 개수가 0이면 버튼 비활성화
                }
            }
        }
        else
        {
            royalLevelUpPotionCountDownButton.SetActive(false);
            royalLevelUpPotionConsumeAmount = 0; // 개수가 0이면 초기화
        }
    }

    public void OnClickLevelUpButton()
    {
        // 레벨업 포션 사용 로직
        int totalExp = (lowLevelUpPotionConsumeAmount * lowLevelUpPotion.ExpAmount) +
                       (midLevelUpPotionConsumeAmount * midLevelUpPotion.ExpAmount) +
                       (highLevelUpPotionConsumeAmount * highLevelUpPotion.ExpAmount) +
                       (royalLevelUpPotionConsumeAmount * royalLevelUpPotion.ExpAmount);
        if (totalExp > 0)
        {
            selectedCharacter.AddExp(totalExp);
            LevelUpPopupRefresh(selectedCharacter);
        }
        // 소모량 만큼 포션 아이템 제거
        ItemManager.Instance.GetItem(lowLevelUpPotion.ItemID, -lowLevelUpPotionConsumeAmount);
        ItemManager.Instance.GetItem(midLevelUpPotion.ItemID, -midLevelUpPotionConsumeAmount);
        ItemManager.Instance.GetItem(highLevelUpPotion.ItemID, -highLevelUpPotionConsumeAmount);
        ItemManager.Instance.GetItem(royalLevelUpPotion.ItemID, -royalLevelUpPotionConsumeAmount);

        lowLevelUpPotionConsumeAmount = 0; // 사용 후 개수 초기화
        midLevelUpPotionConsumeAmount = 0; // 사용 후 개수 초기화
        highLevelUpPotionConsumeAmount = 0; // 사용 후 개수 초기화
        royalLevelUpPotionConsumeAmount = 0; // 사용 후 개수 초기화
        RefreshPotionButtons(); // 버튼 갱신
        LevelUpPopupRefresh(selectedCharacter); // 레벨업 팝업 갱신
    }
    public void OnPotionButtonPointerDown_LowUp() { OnPotionButtonPointerDown(0, true); }
    public void OnPotionButtonPointerDown_LowDown() { OnPotionButtonPointerDown(0, false); }
    public void OnPotionButtonPointerDown_MidUp() { OnPotionButtonPointerDown(1, true); }
    public void OnPotionButtonPointerDown_MidDown() { OnPotionButtonPointerDown(1, false); }
    public void OnPotionButtonPointerDown_HighUp() { OnPotionButtonPointerDown(2, true); }
    public void OnPotionButtonPointerDown_HighDown() { OnPotionButtonPointerDown(2, false); }
    public void OnPotionButtonPointerDown_RoyalUp() { OnPotionButtonPointerDown(3, true); }
    public void OnPotionButtonPointerDown_RoyalDown() { OnPotionButtonPointerDown(3, false); }
    public void OnPotionButtonPointerDown(int potionType, bool countUp)
    {
        isPotionButtonHeld = true;
        potionButtonHoldTime = 0f;
        repeatTimer = 0f;
        holdPotionType = potionType;
        isCountUp = countUp;
    }

    public void OnPotionButtonPointerUp()
    {
        if (isPotionButtonHeld && potionButtonHoldTime < holdThreshold)
        {
            // 짧게 눌렀을 때는 한 번만 동작
            if (isCountUp)
                OnClickLevelUpPotionCountUpButton(holdPotionType);
            else
                OnClickLevelUpPotionCountDownButton(holdPotionType);
        }
        isPotionButtonHeld = false;
        holdPotionType = -1;
    }

    // ------------ 캐릭터 스킬 정보 팝업 관련 메서드-----------
    private void SkillInfoPopupRefresh(LobbyCharacter character)
    {
        if (character == null || character.CharacterData == null)
        {
            Debug.LogError("Character or CharacterData is null in SkillInfoPopupRefresh.");
            return;
        }
        if (skillInfoState == 0)
        {
            selectActiveButton.SetActive(true);
            selectPassiveButton.SetActive(false);
        }
        else if (skillInfoState == 1)
        {
            selectActiveButton.SetActive(false);
            selectPassiveButton.SetActive(true);
        }
        else
        {
            Debug.LogError("Invalid skillInfoState: " + skillInfoState);
            return;
        }
        activeSkillIconImage.sprite = character.CharacterData.activeSO.SkillIcon;
        passiveSkillIconImage.sprite = character.CharacterData.passiveSO.SkillIcon;
        if (skillInfoState == 0) // 액티브 스킬
        {
            skillNameText[0].text = character.ActiveSkill.SkillNameKr;
            skillNameText[1].text = character.ActiveSkill.SkillNameKr;
            string activeBeforeDescriptionText = character.CharacterData.activeSO.SkillDescription;
            Dictionary<string, string> activeReplacements = new Dictionary<string, string>
            {
                { "{Skill_Value}", character.SkillValueA.ToString("F1") },
                { "{Buff_Probability}", (character.BuffProbabilityA * 100).ToString("F1") + "%" },
                { "{Buff_Value}", character.BuffValueA.ToString("F1") }
            };
            skillBeforeDescriptionText.text = ReplaceSkillDescription(activeBeforeDescriptionText, activeReplacements);
            string activeAfterDescriptionText = character.CharacterData.activeSO.SkillDescription;
            if (character.SkillLevelA <= 4)
            {
                activeReplacements = new Dictionary<string, string>
                {
                    { "{Skill_Value}", (character.SkillValueA + character.ActiveSkill.PlusSkillValue).ToString("F1") },
                    { "{Buff_Probability}", (character.BuffProbabilityA + character.ActiveSkill.PlusBuffProbability).ToString("F1") + "%" },
                    { "{Buff_Value}", (character.BuffValueA + character.ActiveSkill.PlusBuffValue).ToString("F1") }
                };
            }
            skillAfterDescriptionText.text = ReplaceSkillDescription(activeAfterDescriptionText, activeReplacements);
            
            skillBeforeLevelText.text = "Lv." + character.SkillLevelA.ToString();
            skillAfterLevelText.text = "Lv." + (character.SkillLevelA + 1).ToString();

            skillBeforeCostText.text = character.ActiveSkill.SkillCost.ToString();
            skillAfterCostText.text = character.ActiveSkill.SkillCost.ToString();

            foreach (Image image in selectedSkillIconImage)
            {
                image.sprite = character.ActiveSkill.SkillIcon;
            }
            foreach (TMP_Text text in skillCooldownText)
            {
                text.text = $"쿨타임 {character.ActiveSkill.CoolTime}턴";
            }

            Dictionary<SkillBookType, int> skillBookCost = SkillUpgradeChecker.SkillBookRequirements.TryGetValue(
                character.SkillLevelA,
                out var costs) ? costs : new Dictionary<SkillBookType, int>();
            string lowSkillBookCost = costs.TryGetValue(SkillBookType.Low, out int lowCost) ? lowCost.ToString() : "0";
            string midSkillBookCost = costs.TryGetValue(SkillBookType.Middle, out int midCost) ? midCost.ToString() : "0";
            string highSkillBookCost = costs.TryGetValue(SkillBookType.High, out int highCost) ? highCost.ToString() : "0";
            int lowSkillBookAmount = ItemManager.Instance.OwnedItems.TryGetValue(lowSkillBook.ItemID, out var lowAmount) ? lowAmount : 0;
            int midSkillBookAmount = ItemManager.Instance.OwnedItems.TryGetValue(midSkillBook.ItemID, out var midAmount) ? midAmount : 0;
            int highSkillBookAmount = ItemManager.Instance.OwnedItems.TryGetValue(highSkillBook.ItemID, out var highAmount) ? highAmount : 0;
            lowSkillBookCostText.text = $"{lowSkillBookAmount} / {lowSkillBookCost}";
            midSkillBookCostText.text = $"{midSkillBookAmount} / {midSkillBookCost}";
            highSkillBookCostText.text = $"{highSkillBookAmount} / {highSkillBookCost}";

            skillLevelUpGoldCostText.text = skillLevelUpGoldCost[character.SkillLevelA - 1].ToString();
        }
        else if(skillInfoState == 1) // 패시브 스킬
        {
            skillNameText[0].text = character.PassiveSkill.SkillNameKr;
            skillNameText[1].text = character.PassiveSkill.SkillNameKr;
            string passiveDescriptionText = character.CharacterData.passiveSO.SkillDescription;
            Dictionary<string, string> passiveReplacements = new Dictionary<string, string>
            {
                { "{Skill_Value}", character.SkillValueB.ToString("F1") },
                { "{Buff_Probability}", (character.BuffProbabilityB * 100).ToString("F1") + "%" },
                { "{Buff_Value}", character.BuffValueB.ToString("F1") }
            };
            skillBeforeDescriptionText.text = ReplaceSkillDescription(passiveDescriptionText, passiveReplacements);
            string passiveAfterDescriptionText = character.CharacterData.passiveSO.SkillDescription;
            if (character.SkillLevelB <= 4)
            {
                passiveReplacements = new Dictionary<string, string>
                {
                    { "{Skill_Value}", (character.SkillValueB + character.PassiveSkill.PlusSkillValue).ToString("F1") },
                    { "{Buff_Probability}", (character.BuffProbabilityB + character.PassiveSkill.PlusBuffProbability).ToString("F1") + "%" },
                    { "{Buff_Value}", (character.BuffValueB + character.PassiveSkill.PlusBuffValue).ToString("F1") }
                };
            }
            skillAfterDescriptionText.text = ReplaceSkillDescription(passiveAfterDescriptionText, passiveReplacements);
            
            skillBeforeLevelText.text = "Lv." + character.SkillLevelB.ToString();
            skillAfterLevelText.text = "Lv." + (character.SkillLevelB + 1).ToString();
            skillBeforeCostText.text = "";
            skillAfterCostText.text = "";
            foreach (Image image in selectedSkillIconImage)
            {
                image.sprite = character.PassiveSkill.SkillIcon;
            }
            foreach (TMP_Text text in skillCooldownText)
            {
                text.text = $"쿨타임 없음";
            }
            Dictionary<SkillBookType, int> skillBookCost = SkillUpgradeChecker.SkillBookRequirements.TryGetValue(
                character.SkillLevelB,
                out var costs) ? costs : new Dictionary<SkillBookType, int>();
            string lowSkillBookCost = costs.TryGetValue(SkillBookType.Low, out int lowCost) ? lowCost.ToString() : "0";
            string midSkillBookCost = costs.TryGetValue(SkillBookType.Middle, out int midCost) ? midCost.ToString() : "0";
            string highSkillBookCost = costs.TryGetValue(SkillBookType.High, out int highCost) ? highCost.ToString() : "0";
            int lowSkillBookAmount = ItemManager.Instance.OwnedItems.TryGetValue(lowSkillBook.ItemID, out var lowAmount) ? lowAmount : 0;
            int midSkillBookAmount = ItemManager.Instance.OwnedItems.TryGetValue(midSkillBook.ItemID, out var midAmount) ? midAmount : 0;
            int highSkillBookAmount = ItemManager.Instance.OwnedItems.TryGetValue(highSkillBook.ItemID, out var highAmount) ? highAmount : 0;

            lowSkillBookCostText.text = $"{lowSkillBookAmount} / {lowSkillBookCost}";
            midSkillBookCostText.text = $"{midSkillBookAmount} / {midSkillBookCost}";
            highSkillBookCostText.text = $"{highSkillBookAmount} / {highSkillBookCost}";

            skillLevelUpGoldCostText.text = skillLevelUpGoldCost[character.SkillLevelB - 1].ToString();
        }
    }

    private string ReplaceSkillDescription(string description, Dictionary<string, string> replacements)
    {
        foreach (var kvp in replacements)
        {
            description = Regex.Replace(description, Regex.Escape(kvp.Key), kvp.Value);
        }
        return description;
    }

    public void OnClickSkillInfoStateButton(int state)
    {
        skillInfoState = state;
        SkillInfoPopupRefresh(selectedCharacter);
    }
}

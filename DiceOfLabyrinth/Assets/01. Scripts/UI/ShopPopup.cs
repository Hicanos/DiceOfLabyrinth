using Helios.GUI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : MonoBehaviour
{
    public static ShopPopup Instance { get; private set; }

    private int selectedArtifactIndexInOwnedList = -1; // -1은 아무것도 선택되지 않았음을 나타냄
    private int selectedArtifactIndexInShopList = -1; // -1은 아무것도 선택되지 않았음을 나타냄

    [SerializeField] private AnimationRect animationRect;
    [SerializeField] private MessagePopup messagePopup;

    [Header("Shop Panel")]
    [SerializeField] private int baseResetCost = 20;
    [SerializeField] private int resetCost;
    [SerializeField] private int maxResetCost = 80;

    [Header("Owned Artifact")]
    [SerializeField] private GameObject[] ownedArtifactIcons = new GameObject[12];
    [SerializeField] private GameObject[] ownedArtifactRarities = new GameObject[12];

    [Header("OwnedArtifactDescriptions")]
    [SerializeField] private TMP_Text ownedArtifactNameText;
    [SerializeField] private TMP_Text ownedArtifactSetEffectText;
    [SerializeField] private TMP_Text ownedArtifactDescriptionText;
    [SerializeField] private TMP_Text sellPriceText;

    [Header("Artifact Icon Refresh Alpha")]
    [SerializeField, Range(0f, 1f)] private float selectedAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float unselectedAlpha = 0.5f;

    [Header("ShopArtifact")]
    [SerializeField] private List<ArtifactData> selectableArtifacts = new List<ArtifactData>(6); // 상점에서 선택 가능한 아티팩트 목록
    //[SerializeField] private List<ArtifactData> exceptedArtifacts = new List<ArtifactData>(); // 상점에서 제외할 아티팩트 목록//현재는 사용하지 않음
    [SerializeField] private List<GameObject> shopArtifactViewers = new List<GameObject>(6); // 상점 아티팩트 뷰어 목록

    [Header("ShopArtifactViewers")]
    [SerializeField] private TMP_Text[] shopArtifactViewerNameText = new TMP_Text[6];
    [SerializeField] private TMP_Text[] purchasePriceText = new TMP_Text[6];
    [SerializeField] private GameObject[] shopArtifactViewerIcons = new GameObject[6]; // 상점 아티팩트 아이콘
    [SerializeField] private GameObject[] shopArtifactViewerRarities = new GameObject[6]; // 상점 아티팩트 희귀도 아이콘

    [Header("ShopArtifactDescription")]
    [SerializeField] private TMP_Text shopArtifactNameText;
    [SerializeField] private TMP_Text shopArtifactSetEffectText;
    [SerializeField] private TMP_Text shopArtifactDescriptionText;
    [SerializeField] private TMP_Text shopArtifactPurchasePriceText;

    [Header("Reset Text")]
    [SerializeField] private TMP_Text resetCostText;

    [Header("Recovery Popup")]
    [SerializeField] private GameObject recoveryPopup;
    //private int resetCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        // 상점 패널이 활성화될 때 초기화
        StartShop();
        animationRect.AnimLeftIn();
        animationRect.AnimRightIn();
    }

    public void StartShop()
    {
        selectedArtifactIndexInOwnedList = -1; // 초기화
        selectedArtifactIndexInShopList = -1; // 초기화
        OwnedArtifactRefresh(); //소유한 아티팩트 갱신
        //exceptedArtifacts.Clear(); // 상점에서 제외할 아티팩트 목록 초기화, // 현재는 사용하지 않음
        ShopArtifactRefresh(); //상점 아티팩트 갱신
        resetCost = baseResetCost;
        ResetButtonRefresh();
    }

    private void OwnedArtifactRefresh()
    {
        for (int i = 0; i < 12; i++)
        {
            if (StageManager.Instance.stageSaveData.artifacts[i] != null)
            {
                ArtifactData artifact = StageManager.Instance.stageSaveData.artifacts[i];
                ownedArtifactIcons[i].SetActive(true);
                ownedArtifactIcons[i].GetComponent<UnityEngine.UI.Image>().sprite = artifact.Icon;
                ownedArtifactRarities[i].SetActive(true);
                ownedArtifactRarities[i].GetComponent<UnityEngine.UI.Image>().sprite = artifact.RaritySprite;
            }
            else
            {
                ownedArtifactIcons[i].SetActive(false);
                ownedArtifactRarities[i].SetActive(false);
            }
        }
    }
    private void ShopArtifactRefresh()
    {
        if(StageManager.Instance.stageSaveData.currentChapterIndex == -1)
        {
            messagePopup.Open("챕터가 선택되지 않아서 상점 아티팩트 데이터를 불러올 수 없습니다.");
            return; // 챕터가 선택되지 않은 경우 리턴
        }
        List<ArtifactData> shopInventoryArtifacts = StageManager.Instance.chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex]
            .stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].ArtifactList.ToList(); 
        //shopInventoryArtifacts.RemoveAll(artifact => exceptedArtifacts.Contains(artifact)); // 제외할 아티팩트 제거
        shopInventoryArtifacts.RemoveAll(artifact => StageManager.Instance.stageSaveData.artifacts.Contains(artifact)); // 이미 소유한 아티팩트 제거
        shopInventoryArtifacts.RemoveAll(artifact => StageManager.Instance.stageSaveData.equipedArtifacts.Contains(artifact)); // 장착된 아티팩트 제거
        // 리스트에서 랜덤 6개 선택
        selectableArtifacts.Clear();
        for (int i = 0; i < 6 && shopInventoryArtifacts.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, shopInventoryArtifacts.Count);
            selectableArtifacts.Add(shopInventoryArtifacts[randomIndex]);
            shopInventoryArtifacts.RemoveAt(randomIndex);
        }
        // 선택된 아티팩트로 상점 뷰어 갱신
        for (int i = 0; i < shopArtifactViewers.Count; i++)
        {
            if (i < selectableArtifacts.Count)
            {
                ArtifactData artifact = selectableArtifacts[i];
                shopArtifactViewers[i].SetActive(true);
                shopArtifactViewerNameText[i].text = artifact.ArtifactName;
                purchasePriceText[i].text = $"{artifact.PurchasePrice}";
                shopArtifactViewerIcons[i].GetComponent<UnityEngine.UI.Image>().sprite = artifact.Icon;
                shopArtifactViewerRarities[i].GetComponent<UnityEngine.UI.Image>().sprite = artifact.RaritySprite;
            }
            else
            {
                if (shopArtifactViewers[i] != null)
                {
                    shopArtifactViewers[i].SetActive(false); // 선택된 아티팩트가 없으면 뷰어 비활성화
                }
            }
        }
    }
    public void OnClickShopArtifactSlot(int index)
    {
        if (index < 0 || index >= selectableArtifacts.Count)
        {
            return; // 유효하지 않은 인덱스면 리턴
        }
        if (selectableArtifacts[index] == null)
        {
            return;
        }
        selectedArtifactIndexInShopList = index; // 선택한 슬롯 인덱스 저장
        ShopArtifactIconRefresh(index);
        ShopArtifactDescriptionRefresh(index);
    }
    public void OnClickOwnedArtifactSlot(int index)
    {
        if (index < 0 || index >= ownedArtifactIcons.Length)
        {
            return; // 유효하지 않은 인덱스면 리턴
        }
        if (StageManager.Instance.stageSaveData.artifacts[index] == null)
        {
            return;
        }
        selectedArtifactIndexInOwnedList = index; // 선택한 슬롯 인덱스 저장
        OwnedArtifactIconRefresh(index);
        OwnedArtifactDescriptionRefresh(index);
    }

    private void ShopArtifactIconRefresh(int slotIndex)
    {
        for (int i = 0; i < 6; i++)
        {
            var canvasGroup = shopArtifactViewers[i].GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = shopArtifactViewers[i].AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = (i == slotIndex) ? selectedAlpha : unselectedAlpha;
        }
    }
    private void ShopArtifactDescriptionRefresh(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= selectableArtifacts.Count)
        {
            return; // 유효하지 않은 인덱스면 리턴
        }
        var artifactInSlot = selectableArtifacts[slotIndex];
        if (artifactInSlot == null)
        {
            shopArtifactNameText.text = "";
            shopArtifactSetEffectText.text = "";
            shopArtifactDescriptionText.text = "";
            shopArtifactPurchasePriceText.text = "0"; // 초기화
            return; // 선택한 슬롯에 아티팩트가 없으면 리턴
        }
        shopArtifactNameText.text = artifactInSlot.ArtifactName;
        shopArtifactSetEffectText.text = string.Join(" ", artifactInSlot.SetEffectData.ConvertAll(setEffect => $"#{setEffect.EffectName}"));
        shopArtifactDescriptionText.text = artifactInSlot.Description;
        shopArtifactPurchasePriceText.text = $"{artifactInSlot.PurchasePrice}"; // 구매 가격 표시
        if (StageManager.Instance.stageSaveData.manaStone < artifactInSlot.PurchasePrice)
        {
            shopArtifactPurchasePriceText.color = Color.red; // 마석이 부족하면 빨간색으로 표시
        }
        else
        {
            shopArtifactPurchasePriceText.color = Color.white; // 충분하면 흰색으로 표시
        }
    }
    private void OwnedArtifactIconRefresh(int slotIndex)
    {
        for (int i = 0; i < 12; i++)
        {
            if (i == slotIndex)
            {
                ownedArtifactIcons[i].GetComponent<CanvasGroup>().alpha = selectedAlpha;
                ownedArtifactRarities[i].GetComponent<CanvasGroup>().alpha = selectedAlpha;
            }
            else
            {
                ownedArtifactIcons[i].GetComponent<CanvasGroup>().alpha = unselectedAlpha;
                ownedArtifactRarities[i].GetComponent<CanvasGroup>().alpha = unselectedAlpha;
            }
        }
    }
    private void OwnedArtifactDescriptionRefresh(int slotIndex)
    {
        if (StageManager.Instance.stageSaveData.artifacts[slotIndex] != null)
        {
            var artifactInSlot = StageManager.Instance.stageSaveData.artifacts[slotIndex];
            string setEffectText = "";
            for (int i = 0; i < artifactInSlot.SetEffectData.Count; i++)
            {
                setEffectText += $"#{artifactInSlot.SetEffectData[i].EffectName} ";
            }
            ownedArtifactNameText.text = artifactInSlot.ArtifactName;
            this.ownedArtifactSetEffectText.text = setEffectText;
            ownedArtifactDescriptionText.text = artifactInSlot.Description;
            sellPriceText.text = $"{artifactInSlot.SellPrice}"; // 판매 가격 표시
        }
        else
        {
            ownedArtifactNameText.text = "";
            ownedArtifactSetEffectText.text = "";
            ownedArtifactDescriptionText.text = "";
            sellPriceText.text = "0"; // 판매 가격 초기화
        }
    }
    public void OnClickResetButton()
    {
        if(StageManager.Instance.stageSaveData.manaStone < resetCost)
        {
            messagePopup.Open("마석이 부족합니다.");
            return; // 마석이 부족하면 리턴
        }
        //if (resetCount >= 3)
        //{
        //    messagePopup.Open("아티팩트 리셋은 3번만 가능합니다.");
        //    return; // 3번만 리셋 가능
        //}
        StageManager.Instance.stageSaveData.manaStone -= resetCost; // 마석 차감
        resetCost = Mathf.Min(resetCost * 2, maxResetCost); // 리셋 비용 증가, 최대값 제한
        //resetCount++;
        //foreach (var artifact in selectableArtifacts)
        //{
        //    exceptedArtifacts.Add(artifact); // 상점에서 제외할 아티팩트 목록에 추가
        //}
        ShopArtifactRefresh();
        ResetButtonRefresh();
        OnClickShopArtifactSlot(0); // 첫 번째 슬롯으로 초기화
    }
    private void ResetButtonRefresh()
    {
        resetCostText.text = $"{resetCost}";
        if (StageManager.Instance.stageSaveData.manaStone < resetCost)
        {
            resetCostText.color = Color.red; // 마석이 부족하면 빨간색으로 표시
        }
        else
        {
            resetCostText.color = Color.white; // 충분하면 흰색으로 표시
        }
    }
    public void OnClickSellButton()
    {
        if (selectedArtifactIndexInOwnedList < 0 || selectedArtifactIndexInOwnedList >= StageManager.Instance.stageSaveData.artifacts.Count)
        {
            messagePopup.Open("판매할 아티팩트를 선택해주세요.");
            return; // 유효하지 않은 인덱스면 리턴
        }
        if (StageManager.Instance.stageSaveData.artifacts[selectedArtifactIndexInOwnedList] == null)
        {
            messagePopup.Open("판매할 아티팩트가 없습니다.");
            return; // 선택한 슬롯에 아티팩트가 없으면 리턴
        }
        ArtifactData soldArtifact = StageManager.Instance.stageSaveData.artifacts[selectedArtifactIndexInOwnedList];
        int sellPrice = soldArtifact.SellPrice;
        StageManager.Instance.stageSaveData.manaStone += sellPrice; // 판매 가격만큼 마석 증가
        StageManager.Instance.stageSaveData.artifacts[selectedArtifactIndexInOwnedList] = null; // 해당 슬롯의 아티팩트 제거
        //스테이지 세이브 데이터의 아티팩트 리스트의 null 정렬
        int listSize = StageManager.Instance.stageSaveData.artifacts.Count;
        StageManager.Instance.stageSaveData.artifacts.RemoveAll(artifact => artifact == null);
        while (StageManager.Instance.stageSaveData.artifacts.Count < listSize)
        {
            StageManager.Instance.stageSaveData.artifacts.Add(null); // null로 채워서 리스트 크기 유지
        }
        OwnedArtifactRefresh();
        OnClickOwnedArtifactSlot(selectedArtifactIndexInOwnedList); // 선택한 슬롯의 아티팩트 정보 갱신
    }
    public void OnClickPurchaseButton()
    {
        if (selectedArtifactIndexInShopList < 0 || selectedArtifactIndexInShopList >= selectableArtifacts.Count)
        {
            messagePopup.Open("구매할 아티팩트를 선택해주세요.");
            return; // 유효하지 않은 인덱스면 리턴
        }
        ArtifactData selectedArtifact = selectableArtifacts[selectedArtifactIndexInShopList];
        if (StageManager.Instance.stageSaveData.manaStone < selectedArtifact.PurchasePrice)
        {
            messagePopup.Open("마석이 부족합니다.");
            return; // 마석이 부족하면 리턴
        }
        int emptySlot= -1; // 빈 슬롯 인덱스 초기화
        for (int i = 0; i < StageManager.Instance.stageSaveData.artifacts.Count; i++)
        {
            if (StageManager.Instance.stageSaveData.artifacts[i] == null)
            {
                emptySlot = i; // 빈 슬롯 발견
                break;
            }
        }
        if(emptySlot == -1)
        {
            messagePopup.Open("아티팩트 슬롯이 부족합니다. 슬롯을 비우고 다시 시도해주세요.");
            return; // 빈 슬롯이 없으면 리턴
        }
        StageManager.Instance.stageSaveData.manaStone -= selectedArtifact.PurchasePrice; // 마석 차감
        StageManager.Instance.stageSaveData.artifacts[emptySlot] = selectedArtifact; // 빈 슬롯에 아티팩트 추가
        OwnedArtifactRefresh(); // 소유한 아티팩트 갱신
        OnClickOwnedArtifactSlot(emptySlot); // 새로 추가된 아티팩트 정보 갱신
        //exceptedArtifacts.Add(selectedArtifact); // 상점에서 제외할 아티팩트 목록에 추가
        shopArtifactViewers[selectedArtifactIndexInShopList].SetActive(false); // 선택한 아티팩트 뷰어 비활성화
        OnClickShopArtifactSlot(selectedArtifactIndexInShopList - 1);
    }

    public void OnClickRecoveryPopupButton()
    {
        
        animationRect.CloseWithCallback(() =>
        {
            gameObject.SetActive(false);
        });
        recoveryPopup.SetActive(true);
    }

    public void OnClickCloseButton()
    {
        animationRect.CloseWithCallback(() =>
        {
            gameObject.SetActive(false);
            StageManager.Instance.stageSaveData.currentPhaseIndex = 5;
            StageManager.Instance.battleUIController.OpenStagePanel(5); // 스테이지 패널 열기
        });
    }
}

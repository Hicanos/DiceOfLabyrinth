using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopPanel : MonoBehaviour
{
    public static ShopPanel Instance { get; private set; }

    [Header("Shop Panel")]
    [SerializeField] private int baseResetCost = 20;
    [SerializeField] private int resetCost;
    [SerializeField] private int maxResetCost = 80;
    [Header("Owned Artifact")]
    private int selectedArtifactIndexInOwnedList = -1; // -1은 아무것도 선택되지 않았음을 나타냄
    [SerializeField] private GameObject[] artifactIcon = new GameObject[12];
    [SerializeField] private GameObject[] artifactRarity = new GameObject[12]; 
    [Header("OwnedArtifactDescriptions")]
    [SerializeField] private TMP_Text ownedArtifactNameText;
    [SerializeField] private TMP_Text ownedArtifactSetEffectText;
    [SerializeField] private TMP_Text ownedArtifactDescriptionText;
    [SerializeField] private TMP_Text sellPriceText;
    [Header("Artifact Icon Refresh Color")]
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor = new Color(1, 1, 1, 0.5f);
    [Header("ShopArtifact")]
    [SerializeField] private List<ArtifactData> shopInventoryArtifacts = new List<ArtifactData>(6);
    [SerializeField] private List<ArtifactData> exceptedArtifacts = new List<ArtifactData>(); // 상점에서 제외할 아티팩트 목록
    [SerializeField] private List<GameObject> shopArtifactViewers = new List<GameObject>(6);
    [Header("ShopArtifactDescriptions")]
    [SerializeField] private TMP_Text shopArtifactNameText;
    [SerializeField] private TMP_Text shopArtifactSetEffectText;
    [SerializeField] private TMP_Text shopArtifactDescriptionText;
    [SerializeField] private TMP_Text purchasePriceText;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void Refresh()
    {
        OwnedArtifactRefresh(); //소유한 아티팩트 갱신
        ShopArtifactRefresh(); //상점 아티팩트 갱신
        resetCost = baseResetCost;
        selectedArtifactIndexInOwnedList = -1; // 초기화
    }

    private void OwnedArtifactRefresh()
    {
        for (int i = 0; i < 12; i++)
        {
            if (StageManager.Instance.stageSaveData.artifacts[i] != null)
            {
                ArtifactData artifact = StageManager.Instance.stageSaveData.artifacts[i];
                artifactIcon[i].SetActive(true);
                artifactIcon[i].GetComponent<UnityEngine.UI.Image>().sprite = artifact.Icon;
                artifactRarity[i].SetActive(true);
                artifactRarity[i].GetComponent<UnityEngine.UI.Image>().sprite = artifact.RaritySprite;
            }
            else
            {
                artifactIcon[i].SetActive(false);
                artifactRarity[i].SetActive(false);
            }
        }
    }
    private void ShopArtifactRefresh()
    {
        // 상점 아티팩트 갱신 로직
        // 예: 상점에서 판매 중인 아티팩트 목록을 가져와 UI에 표시
    }
    public void OnClickOwnedArtifactSlot(int index)
    {
        if (StageManager.Instance.stageSaveData.artifacts[index] == null)
        {
            return; // 해당 슬롯에 아티팩트가 없으면 아무 작업도 하지 않음
        }
        selectedArtifactIndexInOwnedList = index; // 선택한 슬롯 인덱스 저장
        ArtifactIconRefresh(index);
        ArtifactDescriptionRefresh(index);
    }
    private void ArtifactIconRefresh(int slotIndex)
    {
        for (int i = 0; i < 12; i++)
        {
            if (i == slotIndex)
            {
                artifactIcon[i].GetComponent<UnityEngine.UI.Image>().color = selectedColor;
                artifactRarity[i].GetComponent<UnityEngine.UI.Image>().color = selectedColor;
            }
            else
            {
                artifactIcon[i].GetComponent<UnityEngine.UI.Image>().color = unselectedColor;
                artifactRarity[i].GetComponent<UnityEngine.UI.Image>().color = unselectedColor;
            }
        }
    }
    private void ArtifactDescriptionRefresh(int slotIndex)
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
            MessagePopup.Instance.Open("마석이 부족합니다.");
            return; // 마석이 부족하면 리턴
        }
        StageManager.Instance.stageSaveData.manaStone -= resetCost; // 마석 차감
        resetCost = Mathf.Min(resetCost * 2, maxResetCost); // 리셋 비용 증가, 최대값 제한
        ShopArtifactRefresh();
    }
    public void OnClickSellButton()
    {
        SellArtifact(); // 판매 버튼 클릭 시 아티팩트 판매 로직 호출
    }
    private void SellArtifact()
    {
        if (selectedArtifactIndexInOwnedList < 0 || selectedArtifactIndexInOwnedList >= StageManager.Instance.stageSaveData.artifacts.Count)
        {
            MessagePopup.Instance.Open("판매할 아티팩트를 선택해주세요.");
            return; // 유효하지 않은 인덱스면 리턴
        }
        if (StageManager.Instance.stageSaveData.artifacts[selectedArtifactIndexInOwnedList] == null)
        {
            MessagePopup.Instance.Open("판매할 아티팩트가 없습니다.");
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
}

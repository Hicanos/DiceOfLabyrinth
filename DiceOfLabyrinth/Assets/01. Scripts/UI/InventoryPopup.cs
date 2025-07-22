using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryPopup : MonoBehaviour
{
    [Header("InventoryPopup")]
    [SerializeField] private GameObject inventoryPopup;
    [SerializeField] private GameObject setEffectDescriptionPopup;
    [SerializeField] private GameObject setEffectPopupBg;

    [Header("ArtifactSlots")]
    [SerializeField] private GameObject[] artifactIcon = new GameObject[12];
    [SerializeField] private GameObject[] artifactRarity = new GameObject[12];

    [Header("EquipedArtifactSlots")]
    [SerializeField] private GameObject[] equipedArtifactIcon = new GameObject[4];
    [SerializeField] private GameObject[] equipedArtifactRarity = new GameObject[4];
    [SerializeField] private GameObject[] equipedArtifactRockIcon = new GameObject[4];


    [Header("EngravingSlots")]
    [SerializeField] private GameObject[] engravingIcon = new GameObject[3];
    [SerializeField] private GameObject[] engravingBg = new GameObject[3];

    [Header("ItemDescriptions")]
    [SerializeField] private GameObject itemIcon;
    [SerializeField] private GameObject itemRarity;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemTypeText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemFlavorText;

    [Header("SetEffectViewer")]
    [SerializeField] private GameObject createPositionObject;
    [SerializeField] private GameObject viewerPrefab;

    [Header("SetEffectDescriptionPopup")]
    public GameObject setEffectDescriptionPopupObject;
    public GameObject setEffectDescriptionPopupBg;
    public TMP_Text setEffectNameText;
    public GameObject setEffectIcon;
    public TMP_Text setEffectDescriptionText;
    public static InventoryPopup Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        if (inventoryPopup == null)
        {
            Debug.LogError("InventoryPopup is not assigned in the inspector.");
        }
        if (setEffectDescriptionPopup == null)
        {
            Debug.LogError("SetEffectDescriptionPopup is not assigned in the inspector.");
        }
        inventoryPopup.SetActive(false);
        setEffectDescriptionPopup.SetActive(false);
    }
    public void OnClickInventoryButton()
    {
        inventoryPopup.SetActive(true);
        setEffectDescriptionPopup.SetActive(false);
        setEffectPopupBg.SetActive(false);
        Refresh();
        OnClickArtifactSlot(0); // 0번 슬롯으로 초기화
    }
    public void OnClickCloseButton()
    {
        inventoryPopup.SetActive(false);
        setEffectDescriptionPopup.SetActive(false);
        setEffectPopupBg.SetActive(false);
    }

    private void Refresh()
    {
        ArtifactSlotRefresh();
        EquipedArtifactSlotRefresh();
        EngravingSlotRefresh();
        SetEffectViewerRefresh();
    }
    private void ArtifactSlotRefresh()// 아티팩트 슬롯 리프레시
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
    private void EquipedArtifactSlotRefresh()// 장착된 아티팩트 슬롯 리프레시
    {
        for (int i = 0; i < 4; i++)
        {
            if (StageManager.Instance.stageSaveData.equipedArtifacts[i] != null)
            {
                ArtifactData artifact = StageManager.Instance.stageSaveData.equipedArtifacts[i];
                equipedArtifactIcon[i].SetActive(true);
                equipedArtifactIcon[i].GetComponent<UnityEngine.UI.Image>().sprite = artifact.Icon;
                equipedArtifactRarity[i].SetActive(true);
                equipedArtifactRarity[i].GetComponent<UnityEngine.UI.Image>().sprite = artifact.RaritySprite;
                equipedArtifactRockIcon[i].SetActive(false);
            }
            else
            {
                equipedArtifactIcon[i].SetActive(false);
                equipedArtifactRarity[i].SetActive(false);
                equipedArtifactRockIcon[i].SetActive(true);
            }
        }
    }
    private void EngravingSlotRefresh()// 각인 슬롯 리프레시
    {
        for (int i = 0; i < 3; i++)
        {
            if (StageManager.Instance.stageSaveData.engravings[i] != null)
            {
                EngravingData engraving = StageManager.Instance.stageSaveData.engravings[i];
                engravingIcon[i].SetActive(true);
                engravingIcon[i].GetComponent<UnityEngine.UI.Image>().sprite = engraving.Icon;
                engravingBg[i].SetActive(true);
                engravingBg[i].GetComponent<UnityEngine.UI.Image>().sprite = engraving.BgSprite;
            }
            else
            {
                engravingIcon[i].SetActive(false);
                engravingBg[i].SetActive(false);
            }
        }
    }

    public void OnClickArtifactSlot(int slotIndex)
    {
        ArtifactDescriptionRefresh(slotIndex);
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
            itemIcon.GetComponent<UnityEngine.UI.Image>().sprite = artifactInSlot.Icon;
            itemRarity.GetComponent<UnityEngine.UI.Image>().sprite = artifactInSlot.RaritySprite;
            itemIcon.SetActive(true);
            itemNameText.text = artifactInSlot.ArtifactName;
            itemTypeText.text = setEffectText;
            itemDescriptionText.text = artifactInSlot.Description;
            itemFlavorText.text = artifactInSlot.FlavorText;
        }
        else
        {
            itemIcon.SetActive(false);
            itemNameText.text = "";
            itemTypeText.text = "";
            itemDescriptionText.text = "";
            itemFlavorText.text = "";
        }
    }

    public void OnClickEquipedArtifactSlot(int slotIndex)
    {
        EquipedArtifactDescriptionRefresh(slotIndex);
    }
    private void EquipedArtifactDescriptionRefresh(int slotIndex)
    {
        if (StageManager.Instance.stageSaveData.equipedArtifacts[slotIndex] != null)
        {
            var artifactInSlot = StageManager.Instance.stageSaveData.equipedArtifacts[slotIndex];
            string setEffectText = "";
            for (int i = 0; i < artifactInSlot.SetEffectData.Count; i++)
            {
                setEffectText += $"#{artifactInSlot.SetEffectData[i].EffectName} ";
            }
            itemIcon.GetComponent<UnityEngine.UI.Image>().sprite = artifactInSlot.Icon;
            itemRarity.GetComponent<UnityEngine.UI.Image>().sprite = artifactInSlot.RaritySprite;
            itemIcon.SetActive(true);
            itemNameText.text = artifactInSlot.ArtifactName;
            itemTypeText.text = setEffectText;
            itemDescriptionText.text = artifactInSlot.Description;
            itemFlavorText.text = artifactInSlot.FlavorText;
        }
        else
        {
            itemIcon.SetActive(false);
            itemNameText.text = "";
            itemTypeText.text = "";
            itemDescriptionText.text = "";
            itemFlavorText.text = "";
        }
    }

    public void OnClickEngravingSlot(int slotIndex)
    {
        EngravingDescriptionRefresh(slotIndex);
    }
    private void EngravingDescriptionRefresh(int slotIndex)
    {
        if (StageManager.Instance.stageSaveData.engravings[slotIndex] != null)
        {
            var EngravingInSlot = StageManager.Instance.stageSaveData.engravings[slotIndex];
            itemIcon.GetComponent<UnityEngine.UI.Image>().sprite = EngravingInSlot.Icon;
            itemRarity.GetComponent<UnityEngine.UI.Image>().sprite = EngravingInSlot.BgSprite;
            itemIcon.SetActive(true);
            itemNameText.text = EngravingInSlot.EngravingName;
            itemTypeText.text = "";
            itemDescriptionText.text = EngravingInSlot.Description;
            itemFlavorText.text = "";
        }
        else
        {
            itemIcon.SetActive(false);
            itemNameText.text = "";
            itemTypeText.text = "";
            itemDescriptionText.text = "";
            itemFlavorText.text = "";
        }
    }
    private void SetEffectViewerRefresh()
    {

        foreach (Transform child in createPositionObject.transform)
        {
            Destroy(child.gameObject);
        }
        var allArtifacts = new List<ArtifactData>();// artifacts와 equipedArtifacts를 합쳐서 하나의 리스트로 만듦
        allArtifacts.AddRange(StageManager.Instance.stageSaveData.artifacts);
        allArtifacts.AddRange(StageManager.Instance.stageSaveData.equipedArtifacts);
        // 세트 효과별로 카운트 집계
        Dictionary<string, (SetEffectData data, int count, string countText)> effectDict = new();
        foreach (var artifact in allArtifacts)
        {
            if (artifact == null) continue;
            foreach (var effect in artifact.SetEffectData)
            {
                string countText = "";
                List<int> counts = new List<int>();
                foreach (var setEffectType in effect.SetEffects)
                {
                    foreach (var setEffectCountData in setEffectType.SetEffectCountData)
                    {
                        if (!counts.Contains(setEffectCountData.Count))
                        {
                            counts.Add(setEffectCountData.Count);
                        }
                    }
                }
                counts.Sort();
                countText = string.Join("/", counts);
                if (effectDict.ContainsKey(effect.EffectName))
                {
                    effectDict[effect.EffectName] = (effect, effectDict[effect.EffectName].count + 1, countText);
                }
                else
                {
                    effectDict.Add(effect.EffectName, (effect, 1, countText));
                }
            }
        }
        // 세트 효과별로 UI 오브젝트 생성 및 데이터 할당
        foreach (var kvp in effectDict) // kvp.Key는 세트 효과 이름, kvp.Value는 (SetEffectData, count, countText) 튜플
        {
            GameObject viewerObj = Instantiate(viewerPrefab, createPositionObject.transform);
            var viewer = viewerObj.GetComponent<SetEffectViewer>();
            viewer.SetNameText(kvp.Key);
            viewer.setEffectData = kvp.Value.data;
            viewer.SetCurrentCountText(kvp.Value.count);
            viewer.SetCountText(kvp.Value.countText, kvp.Value.count);
            viewer.SetIcon();
        }
    }

    public void OnClickCloseSetEffectDescriptionPopup()
    {
        setEffectDescriptionPopup.SetActive(false);
        setEffectPopupBg.SetActive(false); 
    }                                             
}

using TMPro;
using UnityEngine;

public class InventoryPopup : MonoBehaviour
{
    [Header("InventoryPopup")]
    [SerializeField] private GameObject inventoryPopup;
    [SerializeField] private GameObject setEffectDescriptionPopup;

    [Header("ArtifactSlots")]
    [SerializeField] private GameObject[] artifactIcon = new GameObject[12];
    [SerializeField] private GameObject[] artifactRarity = new GameObject[12];

    [Header("EquipedArtifactSlots")]
    [SerializeField] private GameObject[] equipedArtifactIcon = new GameObject[4];
    [SerializeField] private GameObject[] equipedArtifactRarity = new GameObject[4];
    [SerializeField] private GameObject[] equipedArtifactRockIcon = new GameObject[4];

    [Header("ItemDescriptions")]
    [SerializeField] private GameObject itemIcon;
    [SerializeField] private GameObject itemRarity;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemTypeText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemFlavorText;

    private void Awake()
    {
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
        Refresh();
        OnClickArtifactSlot(0); // 0번 슬롯으로 초기화
    }
    public void OnClickCloseButton()
    {
        inventoryPopup.SetActive(false);
        setEffectDescriptionPopup.SetActive(false);
    }

    private void Refresh()
    {
        ArtifactSlotRefresh();
        EquipedArtifactSlotRefresh();
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
}

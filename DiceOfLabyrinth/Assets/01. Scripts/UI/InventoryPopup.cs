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

    [Header("ArtifactDescriptions")]
    [SerializeField] private TMP_Text artifactNameText;
    [SerializeField] private TMP_Text artifactSetEffectText;
    [SerializeField] private TMP_Text artifactDescriptionText;
    [SerializeField] private TMP_Text artifactFlavorText;

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
        DescriptionRefresh(slotIndex);
    }

    private void DescriptionRefresh(int slotIndex)
    {
        if (StageManager.Instance.stageSaveData.artifacts[slotIndex] != null)
        {
            var artifactInSlot = StageManager.Instance.stageSaveData.artifacts[slotIndex];
            string setEffectText = "";
            for(int i = 0; i < artifactInSlot.SetEffectData.Count; i++)
            {
                setEffectText += $"#{artifactInSlot.SetEffectData[i].EffectName} ";
            }
            artifactNameText.text = artifactInSlot.ArtifactName;
            artifactDescriptionText.text = artifactInSlot.Description;
            artifactFlavorText.text = artifactInSlot.FlavorText;
        }
    }
}

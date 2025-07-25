using UnityEngine;

public class InventoryState : MonoBehaviour
{
    [Header("Inventory State - Item Info")]
    [SerializeField] private GameObject engravingViewer;
    [SerializeField] private GameObject closeButton;
    [Header("Inventory State - Select EquipedArtifact")]
    [SerializeField] private GameObject topBG;
    [SerializeField] private GameObject selectEquipedArtifactViewer;
    [SerializeField] private GameObject selectEquipedArtifactButton;
    private void OnEnable()
    {
        if (StageManager.Instance?.stageSaveData == null)
        {
            return;
        }
        if (StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.EquipmentArtifact)
        {
            engravingViewer.SetActive(false);
            closeButton.SetActive(false);
            topBG.SetActive(true);
            InventoryPopup.Instance.blockInputBg.SetActive(false);
            selectEquipedArtifactViewer.SetActive(true);
            selectEquipedArtifactButton.SetActive(true);
        }
        else
        {
            engravingViewer.SetActive(true);
            closeButton.SetActive(true);
            topBG.SetActive(false);
            InventoryPopup.Instance.blockInputBg.SetActive(false);
            selectEquipedArtifactViewer.SetActive(false);
            selectEquipedArtifactButton.SetActive(false);
        }
    }
}

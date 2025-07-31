using System.Collections;
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

    private Coroutine _coroutine;
    private void OnEnable()
    {
        if (StageManager.Instance?.stageSaveData == null)
        {
            return;
        }
        InventoryPopup.Instance.Refresh();
        if (StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.EquipmentArtifact)
        {
            engravingViewer.SetActive(false);
            closeButton.SetActive(false);
            topBG.SetActive(true);
            InventoryPopup.Instance.blockInputBg.SetActive(false);
            selectEquipedArtifactViewer.SetActive(true);
            selectEquipedArtifactButton.SetActive(true);
            if (_coroutine == null)
            {
                _coroutine = StartCoroutine(UnRocKEquipedArtifact());
            }

        }
        else
        {
            engravingViewer.SetActive(true);
            closeButton.SetActive(true);
            topBG.SetActive(false);
            InventoryPopup.Instance.blockInputBg.SetActive(false);
            selectEquipedArtifactViewer.SetActive(false);
            selectEquipedArtifactButton.SetActive(false);
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }
    }
    private void OnDisable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator UnRocKEquipedArtifact()
    {
        InventoryPopup.Instance.blockInputBg.SetActive(true); // 입력 막기
        var rockIcon = InventoryPopup.Instance.equipedArtifactRockIcon[StageManager.Instance.stageSaveData.currentStageIndex];
        if (rockIcon.GetComponent<CanvasGroup>() == null)
        {
            rockIcon.AddComponent<CanvasGroup>();
        }
        float t = 0;
        float duration = 2; // 애니메이션 지속 시간
        while (t < duration)
        {
            rockIcon.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        rockIcon.GetComponent<CanvasGroup>().alpha = 0f; // 애니메이션이 끝나면 알파를 0으로 설정
        rockIcon.SetActive(false);
        rockIcon.GetComponent<CanvasGroup>().alpha = 1f; // 다음 사용을 위해 알파를 1로 초기화
        InventoryPopup.Instance.blockInputBg.SetActive(false); // 입력 막기 해제
        
    }
}

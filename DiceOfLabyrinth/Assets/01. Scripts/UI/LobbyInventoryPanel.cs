using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class LobbyInventoryPanel : MonoBehaviour
{
    public static LobbyInventoryPanel Instance { get; private set; }
    [Header("인벤토리 뷰어")]
    [SerializeField] private List<ItemInventoryViewer> itemInventoryViewers;

    [Header("인벤토리 디스크립션 패널")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TMP_Text itemAmountText;
    private ItemSO selectedItemSO;
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
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "LobbyScene")
        {
            // 로비 씬이 아닐 경우 패널 비활성화
            gameObject.SetActive(false);
            return;
        }
        // 로비 씬일 경우 리프레시
        Refresh();
    }

    private void Refresh()
    {
        // 인벤토리 패널의 내용을 갱신하는 로직을 여기에 작성
        // 예: 아이템 목록을 불러와서 UI에 표시
        if (itemInventoryViewers == null || itemInventoryViewers.Count == 0)
        {
            Debug.LogWarning("ItemInventoryViewers가 설정되지 않았습니다.");
            return;
        }
        selectedItemSO = null; // 선택된 아이템 초기화
        RefreshInventoryViewer();
        RefreshDescription(); // 디스크립션 초기화
    }

    private void RefreshInventoryViewer()
    {
        while (ItemManager.Instance.OwnedItems.Count > itemInventoryViewers.Count)
        {
            // 아이템 뷰어가 부족할 경우 새로 생성
            ItemInventoryViewer newViewer = Instantiate(itemInventoryViewers[0], itemInventoryViewers[0].transform.parent);
            itemInventoryViewers.Add(newViewer);
        }
        int idx = 0;
        foreach (var kvp in ItemManager.Instance.OwnedItems)
        {
            if (idx < itemInventoryViewers.Count)
            {
                // 아이템 뷰어에 아이템 데이터 설정
                ItemSO itemSO = ItemManager.Instance.AllItems[kvp.Key];
                itemInventoryViewers[idx].SetItemData(itemSO);
                itemInventoryViewers[idx].gameObject.SetActive(true);
                idx++;
            }
        }
        for (int i = idx; i < itemInventoryViewers.Count; i++)
        {
            // 남은 뷰어는 비활성화
            itemInventoryViewers[i].gameObject.SetActive(false);
        }
    }
    private void RefreshDescription()
    {
        // 아이템 클릭 시 디스크립션 패널 업데이트

        if (selectedItemSO == null)
        {
            itemNameText.text = "아이템 없음";
            itemDescriptionText.text = "아이템을 선택해주세요.";
            itemIconImage.sprite = null;
            itemAmountText.text = "x 0";
            return;
        }
        itemNameText.text = selectedItemSO.NameKr; // 한국어 이름
        itemDescriptionText.text = selectedItemSO.Description;
        itemIconImage.sprite = selectedItemSO.Icon;
        itemAmountText.text = ItemManager.Instance.OwnedItems.TryGetValue(selectedItemSO.ItemID, out int amount) ? $"x {amount}" : "0";

    }
    public void OnItemSelected(ItemSO itemSO)
    {
        selectedItemSO = itemSO;
        RefreshDescription();
    }

    public void OnClickLootingWayButton()
    {
        // 아이템 획득 방법 버튼 클릭 시 로직
        if (selectedItemSO == null)
        {
            return;
        }
        string selectedItemSOLootingWay = selectedItemSO.LootType.ToString();
        UIManager.Instance.messagePopup.Open(
            $"아이템 획득 방법: {selectedItemSOLootingWay}"
        );
    }
    public void OnClickUseButton()
    {
        // 아이템 사용 버튼 클릭 시 로직
        if (selectedItemSO == null)
        {
            return;
        }
        if (selectedItemSO is EXPpotion || selectedItemSO is SkillBook)
        {
            // 캐릭터 씬으로 이동할지 묻는 팝업
            UIManager.Instance.messagePopup.Open(
                "아이템을 사용하시겠습니까?\n" +
                "캐릭터 선택 화면으로 이동합니다.",
                () =>
                {
                    // 캐릭터 선택 씬으로 이동
                    SceneManagerEx.Instance.LoadScene("CharacterScene");
                },
                () =>
                {
                    UIManager.Instance.messagePopup.Close();
                }
            );
        }
        else
        {
            UIManager.Instance.messagePopup.Open("이 아이템은 아직 사용할 수 없습니다.");
        }
    }
    public void OnClickUnimplementedFeatureButton() // 미구현된 기능 버튼 클릭 시
    {
        UIManager.Instance.messagePopup.Open("미구현된 기능입니다.\n" +
            "추후 업데이트될 예정입니다.");
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventoryViewer : MonoBehaviour
{
    [Header("아이템 데이터(코드에서 자동으로 할당됨)")]
    public ItemSO ItemSO;

    [Header("아이템 정보")]
    [SerializeField] private Image icon; // 아이템 아이콘 이미지
    [SerializeField] private TMPro.TMP_Text amount; // 아이템 개수 표시

    public void SetItemData(ItemSO item)
    {
        ItemSO = item;
        if (ItemSO == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        icon.sprite = ItemSO.Icon; // 아이템 아이콘 설정
        amount.text = ItemManager.Instance.OwnedItems.TryGetValue(ItemSO.ItemID, out int itemAmount) ? $"x {itemAmount}" : "0"; // 아이템 개수 설정
    }
    public void OnClickItemViewer()
    {
        // 아이템 정보 팝업을 띄우는 로직
        if (ItemSO == null || !ItemManager.Instance.OwnedItems.ContainsKey(ItemSO.ItemID))
        {
            // 아이템이 소유되지 않은 경우
            return;
        }
        // 로비 씬에서 아이템 클릭 시 인벤토리 패널의 아이템 선택 이벤트 호출
        if (LobbyInventoryPanel.Instance.gameObject.activeSelf)
        {
            LobbyInventoryPanel.Instance.OnItemSelected(ItemSO);
        }
    }
}

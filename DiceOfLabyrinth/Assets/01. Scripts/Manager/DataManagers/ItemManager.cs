using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 아이템 데이터를 관리하는 매니저 클래스
/// 유저의 보유 아이템을 저장하고, DataSaver를 통해 데이터를 저장, 불러오는 역할
/// </summary>

public class ItemManager
{
    // 싱글톤 인스턴스

    private static ItemManager _instance;
    public static ItemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ItemManager();
                _instance.Initialize();
            }
            return _instance;
        }
    }



    // 보유 중인 아이템과, 해당 아이템의 개수를 저장하는 딕셔너리
    private Dictionary<string, int> ownedItems;

    // Addressable로 로드된 모든 아이템 SO
    private Dictionary<string, ItemSO> allItems = new Dictionary<string, ItemSO>();


    private bool isLoaded = false;


    //이니셜라이저
    private void Initialize()
    {
        // Addressable에서 모든 아이템 SO를 비동기적으로 로드
        ownedItems = new Dictionary<string, int>();

        //에디터에만 실행하는 디버그
#if UNITY_EDITOR
        Debug.Log("ItemManager Initialized");
#endif
    }


    // 모든 아이템 데이터 로드


    // Addressable에서 모든 아이템 SO 비동기 로드, 아이템은 ItemSO 라벨
    public void LoadAllItemSOs()
    {
        // "ItemSO" 라벨이 붙은 모든 ItemSO를 비동기로 로드
        Addressables.LoadAssetsAsync<ItemSO>("ItemSO", OnItemSOLoaded).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                isLoaded = true;
#if UNITY_EDITOR
                Debug.Log($"All ItemSOs loaded successfully. Count: {allItems.Count}");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("Failed to load ItemSOs.");
#endif
            }
        };
    }

    // 각 SO가 로드될 때마다 딕셔너리에 저장
    private void OnItemSOLoaded(ItemSO itemSO)
    {
        if (itemSO != null && !allItems.ContainsKey(itemSO.ItemID))
            allItems.Add(itemSO.ItemID, itemSO);
    }

    // 아이템ID 유효성 검사
    private bool IsValidItemID(string itemID)
    {
        // 아이템ID가 null이거나 빈 문자열인 경우
        if (string.IsNullOrEmpty(itemID))
            return false;
        // 아이템SO 딕셔너리에 해당 아이템ID가 존재하는지 확인
        return allItems.ContainsKey(itemID);
    }


    // 아이템 획득 메서드
    public void GetItem(string ItemID, int Count)
    {
        // 아이템SO의 ItemID가 유효한지 확인 (별도의 메서드 호출)
        if (!IsValidItemID(ItemID))
        {
            // 유효하지 않은 아이템ID인 경우 예외 처리(보통 일어나면 안됨)
            return;
        }
        // 유효한 아이템이라면 보유 중인 아이템 항목에 추가.
        // 이미 존재한다면 개수만 증가시키고, 존재하지 않는다면 새로 추가
        if (ownedItems.ContainsKey(ItemID))
        {
            ownedItems[ItemID] += Count;
        }
        else
        {
            ownedItems.Add(ItemID, Count);
        }
    }
    /// <summary>
    /// 아이템의 ID를 통해 해당 아이템SO를 반환하는 메서드
    /// </summary>
    /// <param name="itemID"></param>

    public ItemSO GetItemSO(string itemID)
    {    
        if (!IsValidItemID(itemID))
        {
            // 유효하지 않은 아이템ID인 경우 예외 처리
            return null;
        }
        // 아이템ID가 유효한 경우, 해당 아이템SO를 반환
        if (allItems.TryGetValue(itemID, out ItemSO itemSO))
        {
            return itemSO;
        }
        else
        {
            // 아이템SO가 존재하지 않는 경우 null 반환
            return null;
        }
    }


    // 하위는 각 아이템 종류별로 보유 중인 아이템 반환(인벤토리 필터 등)

    // 보유중인 EXP 포션 딕셔너리
    public Dictionary<string, int> GetPotions()
    {
        // 아이템들 중 EXPpotion으로 생성된 SO만 출력
        Dictionary<string, int> potions = new Dictionary<string, int>();
        foreach (var item in ownedItems)
        {
            ItemSO itemSO = GetItemSO(item.Key);
            if (itemSO is EXPpotion expPotion)
            {
                potions.Add(item.Key, item.Value);
            }
        }
        return potions;
    }


    // 보유 중인 아이템 중, 스킬 북 딕셔너리

    public Dictionary<string, int> GetSkillBooks()
    {
        // 아이템들 중 SkillBook으로 생성된 SO만 출력
        Dictionary<string, int> skillBooks = new Dictionary<string, int>();
        foreach (var item in ownedItems)
        {
            ItemSO itemSO = GetItemSO(item.Key);
            if (itemSO is SkillBook skillBook)
            {
                skillBooks.Add(item.Key, item.Value);
            }
        }
        return skillBooks;
    }


    // 보유 중인 아이템 중, 돌파석(AscensionStone) 리스트
    public Dictionary<string, int> GetAscensionStones()
    {
        // 아이템들 중 AscensionStone으로 생성된 SO만 출력
        Dictionary<string, int> ascensionStones = new Dictionary<string, int>();
        foreach (var item in ownedItems)
        {
            ItemSO itemSO = GetItemSO(item.Key);
            if (itemSO is AscensionStone ascensionStone)
            {
                ascensionStones.Add(item.Key, item.Value);
            }
        }
        return ascensionStones;
    }
    
    // 보유 중인 아이템을 아이템 ID 순서대로 정렬 (아이템ID: Item_(숫자) 형식)


}

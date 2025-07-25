using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GetCharacter : MonoBehaviour
{
    //확률에 따라 캐릭터 혹은 아이템을 획득
    //이미 보유한 캐릭터를 획득할 경우, 돌파석(Ascension Stone) 획득
    //아이템은 ItemManager, 캐릭터는 CharacterManager에서 관리
    // SSR 2% SR 18% R 80% 확률로 획득
    // SSR : 현재는 모든 캐릭터들이 SSR등급, 및 상급 포션/상급 스킬북이 SSR등급
    // 포션/스킬북 획득확률이 캐릭터 획득 확률의 2배 (예: SSR 캐릭터 획득 확률 2% -> 상급 포션/스킬북 획득 확률 4%)
    // SR : 스킬북 중급/포션 중급
    // R : 스킬북 하급/포션 하급
    // 아이템 개수는 5~10개 사이로 랜덤 획득

    public void GetCharacters()
    {
        // 1. 이미 보유한 캐릭터 ID 집합
        var ownedCharIDs = new HashSet<string>(
            CharacterManager.Instance.OwnedCharacters.Select(c => c.CharacterData.charID)
        );

        // 2. 아직 보유하지 않은 캐릭터만 후보로 추출
        var candidates = CharacterManager.Instance.AllCharacters.Values
            .Where(so => !ownedCharIDs.Contains(so.charID))
            .ToList();

        // 3. 후보가 5명 미만이면 모두, 아니면 5명만 랜덤으로 선택
        int count = Mathf.Min(5, candidates.Count);

        // 4. 랜덤 셔플
        for (int i = 0; i < candidates.Count; i++)
        {
            int j = Random.Range(i, candidates.Count);
            var temp = candidates[i];
            candidates[i] = candidates[j];
            candidates[j] = temp;
        }

        // 5. 획득 처리
        for (int i = 0; i < count; i++)
        {
            var so = candidates[i];
            CharacterManager.Instance.AcquireCharacter(so.charID);
            Debug.Log($"획득한 캐릭터: {so.nameKr} ({so.charID})");
        }
    }


    // SSR ID, SR ID, R ID 배열

    [Header("소환 등급 별 ID")]
    [SerializeField] private List<string> SSRCharacterIDs; // SSR 캐릭터 ID 리스트 (char_0~char_4)
    [SerializeField] private List<string> SSRItemIds; // SSR 아이템 ID 리스트
    [SerializeField] private List<string> SRCharacterIDs; // SR 캐릭터 ID 리스트
    [SerializeField] private List<string> SRItemIds; // SR 아이템 ID 리스트
    [SerializeField] private List<string> RCharacterIDs; // R 캐릭터 ID 리스트
    [SerializeField] private List<string> RItemIds; // R 아이템 ID 리스트


    // 소환 버튼
    // 단차
    public void GatchaSingle()
    {
        // 1회 소환 로직+ UI 업데이트도 포함
    }

    // 10연차
    public void GatchaTen()
    {
        // 10연차 소환 로직 구현 + UI 업데이트도 포함


    }

    public void Gatcha()
    {
        // 소환 자체 함수 (1회면 1회소환 1회면 10회 소환)
    }


    // 확률 계산 함수 - Random.Range를 사용하여 확률에 따라 캐릭터 또는 아이템 획득
    public void CalculateProbability()
    {
        // 0~100 사이의 랜덤 값 생성
        float randomValue = Random.Range(0f, 100f);

        // 확률에 따라 캐릭터 또는 아이템 획득
        if (randomValue < 2) // SSR 캐릭터 획득 확률 2%
        {
            Debug.Log($"뽑기 확률: {randomValue}");
            Debug.Log("SSR! 보상 획득");

            // SSR 캐릭터 뽑기 확률과 
            // SSR 캐릭터 획득 로직 추가
        }
        else if (randomValue < 20) // SR 캐릭터 획득 확률 18%
        {
            Debug.Log("SR 보상 획득");
            // SR 캐릭터 획득 로직 추가
        }
        else // R 캐릭터 획득 확률 80%
        {
            Debug.Log("R 보상 획득!");
            // R 캐릭터 획득 로직 추가
        }
    }


    // 캐릭터 획득 함수
    public void GetCharacterss(string CharID)
    {
        // 만약 획득한 캐릭터가 이미 보유한 캐릭터라면 돌파석 획득 - CharacterManager에서 처리함
        if (CharacterManager.Instance.IsLoaded == false)
        {
            Debug.LogError("캐릭터 데이터가 로드되지 않았습니다.");
            return;
        }
        List<LobbyCharacter> ownedCharacters = CharacterManager.Instance.OwnedCharacters;

        // 아래의 함수는 중복된 캐릭터면 돌파석, 아니면 새로운 캐릭터를 획득함
        // CharacterManager의 OwnedCharacters에 CharID가 있는지 확인 후, 동일하면 돌파석 표시를, 없다면 캐릭터를 표시 (GridView에 슬롯 추가)

        //true/false로 중복 캐릭터 확인 후 UI표시에 활용
        bool isDuplicate = DuplicateCharacterCheck(CharID);

        if (isDuplicate)
        {
            // 이미 보유한 캐릭터는 같은 charID를 가진 Ascension Stone 획득
            ItemManager.Instance.GetAscensionStone(CharID);
            return;
            
        }
        
        CharacterManager.Instance.AcquireCharacter(CharID);
    }

    private bool DuplicateCharacterCheck(string charID)
    {
        // CharacterManager의 OwnedCharacters에 charID가 있는지 확인
        return CharacterManager.Instance.OwnedCharacters.Any(c => c.CharacterData.charID == charID);
    }

    // 아이템 획득 함수(임시)

    public void GetItems(string itemID, int count)
    {
        ItemManager.Instance.GetItem(itemID, count);
    }
}

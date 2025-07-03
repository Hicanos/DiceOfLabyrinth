using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GetCharacter : MonoBehaviour
{
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
}

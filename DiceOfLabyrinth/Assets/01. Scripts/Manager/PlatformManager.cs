using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance { get; private set; }
    public GameObject[] platforms = new GameObject[5];

    // 캐릭터 오브젝트를 담을 부모 컨테이너
    public Transform platformAndCharacterContainer;

    // 각 플랫폼별 캐릭터 풀
    private GameObject[] characterObjects = new GameObject[5];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetPlatformAndCharacters()
    {
        var stageManager = StageManager.Instance;
        var chapterData = stageManager.chapterData;
        int chapterIdx = stageManager.stageSaveData.currentChapterIndex;
        int formationIdx = (int)stageManager.stageSaveData.currentFormationType;

        if (chapterData.chapterIndex.Count <= chapterIdx || chapterIdx < 0)
            return;

        var stageData = chapterData.chapterIndex[chapterIdx].stageData;
        var formations = stageData.PlayerFormations;

        if (formations.Count <= formationIdx || formationIdx < 0)
            return;

        var playerPositions = formations[formationIdx].PlayerPositions;
        var entryCharacters = stageManager.stageSaveData.entryCharacters;

        // 1. 모든 풀링 오브젝트를 일단 비활성화 (중복 활성화 방지)
        foreach (Transform child in platformAndCharacterContainer)
            child.gameObject.SetActive(false);

        for (int i = 0; i < platforms.Length; i++)
        {
            // 플랫폼 위치 및 활성화
            if (platforms[i] != null && playerPositions != null && playerPositions.Count > i)
            {
                Vector3 position = playerPositions[i].Position;
                platforms[i].transform.position = position;
                platforms[i].SetActive(true);
            }
            else if (platforms[i] != null)
            {
                platforms[i].SetActive(false);
            }

            // 캐릭터 오브젝트 풀링 및 위치/활성화
            CharacterSO charSO = (entryCharacters != null && entryCharacters.Count > i) ? entryCharacters[i] : null;

            if (charSO != null && charSO.charBattlePrefab != null)
            {
                GameObject found = null;

                // 컨테이너 내에서 같은 프리팹 이름(클론)이고 비활성화된 오브젝트 찾기
                foreach (Transform child in platformAndCharacterContainer)
                {
                    if (!child.gameObject.activeSelf && child.gameObject.name == charSO.charBattlePrefab.name + "(Clone)")
                    {
                        found = child.gameObject;
                        break;
                    }
                }

                if (found != null)
                {
                    characterObjects[i] = found;
                    characterObjects[i].SetActive(true);
                    characterObjects[i].transform.position = playerPositions[i].Position;
                }
                else
                {
                    characterObjects[i] = Instantiate(charSO.charBattlePrefab, playerPositions[i].Position, Quaternion.identity, platformAndCharacterContainer);
                }
            }
            else
            {
                characterObjects[i] = null;
            }
        }
        var leader = StageManager.Instance.stageSaveData.leaderCharacter;
        for (int i = 0; i < platforms.Length; i++)
        {
            var entry = (entryCharacters != null && entryCharacters.Count > i) ? entryCharacters[i] : null;
            bool isLeader = (entry != null && entry == leader);
            if (platforms[i] != null)
            {
                var relay = platforms[i].GetComponent<PlatformClickRelay>();
                if (relay != null)
                    relay.SetAsLeader(isLeader);
            }
        }
    }

    public void DeactivateAllCharacters()
    {
        for (int i = 0; i < characterObjects.Length; i++)
        {
            if (characterObjects[i] != null)
                characterObjects[i].SetActive(false);
        }
    }
}

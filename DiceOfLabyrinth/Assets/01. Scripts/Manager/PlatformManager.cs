using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance { get; private set; }
    public GameObject[] platforms = new GameObject[5];

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
    public void SetPlatformPosition()
    {
        var stageManager = StageManager.Instance;
        var chapterData = stageManager.chapterData;
        int chapterIdx = stageManager.stageSaveData.currentChapterIndex;
        int formationIdx = (int)stageManager.stageSaveData.currentFormationType;

        // 챕터 인덱스 범위 체크
        if (chapterData.chapterIndex.Count <= chapterIdx || chapterIdx < 0)
            return;

        var stageData = chapterData.chapterIndex[chapterIdx].stageData;
        var formations = stageData.PlayerFormations;

        // 포메이션 인덱스 범위 체크
        if (formations.Count <= formationIdx || formationIdx < 0)
            return;

        var playerPositions = formations[formationIdx].PlayerPositions;

        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i] != null
                && playerPositions != null
                && playerPositions.Count > i)
            {
                Vector3 position = playerPositions[i].Position;
                platforms[i].transform.position = position;
                platforms[i].SetActive(true); // 플랫폼 활성화
            }
            else if (platforms[i] != null)
            {
                platforms[i].SetActive(false); // 데이터가 없으면 비활성화
            }
        }
    }
}

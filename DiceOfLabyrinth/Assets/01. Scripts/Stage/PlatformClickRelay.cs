using UnityEngine;

public class PlatformClickRelay : MonoBehaviour
{
    public int platformIndex;
    [SerializeField] private GameObject leaderMark;
    private void Awake()
    {
        var uiController = FindAnyObjectByType<BattleUIController>();
        if (uiController != null)
        {
            uiController.characterPlatforms[platformIndex] = this.gameObject;
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
    public void SetAsLeader(bool isLeader)
    {
        leaderMark.SetActive(isLeader);
    }
}
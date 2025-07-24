using UnityEditor.VersionControl;
using UnityEngine;

public class RecoveryPopup : MonoBehaviour
{
    public static RecoveryPopup Instance { get; private set; }

    [SerializeField] private GameObject shopPopup;
    [SerializeField] private MessagePopup messagePopup;

    [Header("Character Viewers")]
    [SerializeField] private GameObject[] characterImages;
    [SerializeField] private GameObject[] characterHealthBars;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;// Don't destroy on load는 필요하지 않음
        }
    }
    private void OnEnable()
    {
        StartRecoveryPopup();
    }

    private void StartRecoveryPopup()
    {
        PlayerStateRefresh(0);
    }
    private void PlayerStateRefresh(int recoveryCount)
    {
        
    }
}

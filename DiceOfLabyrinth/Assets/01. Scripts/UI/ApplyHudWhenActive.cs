using UnityEngine;

public sealed class ApplyHudWhenActive : MonoBehaviour
{
    [SerializeField] private HudMode modeToApply = HudMode.Inventory;

    private void OnEnable()
    {
        if (UIManager.Instance != null && UIManager.Instance.publicUIController != null)
        {
            UIManager.Instance.SetHudMode(modeToApply);
        }
    }
}

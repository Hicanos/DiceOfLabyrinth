using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    [SerializeField] private Button closeBg;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void OnClickCloseButton()
    {
        gameObject.SetActive(false);
    }
}

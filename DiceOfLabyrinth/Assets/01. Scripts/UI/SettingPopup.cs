using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    [SerializeField] private Button closeBg;

    public void OnClickCloseButton()
    {
        gameObject.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class SelectItemPanel : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject artifactsSetEffectPopup;

    [Header("Buttons")]
    [SerializeField] private Button tipButton;

    public void OnClickTip()
    {
        if (artifactsSetEffectPopup != null)
            artifactsSetEffectPopup.SetActive(true);
    }
}

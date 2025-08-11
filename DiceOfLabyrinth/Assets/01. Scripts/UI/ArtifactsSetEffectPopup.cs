using UnityEngine;
using UnityEngine.UI;

public class ArtifactsSetEffectPopup : MonoBehaviour
{
    [SerializeField] private Button closeBg;
    [SerializeField] private Button closeButton;

    public void OnClickCloseButton()
    {
        gameObject.SetActive(false);
    }
}

using UnityEngine;

public class CharacterUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject characterPanel;

    private void OnEnable()
    {
        characterPanel.SetActive(true);
    }
}

using UnityEngine;

public class CharacterUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject characterListPanel;

    private void OnEnable()
    {
        characterListPanel.SetActive(true);
    }
}

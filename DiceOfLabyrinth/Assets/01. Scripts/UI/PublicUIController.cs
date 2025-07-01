using UnityEditor.Build.Pipeline.Tasks;
using UnityEngine;

public class PublicUIController : MonoBehaviour
{
    [SerializeField] private GameObject stamina;
    [SerializeField] private GameObject gold;
    [SerializeField] private GameObject jewel;
    [SerializeField] private GameObject settingButton;
    [SerializeField] private GameObject homeButton;

    public void SetMode_Lobby()
    {
        stamina.SetActive(true);
        gold.SetActive(true);
        jewel.SetActive(true);
        settingButton.SetActive(true);
        homeButton.SetActive(false);
    }

    public void SetMode_SelectAdventure()
    {
        stamina.SetActive(true);
        gold.SetActive(true);
        jewel.SetActive(true);
        settingButton.SetActive(true);
        homeButton.SetActive(true);
        
    }
}

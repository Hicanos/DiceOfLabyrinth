using TMPro;
using UnityEngine;

public class CheckPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text msgText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Open(string msg)
    {
        msgText.text = msg;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
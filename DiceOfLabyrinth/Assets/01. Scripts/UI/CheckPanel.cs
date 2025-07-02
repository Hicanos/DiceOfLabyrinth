using TMPro;
using UnityEngine;

public class CheckPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text questionText;

    public void Open(string msg)
    {
        questionText.text = msg;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
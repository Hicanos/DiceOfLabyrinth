using TMPro;
using UnityEngine;

public class SetEffectViewer : MonoBehaviour
{
    [Header("SetEffectViewer Data")]
    public SetEffectData setEffectData;
    [Header("SetEffectViewer Components")]
    [SerializeField] private TMP_Text effectNameText;
    [SerializeField] private TMP_Text effectCurrentCountText;
    [SerializeField] private TMP_Text effectCountText;
    [SerializeField] private GameObject effectIconObject;
    [Header("SetEffectViewer Colors")]
    [SerializeField] private Color validCountColor = Color.white;
    [SerializeField] private Color invalidCountColor = new Color(0.53f, 0.53f, 0.53f);


    public void SetNameText(string text)
    {
        effectNameText.text = text;
    }
    public void SetCurrentCountText(int count)
    {
        effectCurrentCountText.text = $"{count}";
    }
    public void SetCountText(string text, int currentCount)
    {
        var countArr = text.Split('/');
        string validColorStr = ColorUtility.ToHtmlStringRGB(validCountColor);
        string invalidColorStr = ColorUtility.ToHtmlStringRGB(invalidCountColor);
        string richCountText = "";
        for (int i = 0; i < countArr.Length; i++)
        {
            if (i > 0) richCountText += "/";
            int value;
            if (int.TryParse(countArr[i], out value))
            {
                if (value <= currentCount)
                    richCountText += $"<color=#{validColorStr}>{countArr[i]}</color>";
                else
                    richCountText += $"<color=#{invalidColorStr}>{countArr[i]}</color>";
            }
            else
            {
                richCountText += countArr[i];
            }
        }
        effectCountText.text = richCountText;
    }
    public void SetIcon()
    {
        if (setEffectData != null && setEffectData.Icon != null)
        {
            effectIconObject.GetComponent<UnityEngine.UI.Image>().sprite = setEffectData.Icon;
        }
        else
        {
            effectIconObject.GetComponent<UnityEngine.UI.Image>().sprite = null;
        }
    }
}

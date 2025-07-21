using TMPro;
using UnityEngine;

public class SetEffectViewer : MonoBehaviour
{
    public SetEffectData setEffectData;
    [SerializeField] private TMP_Text effectNameText;
    [SerializeField] private TMP_Text effectCurrentCountText;
    [SerializeField] private TMP_Text effectCountText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (setEffectData == null) return;
        effectNameText.text = setEffectData.EffectName;
        string CountText = "";
        for (int i = 0; i < setEffectData.SetEffectCounts.Count; i++)
        {
            CountText += $"{setEffectData.SetEffectCounts[i].SetEffectCountData.Count}/";
        }
        effectCountText.text = CountText;
    }
    public void SetNameText(string text)
    {
        effectNameText.text = text;
    }
    public void SetCurrentCountText(int count)
    {
        effectCurrentCountText.text = $"{count}";
    }
    public void SetCountText(string text)
    {
        effectCountText.text = text;
    }
}

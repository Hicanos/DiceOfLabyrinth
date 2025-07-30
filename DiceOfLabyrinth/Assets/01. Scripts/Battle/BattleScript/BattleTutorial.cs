using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TMPro;

public class BattleTutorial : MonoBehaviour
{
    [SerializeField] GameObject tutorialBoard;
    [SerializeField] TextMeshProUGUI tutorialText;
    public BattleTutorialData TutorialData = new BattleTutorialData();

    private int textIndex;
    private int textMaxIndex;
    [SerializeField] float textWriteTime;
    private IEnumerator writeTextCoroutine;
    private bool isWriting;
    private LoadTutorialData loadTutorialData;
    private int currentIndex = -1;

    public void LoadData()
    {
        loadTutorialData = new LoadTutorialData();
        loadTutorialData.LoadData();
        loadTutorialData.GetIsTutorialOver();
        TutorialData.Texts = loadTutorialData.GetTexts();
        textMaxIndex = TutorialData.Texts[0].Length;
    }
    
    public void StartTutorial(int iNum = -1)
    {
        if (BattleManager.Instance.isTutorialOver) return;
        int index;

        switch (iNum)
        {
            case (int)DetailedTurnState.Enter:                
                index = 0;
                if(currentIndex == 3)
                {
                    index = 4;
                }
                else if (currentIndex >= index)
                {
                    return;
                }
                break;
            case (int)DetailedTurnState.RollEnd:
                index = 1;
                if (currentIndex >= index)
                {
                    return;
                }
                break;
            case (int)DetailedTurnState.AttackEnd:
                index = 3;
                if (currentIndex >= index)
                {
                    return;
                }
                break;
            case -1:
                index = 2;
                if (currentIndex >= index)
                {
                    return;
                }
                break;
            default:
                return;
        }

        ActiveTutorialText(index);
    }

    private void ActiveTutorialText(int index)
    {
        if (index == currentIndex)
        {
            return;
        }

        currentIndex = index;
        tutorialBoard.SetActive(true);
        textIndex = 0;

        textMaxIndex = TutorialData.Texts[index].Length;

        WriteText(index);
    }

    private void DeactiveTutorialText()
    {
        tutorialBoard.SetActive(false);

        if(currentIndex + 1 == TutorialData.Texts.Length)
        {
            Debug.Log(1);
            BattleManager.Instance.isTutorialOver = true;
            TutorialData.IsTutorialOver = true;
            loadTutorialData.SaveData();
        }
    }

    public void OnClickTutorialTouch()
    {
        if(isWriting)
        {
            SkipWriteText();
        }
        else
        {
            textIndex++;
            if (textIndex == textMaxIndex)
            {
                DeactiveTutorialText();
                return;
            }

            WriteText(currentIndex);
        }        
    }

    public void OnClickTutorialSkip()
    {
        DeactiveTutorialText();
    }

    private void WriteText(int index)
    {
        string text = TutorialData.Texts[index][textIndex];

        writeTextCoroutine = WriteTextCoroutine(text);
        StartCoroutine(writeTextCoroutine);
    }

    IEnumerator WriteTextCoroutine(string text)
    {
        isWriting = true;
        int textLength = text.Length;
        string useText = text.Substring(0,1);
        float pastTime = 0;
        int index = 1;

        while(true)
        {
            if(pastTime > textWriteTime)
            {
                pastTime = 0;
                index++;                
                useText = text.Substring(0, index);
            }

            tutorialText.text = useText;

            if (index == textLength)
            {
                isWriting = false;
                break;
            }

            pastTime += Time.deltaTime;
            yield return null;
        }
    }

    public void SkipWriteText()
    {
        StopCoroutine(writeTextCoroutine);
        isWriting = false;

        tutorialText.text = TutorialData.Texts[currentIndex][textIndex];
    }
}

public class BattleTutorialData
{
    public string[][] Texts;
    public bool IsTutorialOver;
}

public class LoadTutorialData
{
    JObject root;
    string FilePath = Application.dataPath + "\\Resources\\Json\\BattleTutorialData.json";
    public void LoadData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/BattleTutorialData");
        string jsonString = textAsset.text;
        root = JObject.Parse(jsonString);
    }

    public string[][] GetTexts()
    {
        JToken Texts = root["Texts"];

        return JsonConvert.DeserializeObject<string[][]>(Texts.ToString());
    }

    public void GetIsTutorialOver()
    {
        JToken isOver = root["IsTutorialOver"];

        BattleManager.Instance.isTutorialOver = (bool)isOver;
    }

    public void SaveData()
    {
        string jsonString = JsonConvert.SerializeObject(UIManager.Instance.BattleUI.BattleTutorial.TutorialData, Formatting.Indented);

        File.WriteAllText(FilePath, jsonString);
    }
}

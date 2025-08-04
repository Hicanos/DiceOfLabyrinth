using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BattleTutorial : MonoBehaviour
{    
    private LoadTutorialData loadTutorialData;
    public BattleTutorialDataForSave DataForSave = new BattleTutorialDataForSave();

    Dictionary<int, BattleTutorialData> tutorialDataDic = new Dictionary<int, BattleTutorialData>();

    [SerializeField] float textWriteTime;

    private int dataLength;
    private int textLength;
    private int currentDataIndex;
    private int currentTextIndex = -1;

    public bool IsTutorialOver;
    private bool isRollEndTutorialDone;
    private bool isConfirmTutorialDone;
    private bool isWriting;

    private IEnumerator writeTextCoroutine;

    //public void LoadData()
    //{
    //    StartCoroutine(LoadDataCoroutine());
    //}
    public void LoadData()
    {        
        BattleTutorialData[] datas;

        loadTutorialData = new LoadTutorialData();
        loadTutorialData.LoadData();

        Debug.Log(TutorialManager.Instance.isGameTutorialCompleted);
        if (TutorialManager.Instance.isGameTutorialCompleted)
        {
            Debug.Log("튜토리얼이 이미 진행되어 데이터 받아오지 않음");
            return;
        }


        datas = loadTutorialData.GetData();
        DataForSave.Data = datas;

        dataLength = datas.Length;

        for(int i = 0; i < dataLength; i++)
        {
            tutorialDataDic.Add(datas[i].Index, datas[i]);
        }

        BattleUI battleUI = UIManager.Instance.BattleUI;
        battleUI.TutorialPushButton.onClick.AddListener(OnClickTutorialTouch);
        battleUI.TutorialSkipButton.onClick.AddListener(OnClickTutorialSkip);
    }
    
    public void StartTutorial(int iNum = -1)
    {
        if (TutorialManager.Instance.isGameTutorialCompleted) return;

        switch (iNum)
        {
            case (int)DetailedTurnState.Enter:
                ActiveTutorialText();
                break;
            case (int)DetailedTurnState.RollEnd:
                if(isRollEndTutorialDone == false)
                {
                    ActiveTutorialText();
                }
                isRollEndTutorialDone = true;
                break;
            case (int)DetailedTurnState.AttackEnd:
                ActiveTutorialText();
                break;
            case -1:
                if(isConfirmTutorialDone == false)
                {
                    ActiveTutorialText();
                }
                isConfirmTutorialDone = true;
                break;
            default:
                return;
        }
    }

    private void ActiveTutorialText()
    {
        UIManager.Instance.BattleUI.TutorialBoardSetActive(true);

        currentTextIndex = 0;
        textLength = tutorialDataDic[currentDataIndex].Texts.Length;

        WriteText(currentDataIndex, currentTextIndex);
    }

    /// <summary>
    /// 튜토리얼 UI를 끄고, 다음 데이터 인덱스를 받습니다.
    /// 다음 데이터 인덱스가 -1인 경우 튜토리얼 종료입니다.
    /// </summary>
    private void DeactiveTutorialText()
    {
        UIManager.Instance.BattleUI.TutorialBoardSetActive(false);

        currentDataIndex = tutorialDataDic[currentDataIndex].IndexGoesTo;

        if (currentDataIndex == -1)
        {
            loadTutorialData.SaveData();

            BattleUI battleUI = UIManager.Instance.BattleUI;
            battleUI.TutorialPushButton.onClick.RemoveListener(OnClickTutorialTouch);
            battleUI.TutorialSkipButton.onClick.RemoveListener(OnClickTutorialSkip);
        }
    }

    public void OnClickTutorialTouch()
    {
        if(isWriting)
        {
            SkipWriteText(currentDataIndex, currentTextIndex);
        }
        else
        {
            currentTextIndex++;
            if (currentTextIndex == textLength)
            {
                DeactiveTutorialText();
                return;
            }

            WriteText(currentDataIndex, currentTextIndex);
        }
    }

    public void OnClickTutorialSkip()
    {
        DeactiveTutorialText();
    }

    private void WriteText(int dataIndex, int textIndex)
    {
        string text = tutorialDataDic[dataIndex].Texts[textIndex];

        writeTextCoroutine = WriteTextCoroutine(text);
        StartCoroutine(writeTextCoroutine);
    }

    IEnumerator WriteTextCoroutine(string text)
    {
        BattleUI battleUI = UIManager.Instance.BattleUI;

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

            battleUI.ChangeTutorialText(useText);

            if (index == textLength)
            {
                isWriting = false;
                break;
            }

            pastTime += Time.deltaTime;
            yield return null;
        }
    }

    public void SkipWriteText(int dataIndex, int textIndex)
    {
        StopCoroutine(writeTextCoroutine);
        isWriting = false;

        string text = tutorialDataDic[dataIndex].Texts[textIndex];
        UIManager.Instance.BattleUI.ChangeTutorialText(text);
    }
}

public class BattleTutorialData
{
    public int Index;
    public string[] Texts;
    public int  IndexGoesTo;
}

public class BattleTutorialDataForSave
{
    public BattleTutorialData[] Data;
    public bool IsTutorialOver;
}

public class LoadTutorialData
{
    readonly string FilePath = Application.dataPath + "\\Resources\\Json\\BattleTutorialData.json";

    private JObject root;
    private bool isTutorialOver;

    public void LoadData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/BattleTutorialData");
        string jsonString = textAsset.text;
        Debug.Log($"로드 : { jsonString}");
        root = JObject.Parse(jsonString);

        JToken isOver = root["IsTutorialOver"];

        //BattleManager.Instance.IsTutorialOver = (bool)isOver;
        //isTutorialOver = (bool)isOver;
        isTutorialOver = TutorialManager.Instance.isGameTutorialCompleted;
    }

    public BattleTutorialData[] GetData()
    {
        if (isTutorialOver)
        {
            Debug.Log("배틀 튜토리얼이 종료되어 데이터 받아오지 않음");
            return null;
        }

        JToken Data = root["Data"];

        return JsonConvert.DeserializeObject<BattleTutorialData[]>(Data.ToString());

        //JToken Texts = root["Texts"];

        //return JsonConvert.DeserializeObject<string[][]>(Texts.ToString());
    }

    //public void GetIsTutorialOver()
    //{
    //    JToken isOver = root["IsTutorialOver"];

    //    BattleManager.Instance.isTutorialOver = (bool)isOver;
    //}

    public void SaveData()
    {
        Debug.Log("세이브");
        TutorialManager.Instance.isGameTutorialCompleted = true;
        //string jsonString = JsonConvert.SerializeObject(BattleManager.Instance.BattleTutorial.DataForSave, Formatting.Indented);
        //File.WriteAllText(FilePath, jsonString);
    }
}

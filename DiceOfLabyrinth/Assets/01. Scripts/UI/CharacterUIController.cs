using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterUIController : MonoBehaviour
{
    public static CharacterUIController Instance;
    [Header("Panels")]
    [SerializeField] private GameObject characterPanel;
    [Header("Popups")]
    [SerializeField] private GameObject characterListPopup;
    [SerializeField] private GameObject characterInfoPopup;

    [Header("Character Viewers")]
    [SerializeField] private List<GameObject> ownedCharacterViewers = new List<GameObject>(); // 소유한 캐릭터 뷰어 목록
    [SerializeField] private Transform ownedCharacterViewerParent; // 캐릭터 뷰어가 배치될 부모 오브젝트
    [SerializeField] private List<GameObject> unownedCharacterViewers = new List<GameObject>(); // 소유하지 않은 캐릭터 뷰어 목록
    [SerializeField] private Transform unownedCharacterViewerParent; // 소유하지 않은 캐릭터 뷰어가 배치될 부모 오브젝트

    [Header("Info Popup")]

    [Header("CharacterList")]
    [SerializeField] private List<CharacterSO> unownedCharacters = new List<CharacterSO>(); // 소유하지 않은 캐릭터 목록

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        characterPanel.SetActive(true);
        OpenCharacterListPopup();
    }
    private void OnDisable()
    {
        characterPanel.SetActive(false);
        characterListPopup.SetActive(false);
        characterInfoPopup.SetActive(false);
        foreach (var viewer in ownedCharacterViewers)
        {
            viewer.SetActive(false);
        }
        foreach (var viewer in unownedCharacterViewers)
        {
            viewer.SetActive(false);
        }
    }
    public void OpenCharacterListPopup()
    {
        characterListPopup.SetActive(true);
        characterInfoPopup.SetActive(false);
        ListPopupRefresh();
    }


    private void ListPopupRefresh()
    {
        UnownedCharactersCounting();
        CreateOwnedCharacterViewers();
        CreateUnownedCharacterViewers();
    }
    // 소유하지 않은 캐릭터 목록을 갱신하는 메서드
    private void UnownedCharactersCounting()
    {
        unownedCharacters.Clear();

        var ownedCharIDs = new HashSet<string>(
            CharacterManager.Instance.OwnedCharacters.Select(oc => oc.CharacterData.charID)
        );

        foreach (var pair in CharacterManager.Instance.AllCharacters)
        {
            if (!ownedCharIDs.Contains(pair.Key))
            {
                unownedCharacters.Add(pair.Value);
            }
        }
    }
    // 소유한 캐릭터 뷰어를 생성하는 메서드
    private void CreateOwnedCharacterViewers()
    {
        while (CharacterManager.Instance.OwnedCharacters.Count > ownedCharacterViewers.Count) // 개수가 모자라면 채움
        {
            GameObject viewer = Instantiate(ownedCharacterViewers[0], ownedCharacterViewerParent);
            ownedCharacterViewers.Add(viewer);
        }
        for (int i = 0; i < ownedCharacterViewers.Count; i++)
        {
            if (i < CharacterManager.Instance.OwnedCharacters.Count)
            {
                var characterData = CharacterManager.Instance.OwnedCharacters[i].CharacterData;
                ownedCharacterViewers[i].GetComponent<CharacterViewer>().SetCharacterData(characterData);
                ownedCharacterViewers[i].SetActive(true);
            }
            else
            {
                ownedCharacterViewers[i].SetActive(false);
            }
        }
    }
    // 소유하지 않은 캐릭터 뷰어를 생성하는 메서드
    private void CreateUnownedCharacterViewers()
    {
        while (unownedCharacters.Count > unownedCharacterViewers.Count) // 개수가 모자라면 채움
        {
            GameObject viewer = Instantiate(unownedCharacterViewers[0], unownedCharacterViewerParent);
            unownedCharacterViewers.Add(viewer);
        }
        for (int i = 0; i < unownedCharacterViewers.Count; i++)
        {
            if (i < unownedCharacters.Count)
            {
                var characterData = unownedCharacters[i];
                unownedCharacterViewers[i].GetComponent<CharacterViewer>().SetCharacterData(characterData);
                unownedCharacterViewers[i].SetActive(true);
            }
            else
            {
                unownedCharacterViewers[i].SetActive(false);
            }
        }
    }
    public void OpenCharacterInfoPopup(CharacterSO characterData)
    {
        characterListPopup.SetActive(false);
        characterInfoPopup.SetActive(true);
    }
}

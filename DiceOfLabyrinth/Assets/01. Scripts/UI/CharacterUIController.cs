using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private GameObject basicInfoPopup; // 캐릭터 정보 팝업
    [SerializeField] private GameObject levelUpPopup; // 캐릭터 레벨업 팝업
    [SerializeField] private GameObject skillInfoPopup; // 캐릭터 스킬 정보 팝업

    [Header("Basic Info")]
    [SerializeField] private TMP_Text characterNameText; // 캐릭터 이름 표시용 텍스트
    [SerializeField] private TMP_Text characterlevelText; // 캐릭터 레벨 표시용 텍스트
    [SerializeField] private Image characterClassTypeImage; // 캐릭터 클래스 타입 표시용 이미지
    [SerializeField] private TMP_Text characterClassText; // 캐릭터 클래스 타입 표시용 텍스트
    [SerializeField] private Image characterElementTypeImage; // 캐릭터 엘리먼트 타입 표시용 이미지
    [SerializeField] private TMP_Text characterElementText; // 캐릭터 엘리먼트 타입 표시용 텍스트
    [SerializeField] private Image characterSignatureDiceImage; // 캐릭터 시그니처 주사위 이미지 표시용 이미지
    [SerializeField] private TMP_Text characterSignatureDiceText; // 캐릭터 시그니처 주사위 표시용 텍스트
    [SerializeField] private TMP_Text characterAffectionText; // 캐릭터 애정도 표시용 텍스트


    [Header("Button Colors")]
    [SerializeField] private Color selectedButtonColor = new(170/255f,140/255f,100/255f,1); // 선택된 버튼 색상
    [SerializeField] private Color unselectedButtonColor = new(1,220/255f,170/255f,1); // 선택되지 않은 버튼 색상

    [Header("CharacterList")]
    [SerializeField] private List<CharacterSO> unownedCharacters = new List<CharacterSO>(); // 소유하지 않은 캐릭터 목록
    [SerializeField] private LobbyCharacter selectedCharacter; // 현재 선택된 캐릭터

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
    public void OpenCharacterInfoPopup(LobbyCharacter character)
    {
        characterListPopup.SetActive(false);
        characterInfoPopup.SetActive(true);
    }
}

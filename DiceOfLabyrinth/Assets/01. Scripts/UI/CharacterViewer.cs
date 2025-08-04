using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class CharacterViewer : MonoBehaviour
{
    [Header("Character Data(자동으로 할당 됨)")]
    [SerializeField] private CharacterSO characterData;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text characterNameText; // 캐릭터 이름 표시용 텍스트
    [SerializeField] private Image characterClassTypeImage; // 캐릭터 클래스 타입 표시용 이미지
    [SerializeField] private Image characterElimentTypeImage; // 캐릭터 엘리먼트 표시용 이미지
    [SerializeField] private Image characterUpperImage; // 캐릭터 상체 이미지 표시용 이미지
    [SerializeField] private Image characterSignatureDiceImage; // 캐릭터 시그니처 주사위 이미지 표시용 이미지

    public void SetCharacterData(CharacterSO character)
    {
        characterData = character;
        if (characterData == null) return;
        characterNameText.text = characterData.nameKr;
        characterUpperImage.sprite = characterData.Upper;
        // 이미지들의 스프라이트가 데이터로 만들어지면 추가될 예정
        characterClassTypeImage.sprite = characterData.RoleIcons; // 캐릭터 클래스 타입 아이콘
        characterElimentTypeImage.sprite = characterData.elementIcon; // 캐릭터 엘리먼트 타입 아이콘
        characterSignatureDiceImage.sprite = characterData.DiceNumIcon; // 캐릭터 시그니처 주사위 아이콘

    }
    public void OnClickCharacterViewer()
    {
        // 캐릭터 정보 팝업을 띄우는 로직
        //if (characterData == null || !CharacterManager.Instance.OwnedCharacters.Any(c => c.CharacterData.charID == characterData.charID))
        //{
        //    // 캐릭터가 소유되지 않은 경우
        //    return;
        //}
        //CharacterUIController.Instance.OpenCharacterInfoPopup(
        //    CharacterManager.Instance.OwnedCharacters.FirstOrDefault(c => c.CharacterData.charID == characterData.charID)
        //    );
    }
}

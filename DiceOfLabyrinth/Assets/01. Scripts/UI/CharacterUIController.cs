using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject characterPanel;
    [Header("Popups")]
    [SerializeField] private GameObject characterListPopup;
    [SerializeField] private GameObject characterInfoPopup;

    [Header("CharacterList")]
    [SerializeField] private List<CharacterSO> unownedCharacters = new List<CharacterSO>(); // 소유하지 않은 캐릭터 목록

    private void OnEnable()
    {
        characterPanel.SetActive(true);
        characterListPopup.SetActive(true);
        characterInfoPopup.SetActive(false);
        Refresh();
    }

    private void Refresh()
    {
        UnownedCharactersRefresh();
    }
    // 소유하지 않은 캐릭터 목록을 갱신하는 메서드
    private void UnownedCharactersRefresh()
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
}

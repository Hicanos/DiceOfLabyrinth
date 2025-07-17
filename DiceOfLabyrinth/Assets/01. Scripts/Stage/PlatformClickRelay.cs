using UnityEngine;

public class PlatformClickRelay : MonoBehaviour
{
    public int platformIndex;
    public CharacterSO selectedCharacter => StageManager.Instance.stageSaveData.entryCharacters[platformIndex];
}
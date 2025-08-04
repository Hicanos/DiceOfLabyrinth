using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractDiceArea : MonoBehaviour
{
    [SerializeField] GameObject[] fakeDices;
    public int CurrentActiveIndex;

    private const int diceCount = 5;

    public void SetDiceImages(List<BattleCharacter> characters)
    {
        for (int i = 0; i < diceCount; i++)
        {
            //fakeDices[i].GetComponent<Image>().sprite = characters[i].
        }
    }

    public void ActiveDice()
    {
        fakeDices[CurrentActiveIndex].SetActive(true);
    }

    public void DeactiveDice()
    {
        fakeDices[CurrentActiveIndex].SetActive(false);
    }
}

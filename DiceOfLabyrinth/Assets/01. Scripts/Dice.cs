using UnityEngine;

public class DiceMy : MonoBehaviour
{
    [SerializeField] DiceSO diceSO;
    public int MyIndex { get; private set; }

    public void SetIndex() //맵 입장시 실행하면 될듯
    {
        MyIndex = transform.GetSiblingIndex();
    }    
}

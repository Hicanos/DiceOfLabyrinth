using UnityEngine;

[CreateAssetMenu(fileName = "StagmaData", menuName = "ScriptableObjects/Stages/StagmaData")]
public class StagmaData : ScriptableObject
{
    public string stagmaName;
    public string description;
    public Sprite icon;
    //�ֻ��� ������ ���� �߰� ȿ���� ���� �ʵ��
    public float queenAdditionalDamage; // �� ������ �� �߰� ������ ���
    public float fullHouseAdditionalDamage; // Ǯ�Ͽ콺 ������ �� �߰� ������ ���
    public float quadrupleAdditionalDamage; // 4�� �ֻ����� ���� ������ �� �߰� ������ ���
    public float tripleAdditionalDamage; // 3�� �ֻ����� ���� ������ �� �߰� ������ ���
    public float twoPairAdditionalDamage; // 2���� ���� ������ �� �߰� ������ ���
    public float onePairAdditionalDamage; // 1���� ���� ������ �� �߰� ������ ���
    public float smallStraightAdditionalDamage; // ���� ��Ʈ����Ʈ ������ �� �߰� ������ ���
    public float largeStraightAdditionalDamage; // ���� ��Ʈ����Ʈ ������ �� �߰� ������ ���
    public float noPairAdditionalDamage; // ������ ���� ��� �߰� ������ ���
}

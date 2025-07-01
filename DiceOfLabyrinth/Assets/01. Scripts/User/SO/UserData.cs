using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "User/User Data")]
public class UserData : ScriptableObject
{
    // 계정 정보
    public string nickname = "User";
    public int level = 1;
    public int exp = 0;

    // 재화
    public int stamina = 100;
    //public int currentstamina = 50;
    //public int maxstamina = 50;
    public int gold = 0;
    public int jewel = 0;

    // 편성
}

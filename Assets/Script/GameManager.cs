/*
using UnityEngine;
using System;
using SimpleJSON;  // JSON 파싱용 라이브러리 (or UnityEngine.JsonUtility)

[System.Serializable]
public class UserInfo
{
    public string user_id;
    public string token;
}

public class GameManager : MonoBehaviour
{
    public static string userId;
    public static string jwtToken;

    public void ReceiveUserInfo(string jsonString)
    {
        Debug.Log("Received from JS: " + jsonString);
        UserInfo user = JsonUtility.FromJson<UserInfo>(jsonString);
        userId = user.user_id;
        jwtToken = user.token;
        Debug.Log($"UserID: {userId}, Token: {jwtToken}");
    }
}*/


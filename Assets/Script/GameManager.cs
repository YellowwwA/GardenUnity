using UnityEngine;

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

    void Start()
    {
        Debug.Log("✅ Unity WebGL Start 실행됨");
    }

    public void ReceiveUserInfo(string jsonString)
    {
        Debug.Log("Received from JS: " + jsonString);

        // JsonUtility는 UnityEngine 안에 있음, 별도 using 불필요
        UserInfo user = JsonUtility.FromJson<UserInfo>(jsonString);
        userId = user.user_id;
        jwtToken = user.token;

        Debug.Log($"UserID: {userId}, Token: {jwtToken}");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [Header("설정")]
    public int userId = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 이동 시에도 유지하려면
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnClickSave()
    {
        // ✅ GameManager에서 전달된 userId 사용
        userId = int.TryParse(GameManager.userId, out int parsedId) ? parsedId : 0;
        Debug.Log($"💾 저장 버튼 클릭됨 (userId: {userId})");

        List<Photo> placedPhotos = InventoryManager.Instance.GetPlacedPhotos();

        // ✅ CHANGED: 서버에는 plant_id + placenum만 보냄 (user_id/s3_key 불필요)
        var payload = new SavePayload { photos = new List<SaveItem>() };
        foreach (Photo photo in placedPhotos)
        {
            payload.photos.Add(new SaveItem
            {
                plant_id = photo.plant_id,
                placenum = photo.placenum   // 0이면 해제, >0이면 배치
            });
        }

        string json = JsonUtility.ToJson(payload);
        StartCoroutine(SendSaveRequest(json));
    }

    IEnumerator SendSaveRequest(string json)
    {
        string url = $"https://plantmate.site/unity/api/save_placements";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        request.SetRequestHeader("Authorization", "Bearer " + GameManager.jwtToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ 배치 정보 저장 성공");
        }
        else
        {
            Debug.LogError("❌ 저장 실패: " + request.error);
            Debug.LogError("📦 서버 응답: " + request.downloadHandler.text);
        }
    }
}

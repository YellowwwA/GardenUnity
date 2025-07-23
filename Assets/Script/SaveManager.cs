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
        Debug.Log("💾 저장 버튼 클릭됨");

        List<Photo> placedPhotos = InventoryManager.Instance.GetPlacedPhotos();

        // ✅ 누락된 정보 보완 (user_id, image_url → s3_key로 변경 필요 시 아래 수정)
        foreach (Photo photo in placedPhotos)
        {
            if (string.IsNullOrEmpty(photo.user_id))
                photo.user_id = userId.ToString();

            if (string.IsNullOrEmpty(photo.s3_key))
                photo.s3_key = $"plantimage/pixel_image/{photo.pixel_id}.png";

            // image_url 도 FastAPI에서 받을 경우 필요하면 추가
//            if (string.IsNullOrEmpty(photo.image_url))
//                photo.s3_key = photo.s3_key;
        }

        PhotoListWrapper wrapper = new PhotoListWrapper { photos = placedPhotos };

        string json = JsonUtility.ToJson(wrapper);
        StartCoroutine(SendSaveRequest(json));
    }

    IEnumerator SendSaveRequest(string json)
    {
        string url = $"http://13.208.122.37:8000/api/save_placements";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

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

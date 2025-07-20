using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SaveManager : MonoBehaviour
{
    public int userId = 1;

    public void OnClickSave()
    {
        Debug.Log("💾 저장 버튼 클릭됨");

        List<Photo> placedPhotos = InventoryManager.Instance.GetPlacedPhotos();

        foreach (Photo p in placedPhotos)
        {
            StartCoroutine(SendPhotoToServer(p));
        }
    }

    IEnumerator SendPhotoToServer(Photo photo)
    {
        string url = $"http://13.208.122.37:8000/user/{userId}/photos/{photo.pixel_id}";
        string jsonData = JsonUtility.ToJson(photo);

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                Debug.Log($"✅ 저장 완료: photo_id={photo.pixel_id}");
            else
                Debug.LogError($"❌ 저장 실패: photo_id={photo.pixel_id}, error={request.error}");
        }
    }
}

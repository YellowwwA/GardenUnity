using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// C# 프로젝트 내 어딘가에 아래 클래스들이 정의되어 있어야 합니다.
// [System.Serializable]
// public class Photo
// {
//     public int plant_id;
//     public int user_id;
//     public int placenum;
//     public string image_url;
// }
//
// [System.Serializable]
// public class PhotoListWrapper
// {
//     public List<Photo> photos;
// }

public class PhotoLoader : MonoBehaviour
{
    public int userId = 1;
    public Transform inventoryParent;
    public GameObject photoPrefabTemplate;

    void Start()
    {
        // 🔽 GameManager의 userId를 참조해서 설정
        userId = int.TryParse(GameManager.userId, out int parsedId) ? parsedId : 1;

        Debug.Log($"📦 PhotoLoader에서 받은 userId: {userId}");

        StartCoroutine(LoadPhotos());
    }

    IEnumerator LoadPhotos()
    {
        string url = "https://plantmate.site/unity/api/s3photos";  // ✅ user_id 제거된 버전

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // ✅ Authorization 헤더 추가
            request.SetRequestHeader("Authorization", "Bearer " + GameManager.jwtToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawJson = request.downloadHandler.text;

                // ✅ 수정된 부분: 받은 배열 JSON을 객체 형태로 감싸줍니다.
                string wrappedJson = "{\"photos\":" + rawJson + "}";
                PhotoListWrapper data = JsonUtility.FromJson<PhotoListWrapper>(wrappedJson);

                // 이제 data.photos에 정상적으로 접근할 수 있습니다.
                if (data != null && data.photos != null)
                {
                    foreach (Photo p in data.photos)
                        yield return StartCoroutine(DownloadImageAndInstantiate(p));
                }
            }
            else
            {
                Debug.LogError("❌ Error fetching photo data: " + request.error);
                Debug.LogError("📦 서버 응답 내용: " + request.downloadHandler.text);
            }
        }
    }

    IEnumerator DownloadImageAndInstantiate(Photo p)
    {
        // ✅ 수정된 부분: image_url_unity 확인 로직을 제거하고 image_url 필드를 직접 사용합니다.
        string imgUrl = p.image_url;

        if (string.IsNullOrEmpty(imgUrl))
        {
            Debug.LogWarning($"⚠️ image url 비어있음 (plant_id={p.plant_id})");
            yield break;
        }

        // 프록시 URL은 same-origin이라 CORS 문제 없음
        using (UnityWebRequest imgRequest = UnityWebRequestTexture.GetTexture(imgUrl, true))
        {
            yield return imgRequest.SendWebRequest();

            if (imgRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ Image download failed for plant_id={p.plant_id}: {imgRequest.error}");
                yield break;
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(imgRequest);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            GameObject photoObj = Instantiate(photoPrefabTemplate, inventoryParent);
            photoObj.name = $"photo_{p.plant_id}";

            SpriteRenderer renderer = photoObj.GetComponentInChildren<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = sprite;
                photoObj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            }

            PhotoItem item = photoObj.GetComponent<PhotoItem>();
            if (item != null)
                item.photoData = p;

            yield return new WaitForEndOfFrame();
            UpdateColliderToFitSprite(photoObj);

            if (p.placenum == 0)
                InventoryManager.Instance.AddPhotoItem(p, photoObj);
            else
                DropToWorld.DropItemAtStartup(p, photoObj);
        }
    }

    private void UpdateColliderToFitSprite(GameObject obj)
    {
        SpriteRenderer renderer = obj.GetComponentInChildren<SpriteRenderer>();
        if (renderer == null || renderer.sprite == null) return;

        BoxCollider col = obj.GetComponent<BoxCollider>();
        if (col == null) return;

        float height = renderer.sprite.bounds.size.y;
        col.size = new Vector3(1f, height, 1f);
        col.center = new Vector3(0f, height / 2f, 0f);
    }
}
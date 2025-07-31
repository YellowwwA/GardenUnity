using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PhotoLoader : MonoBehaviour
{
    public int userId = 1;
    public Transform inventoryParent;
    public GameObject photoPrefabTemplate;

    void Start()
    {
        // 🔽 GameManager의 userId를 참조해서 설정
        userId = int.TryParse(GameManager.userId, out int parsedId) ? parsedId : 0;

        Debug.Log($"📦 PhotoLoader에서 받은 userId: {userId}");

        StartCoroutine(LoadPhotos());
    }

    IEnumerator LoadPhotos()
    {
        string url = $"http://13.208.122.37:8000/api/s3photos/{userId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawJson = request.downloadHandler.text;
                string wrappedJson = "{\"photos\":" + rawJson + "}";
                PhotoListWrapper data = JsonUtility.FromJson<PhotoListWrapper>(wrappedJson);

                foreach (Photo p in data.photos)
                    yield return StartCoroutine(DownloadImageAndInstantiate(p));
            }
            else
            {
                Debug.LogError("❌ Error fetching photo data: " + request.error);
            }
        }
    }

    IEnumerator DownloadImageAndInstantiate(Photo p)
    {
        using (UnityWebRequest imgRequest = UnityWebRequestTexture.GetTexture(p.image_url))
        {
            yield return imgRequest.SendWebRequest();

            if (imgRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ Image download failed for {p.s3_key}: {imgRequest.error}");
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

            // Sprite 설정이 끝난 이후에 콜라이더 설정
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
        col.center = new Vector3(0f, height / 2f, 0f); // 중심을 Sprite의 중간으로 이동
    }
}
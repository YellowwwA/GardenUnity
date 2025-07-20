using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PhotoLoader : MonoBehaviour
{
    public int userId = 1;

    void Start()
    {
        StartCoroutine(LoadPhotos(userId));
    }

    IEnumerator LoadPhotos(int userId)
    {
        string url = $"http://13.208.122.37:8000/user/{userId}/photos";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawJson = request.downloadHandler.text;
                string wrappedJson = "{\"photos\":" + rawJson + "}";
                PhotoListWrapper data = JsonUtility.FromJson<PhotoListWrapper>(wrappedJson);

                foreach (Photo p in data.photos)
                {
                    if (p.placenum == 0)
                    {
                        InventoryManager.Instance.AddPhotoItem(p);
                    }
                    else
                    {
                        DropToWorld.DropItemAtStartup(p);
                    }
                }

            }
            else
            {
                Debug.LogError("Error fetching photo data: " + request.error);
            }
        }
    }
}

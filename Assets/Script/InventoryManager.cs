using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject itemUIPrefab;
    public Transform inventoryPanel;

    private List<ItemData> currentItems = new List<ItemData>();
    private List<Photo> placedPhotos = new List<Photo>();

    private void Awake()
    {
        Instance = this;
    }


    public void RegisterPlacedPhoto(Photo photo)
    {
        // 이미 동일 pixel_id가 있으면 제거
        placedPhotos.RemoveAll(p => p.pixel_id == photo.pixel_id);
        placedPhotos.Add(photo);
    }

    public void UnregisterPlacedPhoto(Photo photo)
    {
        if (placedPhotos.Contains(photo))
            placedPhotos.Remove(photo);
    }

    public List<Photo> GetPlacedPhotos()
    {
        return placedPhotos;
    }



    public void AddPhotoItem(Photo p)
    {
        string id = p.pixel_id.ToString();

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{id}");
        Sprite icon = Resources.Load<Sprite>($"Icons/{id}");

        if (prefab == null || icon == null)
        {
            Debug.LogWarning($"❌ 프리팹 또는 아이콘 {id} 없음");
            return;
        }

        ItemData item = new ItemData
        {
            itemName = $"Photo {id}",
            icon = icon,
            worldPrefab = prefab,
            photo_id = p.pixel_id  // ✅ 꼭 있어야 함!
        };

        AddItem(item);

        // 📌 배치된 것이 아니어도 저장 대상에 등록해줘야 함 (placenum=0 포함)
        RegisterPlacedPhoto(p);
    }

    public void AddItem(ItemData itemData)
    {
        GameObject newItem = Instantiate(itemUIPrefab, inventoryPanel);
        newItem.GetComponent<ItemUI>().SetItem(itemData);
        currentItems.Add(itemData);
    }

    public void RemoveItem(ItemData itemData)
    {
        Transform found = null;
        foreach (Transform child in inventoryPanel)
        {
            var ui = child.GetComponent<ItemUI>();
            if (ui != null && ui.itemData == itemData)
            {
                found = child;
                break;
            }
        }

        if (found != null)
        {
            Destroy(found.gameObject);
            currentItems.Remove(itemData);
        }
    }


}

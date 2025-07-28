using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject itemUIPrefab;
    public Transform inventoryPanel;

    private List<ItemData> currentItems = new List<ItemData>();
    private List<Photo> placedPhotos = new List<Photo>();
    private Dictionary<int, GameObject> placedPhotoObjects = new Dictionary<int, GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    // ✅ 배치 등록 (GameObject 포함)
    public void RegisterPlacedPhoto(Photo photo, GameObject obj)
    {
        placedPhotos.RemoveAll(p => p.plant_id == photo.plant_id);
        placedPhotos.Add(photo);
        placedPhotoObjects[photo.plant_id] = obj;
    }

    // ✅ 기존 방식 유지 (GameObject 없이)
    public void RegisterPlacedPhoto(Photo photo)
    {
        placedPhotos.RemoveAll(p => p.plant_id == photo.plant_id);
        placedPhotos.Add(photo);
    }

    // ✅ 배치 해제 및 오브젝트 제거
    public void UnregisterPlacedPhoto(Photo photo)
    {
        placedPhotos.RemoveAll(p => p.plant_id == photo.plant_id);

        if (placedPhotoObjects.TryGetValue(photo.plant_id, out GameObject obj))
        {
            Destroy(obj);
            placedPhotoObjects.Remove(photo.plant_id);
        }
    }

    public List<Photo> GetPlacedPhotos()
    {
        return placedPhotos;
    }

    // ✅ 회수 처리: 인벤토리에 다시 등록
    public void RecallPhoto(Photo photo, GameObject existingObj)
    {
        UnregisterPlacedPhoto(photo);

        Sprite icon = existingObj.GetComponentInChildren<SpriteRenderer>()?.sprite;
        if (icon == null)
        {
            Debug.LogWarning($"❌ Sprite 없음 for {photo.plant_id}");
            return;
        }

        photo.placenum = 0;

        // 🔐 인벤토리 중복 제거 (UI + currentItems 둘 다)
        currentItems.RemoveAll(i => i.photo_id == photo.plant_id);
        foreach (Transform child in inventoryPanel)
        {
            var ui = child.GetComponent<ItemUI>();
            if (ui != null && ui.itemData.photo_id == photo.plant_id)
            {
                Destroy(child.gameObject);
            }
        }

        // 🔍 이미 클론해둔 게 있다면 재사용
        if (placedPhotoObjects.TryGetValue(photo.plant_id, out GameObject existingClone))
        {
            if (!existingClone.activeInHierarchy)
            {
                Debug.Log($"♻️ 기존 클론 재사용 (plant_id={photo.plant_id})");

                ItemData item = new ItemData
                {
                    itemName = $"Photo {photo.plant_id}",
                    icon = icon,
                    worldPrefab = existingClone,
                    photo_id = photo.plant_id
                };

                AddItem(item);
                RegisterPlacedPhoto(photo, existingClone);

                Destroy(existingObj);
                return;
            }
        }

        // ✅ 클론 없거나 기존 클론이 active 상태일 경우 → 새로 클론 후 비활성화
        GameObject newPrefab = Instantiate(existingObj);
        newPrefab.SetActive(false);

        placedPhotoObjects[photo.plant_id] = newPrefab;

        ItemData newItem = new ItemData
        {
            itemName = $"Photo {photo.plant_id}",
            icon = icon,
            worldPrefab = newPrefab,
            photo_id = photo.plant_id
        };

        AddItem(newItem);
        RegisterPlacedPhoto(photo, newPrefab);

        Destroy(existingObj);
        Debug.Log($"🧩 새로운 클론 생성 및 등록 (plant_id={photo.plant_id})");
    }










    // ✅ Resources 방식 인벤토리 등록
    /*    public void AddPhotoItem(Photo p)
        {
            string id = p.plant_id.ToString();

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
                photo_id = p.plant_id
            };

            AddItem(item);
            RegisterPlacedPhoto(p);
        }*/

    // ✅ S3 기반 인벤토리 등록 (Photo 정보 보완 포함)
    public void AddPhotoItem(Photo photo, GameObject prefabInstance)
    {
        Sprite icon = prefabInstance.GetComponentInChildren<SpriteRenderer>()?.sprite;
        if (icon == null)
        {
            Debug.LogWarning($"❌ 아이콘 스프라이트 없음 for photo {photo.plant_id}");
            return;
        }

        // ✅ 누락된 정보 보완
        photo.user_id = SaveManager.Instance.userId;
        photo.s3_key = string.IsNullOrEmpty(photo.s3_key)
            ? $"plantimage/pixel_image/{photo.plant_id}.png"
            : photo.s3_key;

        ItemData item = new ItemData
        {
            itemName = $"Photo {photo.plant_id}",
            icon = icon,
            worldPrefab = prefabInstance,
            photo_id = photo.plant_id
        };

        AddItem(item);
        RegisterPlacedPhoto(photo, prefabInstance);
    }

    public void AddItem(ItemData itemData)
    {
        // 이미 동일 photo_id 가진 항목이 있으면 UI에 중복 등록 안되게 방지
        if (currentItems.Exists(i => i.photo_id == itemData.photo_id))
        {
            Debug.LogWarning($"⚠️ 이미 존재하는 photo_id: {itemData.photo_id}, AddItem 중단");
            return;
        }

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

    public void RemoveItemByPhotoId(int photoId)
    {
        // 아이템 데이터 제거
        currentItems.RemoveAll(i => i.photo_id == photoId);

        // UI 제거
        Transform toRemove = null;
        foreach (Transform child in inventoryPanel)
        {
            ItemUI ui = child.GetComponent<ItemUI>();
            if (ui != null && ui.itemData.photo_id == photoId)
            {
                toRemove = child;
                break;
            }
        }

        if (toRemove != null)
        {
            Destroy(toRemove.gameObject);
        }
    }

}

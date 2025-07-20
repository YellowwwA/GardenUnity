using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropToWorld : MonoBehaviour
{
    public static DropToWorld Instance;

    public GameObject arrowHint; // 👈 드래그 가이드 화살표

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// placenum이 있는 사진을 시작 시 배치
    /// </summary>
    public static void DropItemAtStartup(Photo photo)
    {
        if (photo == null) return;

        int index = photo.placenum - 1;
        if (index < 0 || index >= DropPointManager.Instance.dropPoints.Length)
        {
            Debug.LogWarning($"❌ placenum {photo.placenum}은 유효하지 않은 dropPoint입니다.");
            return;
        }

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{photo.pixel_id}");
        if (prefab == null)
        {
            Debug.LogWarning($"❌ 프리팹 {photo.pixel_id} 없음 (Resources/Prefabs/{photo.pixel_id}.prefab)");
            return;
        }

        Vector3 point = DropPointManager.Instance.dropPoints[index].position;
        Vector3 offset = new Vector3(0f, 1f, 0f);
        Quaternion rotation = Quaternion.Euler(10f, 0f, 0f);

        GameObject item = Instantiate(prefab, point + offset, rotation);

        var clickable = item.AddComponent<WorldClickableItem>();
        clickable.dropPointPosition = point;
        clickable.photoData = photo;

        DropPointManager.Instance.ReserveDropPoint(index);

        // ✅ 등록
        InventoryManager.Instance.RegisterPlacedPhoto(photo);
    }

    /// <summary>
    /// 인벤토리에서 드래그로 배치
    /// </summary>
    public static void DropItem(ItemData itemData, Vector3 dropPos)
    {
        if (itemData == null || itemData.worldPrefab == null)
            return;

        if (DropPointManager.Instance.IsValidDropPosition(dropPos, out Vector3 nearestDropPoint))
        {
            Vector3 offset = new Vector3(0f, 1f, 0f);
            Quaternion rotation = Quaternion.Euler(10f, 0f, 0f);

            GameObject item = Instantiate(itemData.worldPrefab, nearestDropPoint + offset, rotation);

            var clickable = item.AddComponent<WorldClickableItem>();
            clickable.dropPointPosition = nearestDropPoint;
            clickable.itemData = itemData;

            // 🧠 drop index 찾기
            int dropIndex = DropPointManager.Instance.GetDropPointIndex(nearestDropPoint);
            if (dropIndex == -1)
            {
                Debug.LogWarning("⚠ 드롭 위치 인덱스 찾기 실패");
                return;
            }

            // ✅ 새 Photo 생성 및 등록
            Photo newPhoto = new Photo
            {
                pixel_id = itemData.photo_id, // itemData에 photo_id 필드 있어야 함!
                placenum = dropIndex + 1
            };

            clickable.photoData = newPhoto;
            InventoryManager.Instance.RegisterPlacedPhoto(newPhoto);

            // ✅ 화살표 제거
            if (Instance.arrowHint != null)
            {
                Destroy(Instance.arrowHint);
                Debug.Log("🧭 화살표 비활성화됨");
            }

            Debug.Log("✅ 드롭 성공!");
        }
        else
        {
            Debug.Log("❌ 드롭 실패: 유효한 위치 아님");
        }
    }
}

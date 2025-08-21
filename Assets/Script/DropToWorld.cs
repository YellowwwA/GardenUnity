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
    public static void DropItemAtStartup(Photo photo, GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"❌ 전달받은 프리팹이 null");
            return;
        }

        int index = photo.placenum - 1;
        if (index < 0 || index >= DropPointManager.Instance.dropPoints.Length)
        {
            Debug.LogWarning($"❌ placenum {photo.placenum}은 유효하지 않은 dropPoint입니다.");
            return;
        }

        Vector3 point = DropPointManager.Instance.dropPoints[index].position;
        Vector3 offset = new Vector3(0f, 1.1f, 0f);
        Quaternion rotation = Quaternion.Euler(10f, 0f, 0f);

        GameObject item = Instantiate(prefab, point + offset, rotation);
        item.SetActive(true); // ✅ 반드시 활성화시켜야 화면에 나타남

        SetupVisuals(item);

        var clickable = item.AddComponent<WorldClickableItem>();
        clickable.dropPointPosition = point;
        clickable.photoData = photo;

        DropPointManager.Instance.ReserveDropPoint(index);
        InventoryManager.Instance.RegisterPlacedPhoto(photo, item);
    }


    /// <summary>
    /// 인벤토리에서 드래그로 배치
    /// </summary>
    public static bool DropItem(ItemData itemData, Vector3 dropPos)
    {
        if (itemData == null || itemData.worldPrefab == null)
        {
            Debug.LogError($"❌ worldPrefab이 null입니다. photo_id = {itemData?.photo_id}");
            return false;
        }

        if (!DropPointManager.Instance.IsValidDropPosition(dropPos, out Vector3 nearestDropPoint))
        {
            Debug.Log("❌ 드롭 실패: 유효한 위치 아님");
            return false; // ❌ 인벤토리 유지하고 아무 작업도 하지 않음
        }

        Vector3 offset = new Vector3(0f, 1.1f, 0f);
        Quaternion rotation = Quaternion.Euler(10f, 0f, 0f);

        GameObject item;

        // ✅ 비활성화된 프리팹이라면 재사용
        if (!itemData.worldPrefab.activeSelf)
        {
            if (itemData.worldPrefab.scene.IsValid())
            {
                Debug.Log("♻️ 씬에 존재하는 worldPrefab 감지 → 복사");
                item = GameObject.Instantiate(itemData.worldPrefab);
            }
            else
            {
                item = itemData.worldPrefab;
            }

            item.transform.position = nearestDropPoint + offset;
            item.transform.rotation = rotation;
            item.SetActive(true);

            Debug.Log("♻️ 회수된 프리팹 재사용");
        }
        else
        {
            // 이미 활성화 상태이면 새 인스턴스 생성
            item = GameObject.Instantiate(itemData.worldPrefab, nearestDropPoint + offset, rotation);
            item.SetActive(true);
            Debug.Log("✨ 새 프리팹 인스턴스 생성");
        }

        SetupVisuals(item);

        // LookAtPlayer 활성화
        LookAtPlayer look = item.GetComponent<LookAtPlayer>();
        if (look != null)
        {
            look.enabled = true;
        }

        var clickable = item.AddComponent<WorldClickableItem>();
        clickable.dropPointPosition = nearestDropPoint;
        clickable.itemData = itemData;

        int dropIndex = DropPointManager.Instance.GetDropPointIndex(nearestDropPoint);
        if (dropIndex == -1)
        {
            Debug.LogWarning("⚠ 드롭 위치 인덱스 찾기 실패");
            Destroy(item); // 생성된 오브젝트 제거
            return false;
        }

        Photo newPhoto = new Photo
        {
            plant_id = itemData.photo_id,
            placenum = dropIndex + 1
        };

        clickable.photoData = newPhoto;
        InventoryManager.Instance.RegisterPlacedPhoto(newPhoto, item);

        // ✅ 여기서만 인벤토리 제거: 드롭이 성공했을 때만
        InventoryManager.Instance.RemoveItem(itemData);

        if (Instance.arrowHint != null)
        {
            Destroy(Instance.arrowHint);
            Debug.Log("🧭 화살표 비활성화됨");
        }

        Debug.Log("✅ 드롭 성공!");
        return true;
    }



    /// <summary>
    /// 스프라이트 기반 크기 및 콜라이더 자동 설정
    /// </summary>
    private static void SetupVisuals(GameObject obj)
    {
        SpriteRenderer renderer = obj.GetComponentInChildren<SpriteRenderer>();
        if (renderer != null && renderer.sprite != null)
        {
            obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            float height = renderer.sprite.bounds.size.y;

            BoxCollider col = obj.GetComponent<BoxCollider>();
            if (col != null)
            {
                col.size = new Vector3(1f, height, 1f);
                col.center = new Vector3(0f, 0f, 0f);
            }
        }
        else
        {
            Debug.LogWarning("❗ SpriteRenderer 또는 sprite가 없습니다.");
        }
    }
}

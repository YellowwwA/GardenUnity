using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static ItemData draggingItem;
    private GameObject dragImageObj;
    private Image dragImage;

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggingItem = GetComponent<ItemUI>().itemData;

        // 드래그 이미지 생성
        dragImageObj = new GameObject("DragItem");
        dragImageObj.transform.SetParent(GameObject.Find("DragCanvas").transform, false);
        dragImage = dragImageObj.AddComponent<Image>();
        dragImage.sprite = draggingItem.icon;
        dragImage.raycastTarget = false;

        RectTransform rt = dragImage.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(60, 60);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragImageObj != null)
        {
            dragImageObj.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 UI 제거
        if (dragImageObj != null)
        {
            Destroy(dragImageObj);
        }

        if (draggingItem == null || draggingItem.worldPrefab == null)
        {
            Debug.LogWarning("❌ 드래그 중인 아이템이나 프리팹이 null입니다.");
            draggingItem = null;
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 dropPosition = hit.point;

            // ✅ 드롭 처리 위임
            DropToWorld.DropItem(draggingItem, dropPosition);
        }
        else
        {
            Debug.Log("❌ 유효한 드롭 위치가 아닙니다.");
        }

        // ✅ 인벤토리 제거
        InventoryManager.Instance.RemoveItem(draggingItem);

        draggingItem = null;
    }
}

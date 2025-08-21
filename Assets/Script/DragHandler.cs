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

        bool success = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 dropPosition = hit.point;
            success = DropToWorld.DropItem(draggingItem, dropPosition);  // ✅ 성공 여부 확인
        }
        else
        {
            Debug.Log("❌ 유효한 드롭 위치가 아닙니다.");
        }

        if (success)
        {
            InventoryManager.Instance.RemoveItem(draggingItem);  // ✅ 성공했을 때만 제거
        }

        draggingItem = null;
    }

}

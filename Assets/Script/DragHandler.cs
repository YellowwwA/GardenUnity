using System.Collections;
using System.Collections.Generic;
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
        dragImageObj.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            DropToWorld.DropItem(draggingItem, hit.point);

            // ✅ 드롭 성공 후 인벤토리에서 제거
            InventoryManager.Instance.RemoveItem(draggingItem);
        }

        Destroy(dragImageObj);
        draggingItem = null;
    }
}

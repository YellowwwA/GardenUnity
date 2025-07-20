using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldClickableItem : MonoBehaviour
{
    public Vector3 dropPointPosition;
    public Photo photoData;       // API로 받아온 데이터 기반 회수
    public ItemData itemData;     // 인벤토리에서 드래그한 경우 회수할 정보

    private void OnMouseDown()
    {
        if (photoData != null)
        {
            photoData.placenum = 0;

            InventoryManager.Instance.UnregisterPlacedPhoto(photoData);
            InventoryManager.Instance.RegisterPlacedPhoto(photoData);
            InventoryManager.Instance.AddPhotoItem(photoData);
        }

        DropPointManager.Instance.ReleaseDropPoint(dropPointPosition);
        Destroy(gameObject);
    }
}
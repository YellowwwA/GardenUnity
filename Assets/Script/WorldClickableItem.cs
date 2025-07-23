using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldClickableItem : MonoBehaviour
{
    public Vector3 dropPointPosition;
    public Photo photoData;       // API로 받아온 사진 정보
    public ItemData itemData;     // 인벤토리에서 드래그한 경우

    private void OnMouseDown()
    {
        if (photoData != null)
        {
            photoData.placenum = 0;

            InventoryManager.Instance.UnregisterPlacedPhoto(photoData);

            // 인벤토리 아이템 추가
            InventoryManager.Instance.RecallPhoto(photoData, this.gameObject);  // ✅ 현재 오브젝트 넘김

            // 드롭 포인트 해제
            DropPointManager.Instance.ReleaseDropPoint(dropPointPosition);

            // ✅ 마지막에 오브젝트 제거
            Destroy(gameObject);
        }
    }

}

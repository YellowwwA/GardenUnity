using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public ItemData itemData;
    public Image icon;

    public void SetItem(ItemData data)
    {
        itemData = data;
        icon.sprite = data.icon;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData data;
    public int onGridPosX;
    public int onGridPosY;

    internal void Set(ItemData itemData)
    {
        data = itemData;

        GetComponent<Image>().sprite = data.itemIcon;

        Vector2 size = new Vector2(data.width * ItemGrid.tileSizeWidth, data.height * ItemGrid.tileSizeHeight);

        GetComponent<RectTransform>().sizeDelta = size;
    }
}

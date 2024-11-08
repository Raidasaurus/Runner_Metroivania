using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public ItemGrid selectedItemGrid;


    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectT;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;
    
    void Update()
    {
        if (selectedItem != null) rectT.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateRandomItem();
        }

        if (selectedItemGrid == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleItem();
        }
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectT = inventoryItem.GetComponent<RectTransform>();
        rectT.SetParent(canvasTransform);

        int rand = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[rand]);

    }

    private void HandleItem()
    {
        Vector2 pos = Input.mousePosition;

        if (selectedItem != null)
        {
            pos.x -= (selectedItem.data.width - 1) * ItemGrid.tileSizeWidth / 2;
            pos.y += (selectedItem.data.height - 1) * ItemGrid.tileSizeHeight / 2;
        }
        Vector2Int tileGridPos = selectedItemGrid.GetGridPosition(pos);

        if (selectedItem == null)
        {
            PickUpItem(tileGridPos);
        }
        else
        {
            PlaceItem(tileGridPos);
        }
    }

    private void PlaceItem(Vector2Int tileGridPos)
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPos.x, tileGridPos.y, ref overlapItem);
        if (complete)
        {
            selectedItem = null;
            if (overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectT = selectedItem.GetComponent<RectTransform>();
            }
        }
    }

    private void PickUpItem(Vector2Int tileGridPos)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPos.x, tileGridPos.y);
        if (selectedItem != null)
        {
            rectT = selectedItem.GetComponent<RectTransform>();
        }
    }
}

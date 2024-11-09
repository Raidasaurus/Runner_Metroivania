using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    private ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid { get => selectedItemGrid; set { selectedItemGrid = value; highlight.SetParent(value); } }


    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectT;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight highlight;


    private void Awake()
    {
        highlight = GetComponent<InventoryHighlight>();    
    }

    void Update()
    {
        if (selectedItem != null) rectT.position = Input.mousePosition;


        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateRandomItem();
        }

        if (selectedItemGrid == null)
        {
            highlight.Show(false);
            return;
        }
        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            HandleItem();
        }
    }

    InventoryItem itemToHighlight;
    Vector2Int oldPos;

    private void HandleHighlight()
    {
        Vector2Int posOnGrid = GetTileGridPosition();

        if (oldPos == posOnGrid) return;
        oldPos = posOnGrid;

        if (selectedItem == null)
        {
            itemToHighlight = selectedItemGrid.GetItem(posOnGrid.x, posOnGrid.y);
            if (itemToHighlight != null)
            {
                highlight.Show(true);
                highlight.SetSize(itemToHighlight);
                highlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
            {
                highlight.Show(false);
            }
        }
        else
        {
            highlight.Show(selectedItemGrid.BoundaryCheck(posOnGrid.x, posOnGrid.y, selectedItem.data.width, selectedItem.data.height));
            highlight.SetSize(selectedItem);
            highlight.SetPosition(selectedItemGrid, selectedItem, posOnGrid.x, posOnGrid.y);
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
        Vector2Int tileGridPos = GetTileGridPosition();

        if (selectedItem == null)
        {
            PickUpItem(tileGridPos);
        }
        else
        {
            PlaceItem(tileGridPos);
        }
    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 pos = Input.mousePosition;

        if (selectedItem != null)
        {
            pos.x -= (selectedItem.data.width - 1) * ItemGrid.tileSizeWidth / 2;
            pos.y += (selectedItem.data.height - 1) * ItemGrid.tileSizeHeight / 2;
        }

        return selectedItemGrid.GetGridPosition(pos);
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

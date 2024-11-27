using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] private ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid { get => selectedItemGrid; set { selectedItemGrid = value; highlight.SetParent(value); } }


    [SerializeField] InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectT;

    [SerializeField] ItemGrid inventoryGrid;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    [SerializeField] Transform playerPos;
    [SerializeField] GameObject itemObjectPrefab;
    [SerializeField] PlayerManager pm;

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

        if (Input.GetMouseButtonDown(0))
        {
            HandleItem();
        }

        if (selectedItemGrid == null)
        {
            highlight.Show(false);
            return;
        }
        HandleHighlight();


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

    public bool AddItem(Item item)
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;
        inventoryItem.Set(item.data);

        Vector2Int? posOnGrid = inventoryGrid.FindSpace(inventoryItem);

        if (posOnGrid == null)
        {
            Destroy(inventoryItem.gameObject);
            return false;
        }

        rectT = inventoryItem.GetComponent<RectTransform>();
        rectT.SetParent(canvasTransform);

        selectedItem = null;
        InsertItem(inventoryItem, inventoryGrid);

        return true;
    }

    private void HandleItem()
    {
        if (selectedItem == null && selectedItemGrid != null)
        {
            Vector2Int tileGridPos = GetTileGridPosition();
            PickUpItem(tileGridPos);
        }
        else
        {
            if (selectedItemGrid != null)
            {
                Vector2Int tileGridPos = GetTileGridPosition();
                PlaceItem(tileGridPos);
            }
            else if (!pm.cam.lockCursor && selectedItem != null)
            {
                Debug.Log("Test");
                var temp = Instantiate(itemObjectPrefab, playerPos.position, Quaternion.identity);
                temp.GetComponent<Rigidbody>().AddForce(Vector3.up * 5f);
                temp.GetComponent<Item>().data = selectedItem.data;
                Destroy(selectedItem.gameObject);
            }
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
                rectT.SetAsLastSibling();
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

    private void InsertItem(InventoryItem item, ItemGrid grid)
    {
        Vector2Int? posOnGrid = grid.FindSpace(item);

        if (posOnGrid == null) return;

        grid.PlaceItem(item, posOnGrid.Value.x, posOnGrid.Value.y);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;

    InventoryItem[,] inventoryItemSlot;

    [SerializeField] int gridSizeWidth = 12;
    [SerializeField] int gridSizeHeight = 5;

    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
    }

    void Init(int width, int height)
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }

    Vector2 posOnGrid = new Vector2();
    Vector2Int tileGridPos = new Vector2Int();
    public Vector2Int GetGridPosition(Vector2 mousePos)
    {
        posOnGrid.x = mousePos.x - rectTransform.position.x - 0.001f;
        posOnGrid.y = rectTransform.position.y - mousePos.y - 0.001f;

        tileGridPos.x = (int)(posOnGrid.x / tileSizeWidth);
        tileGridPos.y = (int)(posOnGrid.y / tileSizeHeight);

        return tileGridPos;
    }

    internal InventoryItem GetItem(int x, int y)
    {
        InventoryItem temp = inventoryItemSlot[x, y];
        if (temp == null) return null;
        else return temp;
    }

    public bool PlaceItem(InventoryItem item, int xPos, int yPos, ref InventoryItem overlapItem)
    {
        if (BoundaryCheck(xPos, yPos, item.data.width, item.data.height) == false) return false;

        if (OverlapCheck(xPos, yPos, item.data.width, item.data.height, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }

        RectTransform rectTransform = item.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for (int x = 0; x < item.data.width; x++)
        {
            for (int y = 0; y < item.data.height; y++)
            {
                inventoryItemSlot[xPos + x, yPos + y] = item;
            }
        }

        item.onGridPosX = xPos;
        item.onGridPosY = yPos;

        Vector2 pos = CalculatePositionOnGrid(item, xPos, yPos);

        rectTransform.localPosition = pos;
        return true;
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem item, int xPos, int yPos)
    {
        Vector2 pos = new Vector2();
        pos.x = xPos * tileSizeWidth + (tileSizeWidth * item.data.width / 2);
        pos.y = -(yPos * tileSizeHeight + (tileSizeHeight * item.data.height / 2));
        return pos;
    }

    private bool OverlapCheck(int xPos, int yPos, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[xPos + x, yPos + y] != null)
                {
                    if (overlapItem == null)
                        overlapItem = inventoryItemSlot[xPos + x, yPos + y];
                    else
                        if (overlapItem != inventoryItemSlot[xPos + x, yPos + y])
                            return false;
                }
            }
        }

        return true;
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null) return null;
        CleanGridReference(toReturn);

        return toReturn;
    }

    private void CleanGridReference(InventoryItem item)
    {
        for (int ix = 0; ix < item.data.width; ix++)
        {
            for (int iy = 0; iy < item.data.height; iy++)
            {
                inventoryItemSlot[item.onGridPosX + ix, item.onGridPosY + iy] = null;
            }
        }
    }

    bool PositionCheck(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }

        if (x >= gridSizeWidth || y >= gridSizeHeight)
        {
            return false;
        }

        return true;
    }

    public bool BoundaryCheck(int x, int y, int w, int h)
    {
        if (PositionCheck(x, y) == false) return false;

        x += w - 1;
        y += h - 1;

        if (PositionCheck(x, y) == false) return false;


        return true;
    }    

}

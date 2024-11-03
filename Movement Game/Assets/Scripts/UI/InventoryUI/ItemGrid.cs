using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    const float tileSizeWidth = 32;
    const float tileSizeHeight = 32;

    InventoryItem[,] inventoryItemSlot;

    [SerializeField] int gridSizeWidth = 12;
    [SerializeField] int gridSizeHeight = 5;

    RectTransform rectTransform;

    public GameObject testPrefab;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
        InventoryItem testItem = Instantiate(testPrefab.GetComponent<InventoryItem>());
        PlaceItem(testItem, 1, 1);
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
        posOnGrid.x = mousePos.x - rectTransform.position.x;
        posOnGrid.y = rectTransform.position.y - mousePos.y;

        tileGridPos.x = (int)(posOnGrid.x / tileSizeWidth);
        tileGridPos.y = (int)(posOnGrid.y / tileSizeHeight);

        return tileGridPos;
    }

    public void PlaceItem(InventoryItem item, int xPos, int yPos)
    {
        RectTransform rectTransform = item.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);
        inventoryItemSlot[xPos, yPos] = item;

        Vector2 pos = new Vector2();
        pos.x = xPos * tileSizeWidth + (tileSizeWidth / 2);
        pos.y = -(yPos * tileSizeHeight + (tileSizeHeight / 2));

        rectTransform.localPosition = pos;
    }

}

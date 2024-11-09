using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHighlight : MonoBehaviour
{
    [SerializeField] RectTransform highlight;

    public void Show(bool b)
    {
        highlight.gameObject.SetActive(b);
    }

    public void SetSize(InventoryItem item)
    {
        highlight.sizeDelta = new Vector2(item.data.width * ItemGrid.tileSizeWidth, item.data.height * ItemGrid.tileSizeHeight);
    }

    public void SetPosition(ItemGrid grid, InventoryItem item)
    {
        Vector2 pos = grid.CalculatePositionOnGrid(item, item.onGridPosX, item.onGridPosY);

        highlight.localPosition = pos;
    }

    public void SetPosition(ItemGrid grid, InventoryItem item, int x, int y)
    {
        Vector2 pos = grid.CalculatePositionOnGrid(item, x, y);
        highlight.localPosition = pos;
    }

    public void SetParent(ItemGrid grid)
    {
        if (grid == null) return;
        highlight.SetParent(grid.GetComponent<RectTransform>());
    }
}

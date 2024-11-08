using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "New Item Data", menuName = "Assets/Items/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Variables")]
    public float itemID;
    public string itemName;

    [Header("UI Variables")]
    public int width = 1;
    public int height = 1;

    public Sprite itemIcon;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;
    public int stackSize;
    public int OnGridPositionX;
    public int OnGridPositionY;

    public void Set(ItemData itemData) {
        this.itemData = itemData;

        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = itemData.width * ItemGrid.tileSizeWidth;
        size.y = itemData.height * ItemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public InventoryItem(ItemData item) {
        itemData = item;
        AddToStack();
    }

    public void AddToStack() {
        stackSize++;
    }

    public void RemoveFromStack() {
        stackSize--;
    }
}

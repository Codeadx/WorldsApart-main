using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{

    public const float tileSizeWidth = 30f;
    public const float tileSizeHeight = 30f;
    RectTransform rectTransform;
    Vector2 positionOnGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();
    InventoryItem[,] inventoryItemSlot;
    [SerializeField] int gridSizeWidth = 10;
    [SerializeField] int gridSizeHeight = 18;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
    }

    private void Init(int width, int height) {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }

    internal InventoryItem GetItem(int x, int y) {
        return inventoryItemSlot[x, y];
    }

    public InventoryItem PickUpItem(int x, int y) {
        InventoryItem item = inventoryItemSlot[x, y];

        if (item == null) { return null; }

        ClearGrid(item);

        return item;
    }

    private void ClearGrid(InventoryItem item) {
        for (int ix = 0; ix < item.itemData.width; ix++)
        {
            for (int iy = 0; iy < item.itemData.height; iy++)
            {
                inventoryItemSlot[item.OnGridPositionX + ix, item.OnGridPositionY + iy] = null;
            }
        }
    }

    public Vector2Int GetGridPosition(Vector2 mousePosition) {
        positionOnGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnGrid.y = rectTransform.position.y - mousePosition.y;

        tileGridPosition.x = (int)(positionOnGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnGrid.y / tileSizeHeight);
       
        return tileGridPosition;
    }

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert) {
        int height = gridSizeHeight;
        int width = gridSizeWidth;
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                if (CheckAvailableSpace(x, y, itemToInsert.itemData.width, itemToInsert.itemData.height) == true) {
                    return new Vector2Int (x, y);
                }
            }
        }
        return null;
    }

    private bool CheckAvailableSpace(int posX, int posY, int width, int height) {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (inventoryItemSlot[posX + x, posY + y] != null) {
                    return false;
                }
            }
        }
        return true;
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem) {
        if (Boundaries(posX, posY, inventoryItem.itemData.width, inventoryItem.itemData.height) == false) {
            return false;
        }
        if (OverlapCheck(posX, posY, inventoryItem.itemData.width, inventoryItem.itemData.height, ref overlapItem) == false) {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null) {
            ClearGrid(overlapItem);
        }

        PlaceItem(inventoryItem, posX, posY);

        return true;
    }

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY) {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);
        for (int x = 0; x < inventoryItem.itemData.width; x++) {
            for (int y = 0; y < inventoryItem.itemData.height; y++) {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.OnGridPositionX = posX;
        inventoryItem.OnGridPositionY = posY;
        Vector2 position = CalculatePosition(inventoryItem, posX, posY);

        rectTransform.localPosition = position;
    }

    public Vector2 CalculatePosition(InventoryItem inventoryItem, int posX, int posY) {
        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.itemData.width / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.itemData.height / 2);
        return position;
    }

    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem) {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (inventoryItemSlot[posX + x, posY + y] != null) {
                    if (overlapItem == null) {
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    } else {
                        if (overlapItem != inventoryItemSlot[posX + x, posY + y])
                        return false;
                    }
                }
            }
        }
        return true;
    }

    bool PositionCheck(int posX, int posY) {
        if (posX < 0 || posY < 0) {
            return false;
        }

        if (posX >= gridSizeWidth || posY >= gridSizeHeight) {
            return false;
        }

        return true;
    }

    public bool Boundaries(int posX, int posY, int width, int height) {
        if (PositionCheck(posX, posY) == false) { return false; }

        posX += width - 1;
        posY += height - 1;

        if (PositionCheck(posX, posY) == false) { return false; }
        return true;
    }
}

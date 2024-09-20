using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public ItemGrid selectedItemGrid;
    public ItemGrid itemGrid;
    public ItemData itemData;
    public List<InventoryItem> inventory = new List<InventoryItem>();
    private Dictionary<ItemData, InventoryItem> itemDictionary = new Dictionary<ItemData, InventoryItem>();
    InventoryItem selectedItem;
    InventoryItem overlapitem;
    RectTransform rectTransform;
    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;
    public bool EnableRandomItem = false;

    private void OnEnable() {
        ItemToCollect.OnItemCollected += Add;
    }
    
    private void OnDisable() {
        ItemToCollect.OnItemCollected -= Add;
    }

    private void Update() {
        ItemIconDrag();

        if (Input.GetMouseButtonDown(0) && selectedItemGrid != null) {
            ItemClicked();
        }

        if (EnableRandomItem == true) {
            if (Input.GetKeyDown(KeyCode.E)) {
                InsertRandomItem();
            }
        }
    }

    private void InsertRandomItem() {
        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    private void CreateRandomItem() {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        
        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }

    public void Set(ItemData itemData) {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        inventoryItem.GetComponent<Image>().sprite = itemData.itemIcon;
        inventoryItem.itemData = itemData;
        Vector2 size = new Vector2();
        size.x = itemData.width * ItemGrid.tileSizeWidth;
        size.y = itemData.height * ItemGrid.tileSizeHeight;
        inventoryItem.GetComponent<RectTransform>().sizeDelta = size;
        selectedItem = null;
        InsertItem(inventoryItem);
    }

    public void Add(ItemData itemData) {
        if(itemDictionary.TryGetValue(itemData, out InventoryItem item)) {
            item.AddToStack();
            Set(itemData);
            Debug.Log($"{item.itemData.name} total stack is now {item.stackSize}");
        } else {
            InventoryItem newItem = new InventoryItem(itemData);
            inventory.Add(newItem);
            itemDictionary.Add(itemData, newItem);
            Set(itemData);
            Debug.Log($"Added {itemData.name} to the inventory.");
        }
    }

    public void Remove(ItemData itemData) {
        if(itemDictionary.TryGetValue(itemData, out InventoryItem item )) {
            item.RemoveFromStack();
            if(item.stackSize == 0) {
                inventory.Remove(item);
                itemDictionary.Remove(itemData);
            }
        }
    }

    private void InsertItem(InventoryItem itemToInsert) {
        Vector2Int? posOnGrid = itemGrid.FindSpaceForObject(itemToInsert);
        if (posOnGrid == null) { return; }

        itemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void ItemClicked() {
        Vector2Int tileGridPosition = GetTileGridPosition();

        if (selectedItem == null) {
            PickUpItem(tileGridPosition);
        } else {
            PlaceItem(tileGridPosition);
        }
    }

    private Vector2Int GetTileGridPosition() {
        Vector2 position = Input.mousePosition;
        if (selectedItem != null) {
            position.x -= (selectedItem.itemData.width - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.itemData.height - 1) * ItemGrid.tileSizeHeight / 2;
        }
        return selectedItemGrid.GetGridPosition(position);
    }

    private void PlaceItem(Vector2Int tileGridPosition) {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapitem);

        if (complete == true) {
            selectedItem = null;
            if (overlapitem != null) {
                selectedItem = overlapitem;
                overlapitem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
            }
        }
    }

    private void PickUpItem(Vector2Int tileGridPosition) {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null) {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }

    private void ItemIconDrag() {
        if (selectedItem != null) {
            rectTransform.position = Input.mousePosition;
        }
    }
}

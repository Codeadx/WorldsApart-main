using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToCollect : MonoBehaviour
{
    public static event HandleItemCollection OnItemCollected;
    public delegate void HandleItemCollection(ItemData itemData);
    public ItemData itemData;

    public void Collect() {
        Destroy(gameObject);
        OnItemCollected?.Invoke(itemData);
    }
}

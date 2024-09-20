using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision) {
        ItemToCollect itemCollect = collision.GetComponent<ItemToCollect>();
        if (itemCollect != null) {
            itemCollect.Collect();
        }
    }

}

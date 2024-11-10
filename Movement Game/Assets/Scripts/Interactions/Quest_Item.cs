using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_Item : Interactable
{
    public MovementScript keyItem;

    public override void Interact()
    {
        //Debug.Log("Picked up " + data.itemName);
        keyItem.enabled = true;
        gameObject.SetActive(false);
    }
}

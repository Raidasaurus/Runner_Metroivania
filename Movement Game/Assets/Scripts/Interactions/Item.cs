using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    public override void Interact()
    {
        Debug.Log("Picked up " + itemName);
        gameObject.SetActive(false);
    }
}

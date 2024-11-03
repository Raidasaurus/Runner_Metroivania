using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Item Variables")]
    public float itemID;
    public string itemName;

    public abstract void Interact();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered");

            // Get reference to playerUI
            PlayerUI ui = other.GetComponent<PlayerUI>();
            if (ui == null) return;

            // UI
            ui.FadeInInteractUI();

            // Interactable
            if (!ui.pm.inventory.interactables.Contains(this))
                ui.pm.inventory.interactables.Add(this);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited");

            // Get reference to player manager
            PlayerUI ui = other.GetComponent<PlayerUI>();
            if (ui == null) return;

            // UI
            ui.FadeOutInteractUI();

            // Interactable
            if (ui.pm.inventory.interactables.Contains(this))
                ui.pm.inventory.interactables.Remove(this);
        }
    }
}

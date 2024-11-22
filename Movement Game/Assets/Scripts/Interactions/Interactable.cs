using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public ItemData data;

    public abstract void Interact();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            

            // Get reference to playerUI
            PlayerUI ui = other.GetComponent<PlayerUI>();
            if (ui == null) return;

            // UI
            ui.interactText.text = "Interact";
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

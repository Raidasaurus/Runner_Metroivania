using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Interactable")]
    public List<Interactable> interactables = new List<Interactable>();
    public Interactable currentInteractable;

    [Header("Items")]
    public List<Item> items = new List<Item>();

    [Header("References")]
    [SerializeField] GridController gc;
    PlayerManager pm;
    PlayerUI ui;

    private void Start()
    {
        pm = GetComponent<PlayerManager>();
        ui = GetComponent<PlayerUI>();
        gc = FindObjectOfType<GridController>();
    }
    private void Update()
    {
        currentInteractable = CheckForClosestInteractable();

        if (Input.GetKeyDown(pm.keybind.interactKey) && currentInteractable != null)
        {
            if (currentInteractable is Item)
            {
                items.Add((Item)currentInteractable);
                if (pm.inventory.interactables.Contains(currentInteractable))
                    pm.inventory.interactables.Remove(currentInteractable);

                gc.AddItem((Item)currentInteractable);
            }
            ui.FadeOutInteractUI();
            currentInteractable.Interact();
        }
    }

    Interactable CheckForClosestInteractable()
    {
        Interactable closestItem = null;
        float closestAngle = Mathf.Infinity;

        foreach (var item in interactables)
        {
            Vector3 dir = item.transform.position - transform.position;
            float theta = Vector3.Angle(Camera.main.transform.forward, dir);

            if (theta < closestAngle)
            {
                closestAngle = theta;
                closestItem = item;
            }
        }

        return closestItem;
    }
}

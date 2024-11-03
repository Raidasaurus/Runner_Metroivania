using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GridController gc;
    [SerializeField] ItemGrid ig;

    private void Awake()
    {
        gc = FindObjectOfType(typeof(GridController)) as GridController;
        ig = GetComponent<ItemGrid>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gc.selectedItemGrid = ig;
        Debug.Log("Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gc.selectedItemGrid = null;
        Debug.Log("Exit");
    }
}

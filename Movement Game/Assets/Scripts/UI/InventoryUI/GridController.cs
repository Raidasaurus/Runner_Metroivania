using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public ItemGrid selectedItemGrid;

    // Update is called once per frame
    void Update()
    {
        if (selectedItemGrid == null) return;

        if (Input.GetMouseButton(0))
        {
            Debug.Log(selectedItemGrid.GetGridPosition(Input.mousePosition));
        }
    }
}

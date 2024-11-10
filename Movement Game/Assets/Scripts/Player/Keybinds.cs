using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "New Keybind Data", menuName = "Assets/Player/Keybind Data")]
public class Keybinds : ScriptableObject
{
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode interactKey = KeyCode.F;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode grappleKey = KeyCode.Mouse1;
    public KeyCode inventoryKey = KeyCode.Tab;
}

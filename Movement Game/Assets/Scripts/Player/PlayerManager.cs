using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [Header("Conditions")]
    public bool sliding;
    public bool wallrunning;
    public bool climbing;
    public bool dashing;
    public bool grappling;
    public bool freeze;

    [Header("Speed Values")]
    public float walkSpeed;
    [Range(0,1)]
    public float strafeFactor;
    public float airSpeed;
    public float slideSpeed;
    public float crouchSpeed;
    public float wallRunSpeed;
    public float dashSpeed;

    [Header("References")]
    public PlayerCamera cam;
    public Inventory inventory;
    public Transform orientation;
    public Animator aniUI;
    public Keybinds keybind;
}

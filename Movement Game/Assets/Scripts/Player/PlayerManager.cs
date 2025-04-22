using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("Stats")]
    public float TotalHP;
    public float maxHP;
    public float hp;
    public float o2;
    public float hpRegenRate;

    [Header("Conditions")]
    public bool sliding;
    public bool wallrunning;
    public bool crouching;
    public bool climbing;
    public bool dashing;
    public bool grappling;
    public bool freeze;

    [Header("Speed Values")]
    public float walkSpeed;
    [Range(0,1)]
    public float accelFactor;
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
    public Helper helper;

    [Header("Inputs")]
    public InputAction move;
    public InputAction attack;
    public InputAction jump;
    public InputAction crouch;
    public InputAction openInventory;
    public InputAction interact;
    public InputAction dash;
    public InputAction sprint;
    
    [HideInInspector] public PlayerControls playerControls;



    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        attack = playerControls.Player.Fire;
        attack.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();

        crouch = playerControls.Player.Crouch;
        crouch.Enable();

        openInventory = playerControls.Player.OpenInventory;
        openInventory.Enable();

        interact = playerControls.Player.Interact;
        interact.Enable();

        dash = playerControls.Player.Dash;
        dash.Enable();

        sprint = playerControls.Player.Sprint;
        sprint.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        attack.Disable();
        jump.Disable();
        crouch.Disable();
        openInventory.Disable();
        interact.Disable();
        dash.Disable();
        sprint.Disable();
    }
}

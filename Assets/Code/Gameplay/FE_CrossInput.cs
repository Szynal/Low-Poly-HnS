using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A helper class, allows for easier use of input axes in Unity
public static class FE_CrossInput
{
    /// <summary>
    /// Handles movement forwards and backwards (on PS1 - D-pad up and down)
    /// Returns values [-1.0f; 1.0f] => up is positive, while down is negative
    /// </summary>
    public static float MoveY()
    {
        return Input.GetAxisRaw("Vertical");
    }
    ///<summary>
    ///Handles rotating to left and right (on PS1 - D-pad right and left)
    ///Returns values [-1.0f; 1.0f] => right is positive, while left is negative
    ///</summary>
    public static float MoveX()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public static float MenuUpDown()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public static float MenuLeftRight()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public static float RotateY()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public static float RotateX()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    //Returns true when we release the rotation button - used for blocking roll when spinning
    public static bool EndRotation()
    {
        return Input.GetAxisRaw("Horizontal") == 0f;
    }

    public static bool Run()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }
    //Handles instantly rotating character
    //Only rotates once, then we need to click it again for it to work
    public static bool AboutFace()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        return Input.GetButtonDown("AboutFace") || Input.GetAxisRaw("AboutFace") > 0f;
#else
        return Input.GetKeyDown(KeyCode.R);
#endif
    }

    public static bool EndAboutFace()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        return Input.GetButtonUp("AboutFace") || Input.GetAxisRaw("AboutFace") <= 0f;
#else
        return Input.GetKeyDown(KeyCode.R);
#endif
    }

    //Handles crouching. This one is called when we click the button for crouching
    public static bool Crouch()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    //Handles rolling. This one is called when we click the button for roll
    public static bool StartRoll()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        return Input.GetButtonDown("Roll") || Input.GetAxisRaw("Roll") > 0f;
#else
        return Input.GetKeyDown(KeyCode.Q);
#endif
    }
    //Handles crouching. This one is called when we release the button for crouching
    public static bool EndRoll()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        return Input.GetButtonUp("Roll") || Input.GetAxisRaw("Roll") <= 0f;
#else
        return Input.GetKeyDown(KeyCode.Q);
#endif
    }
    //Handles moving through inventory
    public static float InventoryLeftRight()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public static float InventoryUpDown()
    {
        return Input.GetAxisRaw("Vertical");
    }

    //Handles moving through inventory
    public static bool OpenInventory()
    {
        return Input.GetKeyDown(KeyCode.I);
    }

    public static bool CloseInventory()
    {
        return Input.GetKeyDown(KeyCode.I);
    }

    //Using items
    public static bool UseItem()
    {
        return Input.GetKeyDown(KeyCode.F);
    }

    public static bool UseHeld()
    {
        return Input.GetKeyDown(KeyCode.F);
    }
    //Attacking
    public static bool Attack()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        return Input.GetButton("Attack") || Input.GetAxisRaw("Attack") < 0f;
#else
        return Input.GetKeyDown(KeyCode.Space);
#endif
    }

    public static bool ShowControllerInputMap()
    {
        return Input.GetKeyDown(KeyCode.Home);
    }

    public static bool ShowKeyboardInputMap()
    {
        return Input.GetKeyDown(KeyCode.M);
    }

    public static bool ShowMenu()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public static bool MenuCancel()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public static bool HolsterWeapon()
    {
        return Input.GetKeyDown(KeyCode.R);
    }
}
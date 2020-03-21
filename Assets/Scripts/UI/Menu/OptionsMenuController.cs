using System.Collections;
using System.Collections.Generic;
using LowPolyHnS;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : SubMenuController
{
    [Header("Properties for options menu controller")]
    [SerializeField] Toggle characterBasedMovementToggle = null;

    public override void Show(SubMenuController _parent)
    {
        characterBasedMovementToggle.isOn = GameManager.Instance.Settings.UseCharacterBasedMovement;

        base.Show(_parent);
    }

    public override void Exit()
    {
        GameManager.Instance.Settings.UseCharacterBasedMovement = characterBasedMovementToggle.isOn;
        GameManager.Instance.SaveGame();

        base.Exit();
    }
}

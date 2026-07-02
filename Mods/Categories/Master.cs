using GorillaLocomotion;
using System;
using System.Collections.Generic;
using System.Text;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Master
{
    public static void GreyScreen()
    {
        if (GreyZoneManager.Instance == null) return;

        if (!Variables.IsMaster()) return;

        GreyZoneManager.Instance.ActivateGreyZoneAuthority();

        GTPlayer.Instance?.SetGravityOverride(
            GreyZoneManager.Instance,
            GreyZoneManager.Instance.GravityOverrideFunction
        );
    }

    public static void DisableGreyScreen()
    {
        if (GreyZoneManager.Instance == null) return;

        if (!Variables.IsMaster()) return;

        GTPlayer.Instance?.UnsetGravityOverride(GreyZoneManager.Instance);

        GreyZoneManager.Instance.DeactivateGreyZoneAuthority();
    }
}
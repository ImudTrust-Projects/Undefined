using GorillaLocomotion;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
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
        
        // Patched
        /*GTPlayer.Instance?.SetGravityOverride(
            GreyZoneManager.Instance,
            GreyZoneManager.Instance.GravityOverrideFunction
        );*/
    }

    public static void DisableGreyScreen()
    {
        if (GreyZoneManager.Instance == null) return;

        if (!Variables.IsMaster()) return;

        GTPlayer.Instance?.UnsetGravityOverride(GreyZoneManager.Instance);

        GreyZoneManager.Instance.DeactivateGreyZoneAuthority();
    }

    public static void GuardianAll()
    {
        if (!Variables.IsMaster()) return;

        int Players = 0;

        foreach (var ZoneManager in GorillaGuardianZoneManager.zoneManagers
                 .Where(g => g.enabled && g.IsZoneValid()))
        {
            ZoneManager.SetGuardian(PhotonNetwork.PlayerList[Players]);
            Players++;
        }
    }
    
    public static void DisableGuardianAll()
    {
        foreach (var ZoneManager in GorillaGuardianZoneManager.zoneManagers.Where(gorillaGuardianZoneManager => gorillaGuardianZoneManager.enabled && gorillaGuardianZoneManager.IsZoneValid()))
        {
            ZoneManager.SetGuardian(null);
        }
    }
}
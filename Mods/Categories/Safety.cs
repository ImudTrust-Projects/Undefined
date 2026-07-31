using System.Collections.Generic;
using System.Diagnostics;
using GorillaNetworking;
using Photon.Voice.Unity;
using System.Linq;
using HarmonyLib;
using Photon.Pun;
using Undefined.Utilities;
using UnityEngine;

namespace Undefined.Mods.Categories;

public class Safety
{
    private static float delay;

    public static void AntiReport()
    {
        if (!NetworkSystem.Instance.InRoom) return;

        foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
        {
            if (line.linePlayer != NetworkSystem.Instance.LocalPlayer) continue;

            Transform report = line.reportButton.gameObject.transform;

            foreach (VRRig vrrig in VRRigCache.ActiveRigs.Where(v =>
                         !v.isLocal &&
                         (Vector3.Distance(v.rightHandTransform.position, report.position) < 0.35f ||
                          Vector3.Distance(v.leftHandTransform.position, report.position) < 0.35f)))
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();

                if (!(Time.time > delay)) return;
                delay = Time.time + 1f;
            }
        }
    }

    public static void RestartGame()
    {
        Process.Start("steam://rungameid/1533390");
        Application.Quit();
    }

    private static string[] badcosmetic = new string[]
    {
        "LBAAk", "LBAAD", "LMAPY"
    };

    public static void AntiModeration()
    {
        foreach (VRRig rigs in VRRigCache.ActiveRigs)
        {
            var cosmetic = Traverse.Create(rigs).Field<HashSet<string>>("_playerOwnedCosmetics").Value;
            foreach (var verybadcosmetic in cosmetic)
            {
                if (cosmetic.Contains(verybadcosmetic))
                {
                    if (!PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED"))
                    {
                        NetworkSystem.Instance.ReturnToSinglePlayer();
                        Variables.RPCProtection();
                        NotificationLib.SendNotification(NotificationLib.NotificationType.Room, "Kicked Due to Moderation in room");
                    }
                }
            }
        }
    }
    
    public static void NoFingerMovement()
    {
        ControllerInputPoller.instance.leftControllerGripFloat = 0f;
        ControllerInputPoller.instance.rightControllerGripFloat = 0f;
        ControllerInputPoller.instance.leftControllerIndexFloat = 0f;
        ControllerInputPoller.instance.rightControllerIndexFloat = 0f;
        ControllerInputPoller.instance.leftControllerPrimaryButton = false;
        ControllerInputPoller.instance.leftControllerSecondaryButton = false;
        ControllerInputPoller.instance.rightControllerPrimaryButton = false;
        ControllerInputPoller.instance.rightControllerSecondaryButton = false;
        ControllerInputPoller.instance.leftControllerPrimaryButtonTouch = false;
        ControllerInputPoller.instance.leftControllerSecondaryButtonTouch = false;
        ControllerInputPoller.instance.rightControllerPrimaryButtonTouch = false;
        ControllerInputPoller.instance.rightControllerSecondaryButtonTouch = false;
    }
}